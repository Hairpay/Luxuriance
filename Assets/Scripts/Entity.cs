using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public delegate void ExecuteState();
    protected ExecuteState _executeState;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    public float _moveSpeed;
    public Vector2 _velocity;
    public bool _isOnGround ;
    public bool _isDirectionRight;
    
    protected virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _velocity = Vector2.zero;
    }

    protected void Translate(Vector2 v)
    {
        transform.position = new Vector2
        (
            transform.position.x + v.x,
            transform.position.y + v.y
        );
    }

    protected bool IsOnGround()
    {
        float distGround = 0.1f;
        _isOnGround = Physics2D.Raycast( transform.position, Vector2.down, distGround );
        return _isOnGround;
    }
}