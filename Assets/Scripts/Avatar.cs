using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Avatar : Entity
{
    #region Members
    public float _jumpForce;
    public float _dashForce;
    public float _dashDuration;
    public float _dashRecovery;
    public int _maxDashes;
    public float _hopDuration;

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
            Vector2 mirrorScale = transform.localScale;
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
            if( IsOnGround && Input.GetButtonDown( "Jump" ) )
            {
                DoJump();
            }
            // Dash
            else if( Input.GetButtonDown( "Dash" ) && _dashesLeft > 0
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
            //if( !_timerManager.Exists( "IdleState" ) ) _timerManager.CreateTimer( "IdleState", 0.03f );
        }
        // On Update
        else if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x )
            && ( _collisionDirection.RightBottom && _isDirectionRight && !_collisionDirection.RightTop
                || _collisionDirection.LeftBottom && !_isDirectionRight && !_collisionDirection.LeftTop )
            && IsOnGround )
        {
            SetState( Hop );
        }
        else
        {
            float horizontalAxis = GetAxisAbsolute( "Horizontal" );
            _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );

            if( horizontalAxis == 0.0f /*&& _timerManager.IsOverOrNull( "IdleState" )*/)
            {
                SetState( Idle );
            }
            /*else
            {
                _timerManager.AddTimeToTimer( "IdleState", Time.deltaTime );
            }

            _lastAxis = horizontalAxis;*/
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
            DoJump();
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
            _rigidbody.velocity = new Vector2( _storeVelocity.x, _rigidbody.velocity.y );
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

    private void DoJump()
    {
        _rigidbody.AddForce( Vector2.up * _jumpForce );
    }
    #endregion
}