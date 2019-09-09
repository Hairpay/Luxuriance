using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public delegate void ExecuteState();
    protected ExecuteState _executeState;
    public Collider2D _collider { get => GetComponent<Collider2D>(); }
    public Rigidbody2D _rigidbody { get => GetComponent<Rigidbody2D>(); }
    public Animator _animator { get => GetComponent<Animator>(); }
    public float _moveSpeed;
    //public Vector2 _velocity = new Vector2( 0, 0 );
    //public Vector2 _acceleration = new Vector2( 0, -9.81f );
    public bool _isOnGround = false;
    
    protected void Translate(Vector2 v)
    {
        transform.position = new Vector2
        (
            transform.position.x + v.x,
            transform.position.y + v.y
        );
    }

    public bool IsOnGround()
    {
        return _isOnGround;
    }
}