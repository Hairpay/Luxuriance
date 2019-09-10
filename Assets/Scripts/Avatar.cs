using UnityEngine;

public class Avatar : Entity
{
    #region Members
    private enum State { idle, move, jump };
    private State _state;

    private bool _isJumpEnabled;
    public float dist = 0.1f;
    #endregion

    #region Behaviour
    protected override void Awake()
    {
        base.Awake();
        _isDirectionRight = true;
    }

    private void Start()
    {
        SetState( State.idle );
    }

    private void FixedUpdate()
    {
        HandlePhysics();
    }

    private void Update()
    {
        _executeState();
    }

    private void HandlePhysics()
    {
        float horizontalAxis = Input.GetAxis( "Horizontal" );
        _rigidbody.velocity = new Vector2( horizontalAxis * _moveSpeed, _rigidbody.velocity.y );
        //_velocity.x = horizontalAxis * _moveSpeed;
        //transform.Translate( _rigidbody.velocity );
        bool col = Physics2D.Raycast( _collider.transform.position, Vector2.down, dist);
        Debug.Log( col );
        transform.Translate( _velocity );
    }

    private void HandleGraphics()
    {
    }

    private void HandleInputs()
    {
    }
    #endregion

    #region States
    private void Stands()
    {
        if( Utils.IsFloatSmallerThanEpsilon( _rigidbody.velocity.x ) == false )
        {
            SetState( State.move );
        }
    }

    private void Moves()
    {
        if( Utils.IsFloatSmallerThanEpsilon( _rigidbody.velocity.x ) == true )
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
        _state = state;
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