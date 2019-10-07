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

    private enum State { idle, walk, jump, dash, hop };
    private State _currentState;
    private TimerManager _timerManager;
    private Vector2 _shotDirection;
    private Vector2 _direction;
    public int _dashesLeft;
    #endregion

    #region Behaviour
    private void Start()
    {
        _timerManager = new TimerManager();
        _isDirectionRight = true;
        _dashesLeft = _maxDashes;
        SetState( State.idle );
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
        if( _collisionDirection.Bottom && !IsState( State.dash ) )
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
        _shotDirection.Set(Input.GetAxis("HorizontalDirection"), Input.GetAxis( "VerticalDirection" ) );
        // Jump
        if( _collisionDirection.Bottom && Input.GetButtonDown( "Jump" ) )
        {
            SetState( State.jump );
        }
        //Dash
        else if( Input.GetButtonDown( "Dash" )&& _dashesLeft > 0
            && _timerManager.IsOverOrNull( "DashRecovery") )
        {
            SetState( State.dash );
        }
    }

    private void HandeStates()
    {
        if( Input.GetButtonDown( "Dash" ) )
        {
            SetState( State.dash );
        }
        else if( Input.GetButtonDown( "Jump" ) )
        {
            SetState( State.jump );
        }
        else if( Input.GetButton( "Horizontal" ) )
        {
            SetState( State.walk );
        }
    }
    #endregion

    #region States
    private void Idles()
    {
        if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
        {
            SetState( State.walk );
        }
    }

    private void Walks()
    {
        float horizontalAxis = GetAxisAbsolute( "Horizontal" );
        _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );

        if( horizontalAxis == 0.0f )
        {
            SetState( State.idle );
        }
    }

    private void Jumps()
    {
        if( _collisionDirection.Bottom )
        {
            SetState( State.idle );
        }
    }

    private void Dashes()
    {
        if( _timerManager.IsOverOrNull( "DashDuration") )
        {
            _rigidbody.gravityScale = _gravityScaleDefault;
            //_rigidbody.velocity = Vector2.zero;
            _timerManager.CreateTimer( "DashRecovery", _dashRecovery );
            SetState( State.idle );
        }
    }
    #endregion

    #region Setters
    private void SetState( State state )
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
    }
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

    private bool IsState(State state )
    {
        return _currentState == state;
    }
    #endregion
}