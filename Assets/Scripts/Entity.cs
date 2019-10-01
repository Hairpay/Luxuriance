using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    #region Members
    public delegate void ExecuteState();

    public float _moveSpeed;
    public bool _isDirectionRight;
    public float _collisionDetectionDistance;
    public bool _drawCollisionsEnabled;

    protected class CollisionDirection
    {
        public bool Bottom { get; set; }
        public bool Top { get; set; }
        public bool LeftBottom { get; set; }
        public bool RightBottom { get; set; }
        public bool LeftTop { get; set; }
        public bool RightTop { get; set; }

        public bool Any()
        {
            bool any = Bottom || Top || LeftBottom || RightBottom || LeftTop || RightTop;
            return any;
        }

        public bool AnySide()
        {
            bool anySide = LeftBottom || RightBottom || LeftTop || RightTop;
            return anySide;
        }

        public bool None()
        {
            return !Any();
        }

        public override string ToString()
        {
            if( None() )
            {
                return string.Empty;
            }

            string result = "Collisions:\n";

            if( Bottom )
                result += "Bottom";
            if( Top )
                result += ", Top";
            if( LeftBottom )
                result += ", LeftBottom";
            if( RightBottom )
                result += ", RightBottom";
            if( LeftTop )
                result += ", LeftTop";
            if( RightTop )
                result += ", RightTop";

            return result;

        }
    }

    protected CollisionDirection _collisionDirection;

    protected ExecuteState _executeState;
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    protected float _gravityScaleDefault;
    #endregion

    #region Behaviour // Always call virtual methods within corresponding derived class overriden methods !
    private void Awake()
    {
        _collisionDirection = new CollisionDirection();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _gravityScaleDefault = _rigidbody.gravityScale;
    }

    protected virtual void Update()
    {
        _executeState();
    }

    protected virtual void FixedUpdate()
    {
        CheckCollisions();

        //if( _collisionDirection.Any() )
        //{
        //    Debug.Log( _collisionDirection.ToString() );
        //}
    }
    #endregion

    protected void CheckCollisions()
    {
        float offset = 0.01f;

        #region Collision: Bottom
        Vector2 colliderBottomLeft = new Vector2( _collider.bounds.min.x, _collider.bounds.min.y - offset );
        Vector2 colliderBottomRight = new Vector2( _collider.bounds.min.x + _collider.bounds.size.x, _collider.bounds.min.y - offset );

        _collisionDirection.Bottom = Physics2D.Raycast( colliderBottomLeft, Vector2.down, _collisionDetectionDistance )
            || Physics2D.Raycast( colliderBottomRight, Vector2.down, _collisionDetectionDistance );
        #endregion

        #region Collision: Top
        Vector2 colliderTopLeft = new Vector2( _collider.bounds.min.x, _collider.bounds.max.y + offset );
        Vector2 colliderTopRight = new Vector2( _collider.bounds.max.x, _collider.bounds.max.y + offset );

        _collisionDirection.Top = Physics2D.Raycast( colliderTopLeft, Vector2.up, _collisionDetectionDistance )
            || Physics2D.Raycast( colliderTopRight, Vector2.up, _collisionDetectionDistance );
        #endregion

        #region Collision: Left
        Vector2 colliderLeftBottom = new Vector2( _collider.bounds.min.x - offset, _collider.bounds.min.y );
        Vector2 colliderLeftTop = new Vector2( _collider.bounds.min.x - offset, _collider.bounds.max.y );

        _collisionDirection.LeftBottom = Physics2D.Raycast( colliderLeftBottom, Vector2.left, _collisionDetectionDistance );
        _collisionDirection.LeftTop = Physics2D.Raycast( colliderLeftTop, Vector2.left, _collisionDetectionDistance );
        #endregion

        #region Collision: Right
        Vector2 colliderRightBottom = new Vector2( _collider.bounds.max.x + offset, _collider.bounds.min.y );
        Vector2 colliderRightTop = new Vector2( _collider.bounds.max.x + offset, _collider.bounds.max.y );

        _collisionDirection.RightBottom = Physics2D.Raycast( colliderRightBottom, Vector2.right, _collisionDetectionDistance );
        _collisionDirection.RightTop = Physics2D.Raycast( colliderRightTop, Vector2.right, _collisionDetectionDistance );
        #endregion

        #region Draw Rays
        if( _drawCollisionsEnabled )
        {
            Debug.DrawRay( colliderBottomLeft, new Vector2( 0, -_collisionDetectionDistance ), Color.cyan );
            Debug.DrawRay( colliderBottomRight, new Vector2( 0, -_collisionDetectionDistance ), Color.cyan );
            Debug.DrawRay( colliderTopLeft, new Vector2( 0, _collisionDetectionDistance ), Color.cyan );
            Debug.DrawRay( colliderTopRight, new Vector2( 0, _collisionDetectionDistance ), Color.cyan );
            Debug.DrawRay( colliderLeftBottom, new Vector2( -_collisionDetectionDistance, 0 ), Color.cyan );
            Debug.DrawRay( colliderLeftTop, new Vector2( -_collisionDetectionDistance, 0 ), Color.cyan );
            Debug.DrawRay( colliderRightBottom, new Vector2( _collisionDetectionDistance, 0 ), Color.cyan );
            Debug.DrawRay( colliderRightTop, new Vector2( _collisionDetectionDistance, 0 ), Color.cyan );
        }
        #endregion
    }

    #region Other
    protected void SetVelocity(float? x = null, float? y = null)
    {
        float newX = x ?? _rigidbody.velocity.x;
        float newY = y ?? _rigidbody.velocity.y;
        _rigidbody.velocity.Set( newX, newY );
    }
    #endregion
}