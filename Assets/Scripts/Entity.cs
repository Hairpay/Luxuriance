using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    #region Members
    public delegate void ExecuteState();

    public float _moveSpeed;
    public float _jumpForce;
    public bool _isOnGround;
    public bool _isDirectionRight;
    public float _groundCollisionDetectionDistance;
    public Transform _ground;

    protected ExecuteState _executeState;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    #endregion

    #region Behaviour // Always call virtual methods within corresponding derived class overriden methods !
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        _executeState();
    }

    protected virtual void FixedUpdate()
    {
        CheckGroundCollision();
        Debug.Log( _isOnGround.ToString() );
    }
    #endregion

    protected bool CheckGroundCollision()
    {
        float offset = -0.01f;
        Vector2 colliderBottomLeft = new Vector2( _collider.bounds.min.x, _collider.bounds.min.y + offset );
        Vector2 colliderBottomRight = new Vector2( _collider.bounds.min.x + _collider.bounds.size.x, _collider.bounds.min.y + offset );

        //Debug.DrawRay( colliderBottomLeft, new Vector2( 0, -_groundCollisionDetectionDistance ), Color.cyan );
        //Debug.DrawRay( colliderBottomRight, new Vector2( 0, -_groundCollisionDetectionDistance ), Color.cyan );

        _isOnGround = Physics2D.Raycast( colliderBottomLeft, Vector2.down, _groundCollisionDetectionDistance )
            || Physics2D.Raycast( colliderBottomRight, Vector2.down, _groundCollisionDetectionDistance );

        return _isOnGround;
    }
}