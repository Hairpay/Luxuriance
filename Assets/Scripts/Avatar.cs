using UnityEngine;

public class Avatar : Entity
{
    #region Members
    private enum State { idle, move };
    private State _state;

    private bool _isJumpEnabled;
    #endregion

    #region Behaviour
    private void Start()
    {
        SetState( State.idle );
    }

    private void Update()
    {
        HandlePhysics();
        _executeState();
    }

    private void HandlePhysics()
    {
        //_rigidbody.velocity.x = Input.GetAxis( "Horizontal" );
        //Translate( _rigidbody.velocity );
        _rigidbody.velocity = new Vector2( Input.GetAxis( "Horizontal" ) * _moveSpeed, _rigidbody.velocity.y );
        transform.Translate( _rigidbody.velocity );
    }

    private void HandleInputs()
    {
    }
    #endregion

    #region States
    private void Stands()
    {
        if( Utils.IsFloatGreaterThanEpsilon(_rigidbody.velocity.x) )
        {
            Vector2 mirrorScale = transform.localScale;
            mirrorScale.x = Mathf.Abs( mirrorScale.x );
            transform.localScale = mirrorScale;
            SetState( State.move );
        }
        else if( !Utils.IsFloatGreaterThanEpsilon( _rigidbody.velocity.x, -0.01f) )
        {
            Vector2 mirrorScale = transform.localScale;
            mirrorScale.x = -Mathf.Abs( mirrorScale.x );
            transform.localScale = mirrorScale;
            SetState( State.move );
        }
    }

    private void Moves()
    {
        if( Utils.IsFloatEpsilon( _rigidbody.velocity.x ) )
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
                Debug.Log( "ORAORAORAORAORAORAORAORAORAORAORAORAORAORAORAORA" );
                _isJumpEnabled = true;
                _animator.Play( "Idle" );
                _executeState = Stands;
                break;
            case State.move:
                Debug.Log( "They see me rolling..." );
                _animator.Play( "Walk" );
                _executeState = Moves;
                break;
        }
    }
    #endregion

    #region Others
    #endregion

}