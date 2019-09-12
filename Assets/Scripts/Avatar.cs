using UnityEngine;

public class Avatar : Entity
{
    #region Members
    private enum State { idle, move, jump };
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
        float horizontalAxis = Input.GetAxisRaw( "Horizontal" );
        _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );
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
        // Jump
        if( _isOnGround && Input.GetButtonDown( "Jump" ) )
        {
            _rigidbody.AddForce( Vector2.up * _jumpForce );
            SetState( State.jump );
        }
    }
    #endregion

    #region States
    private void Stands()
    {
        if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x ) )
        {
            SetState( State.move );
        }
    }

    private void Moves()
    {
        if( Utils.IsFloatEpsilonZero( _rigidbody.velocity.x ) )
        {
            SetState( State.idle );
        }
    }

    private void Jumps()
    {
        if( _isOnGround )
        {
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
                Debug.Log( "stands" );
                _animator.Play( "Idle" );
                _executeState = Stands;
                break;

            case State.move:
                Debug.Log( "moves" );
                _animator.Play( "Walk" );
                _executeState = Moves;
                break;

            case State.jump:
                Debug.Log( "jumps" );
                _executeState = Jumps;
                break;
        }
    }
    #endregion

    #region Others
    #endregion
}