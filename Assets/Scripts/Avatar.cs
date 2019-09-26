using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Avatar : Entity
{
    #region Members
    public float _jumpForce;
    public float _dashForce;
    public float _dashDuration;

    private enum State { idle, walk, jump, dash, hop };
    private float timer;
    private Vector2 _shotDirection;
    private Vector2 _direction;
    #endregion

    #region Behaviour
    private void Start()
    {
        _isDirectionRight = true;
        SetState( State.idle );
    }

    protected override void Update()
    {
        HandleInputs();
        HandleGraphics();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        HandlePhysics();
        base.FixedUpdate();
    }

    private void HandlePhysics()
    {
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
        if( Input.GetButtonDown( "Dash" ) )
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
        timer -= Time.deltaTime;
        if( timer <= 0.0f || _collisionDirection.AnySide() )
        {
            _rigidbody.gravityScale = _gravityScaleDefault;
            SetVelocity( x: 0.0f );
            SetState( State.idle );
        }
    }
    #endregion

    #region Setters
    private void SetState( State state )
    {
        switch( state )
        {
            case State.idle:
                Debug.Log( "idles" );
                _animator.Play( "Idle" );
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
                timer = _dashDuration;
                _rigidbody.gravityScale = 0;
                SetVelocity( y: 0.0f );
                _rigidbody.AddForce( new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis( "Vertical" ) ).normalized * _dashForce );
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
    #endregion
}