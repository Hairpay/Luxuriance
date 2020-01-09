using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Avatar : Entity
{
    #region Members
    public float _jumpForce;
    public float _hopForce;
    public float _dashForce;
    public float _dashDuration;
    public float _dashRecovery;
    public int _maxDashes;

    private bool _controlsEnabled;
    private TimerManager _timerManager;
    private Vector2 _direction;
    private Vector2 _shotDirection;
    private int _dashesLeft;
    private Vector2 _storeVelocity;
    private float _lastAxis;
    #endregion

    #region Behaviour
    private void Start()
    {
        _timerManager = new TimerManager();
        _isDirectionRight = true;
        _controlsEnabled = true;
         _dashesLeft = _maxDashes;
        SetState( Idle );
    }

    protected override void Update()
    {
        HandleInputs();
        HandleGraphics();
        _timerManager.UpdateTimers();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        HandlePhysics();
        base.FixedUpdate();
    }

    private void HandlePhysics()
    {
        if( _collisionDirection.Bottom && IsOnGround )
        {
            _dashesLeft = _maxDashes;
        }
    }

    private void HandleGraphics()
    {
        // Flip graphics
        
        if( _isDirectionRight && _rigidbody.velocity.x < 0
            || !_isDirectionRight && _rigidbody.velocity.x > 0 )
        {
            Vector3 mirrorScale = transform.localScale;
            mirrorScale.x *= -1;
            transform.localScale = mirrorScale;
            _isDirectionRight = !_isDirectionRight;
        }
    }

    private void HandleInputs()
    {
        //Directions
        _shotDirection.Set( Input.GetAxis( "HorizontalDirection" ), Input.GetAxis( "VerticalDirection" ) );
        _direction.Set( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) );

        if( _controlsEnabled )
        {
            // Jump
            if( IsOnGround && Input.GetButtonDown( "Jump" )
                && ( _executeState != Hop && _executeState != Crouch && _executeState != Crawl ) )
            {
                DoJump( _jumpForce );
            }
            // Dash
            else if( Input.GetButtonDown( "Dash" ) && _dashesLeft > 0
                && ( _executeState != Hop && _executeState != Crouch && _executeState != Crawl )
                && _timerManager.IsOverOrNull( "DashRecovery" ) )
            {
                SetState( Dash );
            }
        }
    }
    #endregion

    #region States
    private void Idle()
    {
        // On State Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "idle" );
            _animator.Play( "Idle" );
            _isStateEntryMode = false;
        }
        // On Update
        else if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
        {
            SetState( Walk );
        }
    }

    private void Walk()
    {
        // On State Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "walk" );
            _animator.Play( "Walk" );
            _isStateEntryMode = false;
        }
        else if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x )  && IsOnGround
            && ( _collisionDirection.RightTop && _isDirectionRight && !_collisionDirection.RightBottom
                || _collisionDirection.LeftTop && !_isDirectionRight && !_collisionDirection.LeftBottom ) )
        {
            _collider.transform.Rotate( 0.0f, 0.0f, 90.0f );
            _collider.transform.Translate( _collider.bounds.size.x * 0.5f, _collider.bounds.size.y * -0.5f, 0.0f );
            _collisionHitDistanceTopLength = 6.0f;
            // need to modify the distance between the two detection rays
            SetState( Crawl );
        }
        // On Update
        else if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x )
            && ( _collisionDirection.RightBottom && _isDirectionRight && !_collisionDirection.RightTop
                || _collisionDirection.LeftBottom && !_isDirectionRight && !_collisionDirection.LeftTop ) )
        {
            SetState( Hop );
        }
        else
        {
            float horizontalAxis = GetAxisAbsolute( "Horizontal" );
            _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );

            if( horizontalAxis == 0.0f )
            {
                SetState( Idle );
            }
        }
    }

    private void Dash()
    {
        // On State Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "dash" );
            _animator.Play( "Dash" );
            _timerManager.CreateTimer( "DashDuration", _dashDuration );
            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;

            if( Utils.IsFloatEpsilonZero( Input.GetAxis( "Horizontal" ) )
                && Utils.IsFloatEpsilonZero( Input.GetAxis( "Vertical" ) ) )
            {
                _rigidbody.AddForce( ( _isDirectionRight ? Vector2.right : Vector2.left ) * _dashForce );
            }
            else
            {
                _rigidbody.AddForce( _direction.normalized * _dashForce );
            }
            _isStateEntryMode = false;
        }
        // On Update
        else if( _timerManager.IsOverOrNull( "DashDuration" ) )
        {
            --_dashesLeft;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.gravityScale = _gravityScaleDefault;
            _timerManager.CreateTimer( "DashRecovery", _dashRecovery );
            SetState( Idle );
        }
    }

    private void DashFreeze()
    {
        // On Entry
        if( _isStateEntryMode )
        {
            _isStateEntryMode = false;
        }
        // On Update
        else 
        {

        }
    }

    private void Hop()
    {
        // On Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "hop" );
            _animator.Play( "Hop" );
            float horizontalAxis = GetAxisAbsolute( "Horizontal" );
            _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, 0.0f );
            _storeVelocity = _rigidbody.velocity;
            DoJump( _hopForce );
            _isStateEntryMode = false;
            _controlsEnabled = false;
        }
        // On Update
        else if( _collisionDirection.Bottom && IsOnGround )
        {
             _controlsEnabled = true;
            if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
            {
                if( _collisionDirection.RightBottom && _isDirectionRight && !_collisionDirection.RightTop
                || _collisionDirection.LeftBottom && !_isDirectionRight && !_collisionDirection.LeftTop )
                {
                    SetState( Hop );
                }
                else
                {
                    SetState( Walk );
                }
            }
            else
            {
                _rigidbody.velocity = Vector2.zero;
                SetState( Idle );
            }
        }
        else
        {
            if( !_collisionDirection.RightTop && !_collisionDirection.LeftTop )
            {
                _rigidbody.velocity = new Vector2( _storeVelocity.x, _rigidbody.velocity.y );
            }
        }
    }

    private void Crouch()
    {
        // On State Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "crouch" );
            _animator.Play( "Crouch" );
            _isStateEntryMode = false;
        }
        // On Update
        else if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
        {
            SetState( Crawl );
        }
    }

    private void Crawl()
    {// On State Entry
        if( _isStateEntryMode )
        {
            Debug.Log( "crawl" );
            _animator.Play( "Crawl" );
            _isStateEntryMode = false;
        }
        // On Update
        else if( !_collisionDirection.Top )
        {
            _collider.transform.Rotate( 0.0f, 0.0f, -90.0f );
            _collider.transform.Translate( _collider.bounds.size.x * -0.5f, _collider.bounds.size.y * -0.5f, 0.0f );
            _collisionHitDistanceTopLength = 1.0f;
            SetState( Walk );
        }
        else
        {
            float horizontalAxis = GetAxisAbsolute( "Horizontal" );
            _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );

            if( horizontalAxis == 0.0f )
            {
                SetState( Crouch );
            }
        }
    }
    #endregion
    
    #region Setters
    /*private void SetState( State state )
    {
        _currentState = state;
        switch( state )
        {
            case State.idle:
                Debug.Log( "idles" );
                _animator.Play( "Idle" );
                _rigidbody.velocity = Vector2.zero;
                _executeState = Idles;
                break;

            case State.walk:
                Debug.Log( "walks" );
                _animator.Play( "Walk" );
                _executeState = Walks;
                break;

            case State.jump:
                Debug.Log( "jumps" );
                _rigidbody.AddForce( Vector2.up * _jumpForce );
                _executeState = Jumps;
                break;

            case State.dash:
                Debug.Log( "dashes" );
                _animator.Play( "Dash" );
                _timerManager.CreateTimer( "DashDuration", _dashDuration );
                --_dashesLeft;
                _rigidbody.gravityScale = 0;
                _rigidbody.velocity = Vector2.zero;
                if( Utils.IsFloatEpsilonZero( Input.GetAxis( "Horizontal" ) )
                    && Utils.IsFloatEpsilonZero( Input.GetAxis( "Vertical" ) ) )
                {
                    _rigidbody.AddForce( ( _isDirectionRight ? Vector2.right : Vector2.left ) * _dashForce );
                }
                else
                {
                    _rigidbody.AddForce( new Vector2( GetAxisAbsolute( "Horizontal" ), GetAxisAbsolute( "Vertical" ) ).normalized * _dashForce );
                }
                _executeState = Dashes;
                break;
        }
    }*/
    #endregion

    #region Other
    private float GetAxisAbsolute(string axisName )
    {
        float axis = Input.GetAxis( axisName );
        if( axis > 0 )
            axis = 1.0f;
        else if( axis < 0 )
            axis = -1.0f;
        else
            axis = 0.0f;

        return axis;
    }

    private void DoJump(float force)
    {
        _rigidbody.AddForce( Vector2.up * force );
    }
    #endregion
}