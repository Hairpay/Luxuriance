using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Defines global behaviour of player and NPCs.
    /// All characters should derives from this abstract class
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class Entity : MonoBehaviour
    {
        #region Members
        public delegate void ExecuteState();

        public float MoveSpeed;
        public float CollisionHitDistance = 0.05f;
        public float CollisionHitDistanceBottomLength = 1.0f;
        public float CollisionHitDistanceTopLength = 1.0f;
        public float CollisionHitDistanceLeftLength = 1.0f;
        public float CollisionHitDistanceRightLength = 1.0f;
        public bool DrawCollisionsEnabled;

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
                var any = Bottom || Top || LeftBottom || RightBottom || LeftTop || RightTop;
                return any;
            }

            public bool AnySide()
            {
                var anySide = LeftBottom || RightBottom || LeftTop || RightTop;
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

                var result = "Collisions:\n";

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
        protected bool _isDirectionRight;
        protected bool _isStateEntryMode;
        protected float _gravityScaleDefault;
        protected ExecuteState _executeState;
        protected Collider2D _collider;
        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
        #endregion

        #region Behaviour // Always call virtual methods within corresponding derived class overriden methods !
        private void Awake()
        {
            _collisionDirection = new CollisionDirection();
            _collider = GetComponentInChildren<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _gravityScaleDefault = _rigidbody.gravityScale;
            _isStateEntryMode = false;
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

        protected void SetState( ExecuteState state )
        {
            _executeState = state;
            _isStateEntryMode = true;
        }
        #endregion

        protected void CheckCollisions()
        {
            var offset = 0.01f;

            #region Collision: Bottom
            var colliderBottomLeft = new Vector2( _collider.bounds.min.x, _collider.bounds.min.y - offset );
            var colliderBottomRight = new Vector2( _collider.bounds.min.x + _collider.bounds.size.x, _collider.bounds.min.y - offset );

            _collisionDirection.Bottom = Physics2D.Raycast( colliderBottomLeft, Vector2.down, CollisionHitDistance * CollisionHitDistanceBottomLength )
                || Physics2D.Raycast( colliderBottomRight, Vector2.down, CollisionHitDistance * CollisionHitDistanceBottomLength );
            #endregion

            #region Collision: Top
            var colliderTopLeft = new Vector2( _collider.bounds.min.x, _collider.bounds.max.y + offset );
            var colliderTopRight = new Vector2( _collider.bounds.max.x, _collider.bounds.max.y + offset );

            _collisionDirection.Top = Physics2D.Raycast( colliderTopLeft, Vector2.up, CollisionHitDistance * CollisionHitDistanceTopLength )
                || Physics2D.Raycast( colliderTopRight, Vector2.up, CollisionHitDistance * CollisionHitDistanceTopLength );
            #endregion

            #region Collision: Left
            var colliderLeftBottom = new Vector2( _collider.bounds.min.x - offset, _collider.bounds.min.y );
            var colliderLeftTop = new Vector2( _collider.bounds.min.x - offset, _collider.bounds.max.y );

            _collisionDirection.LeftBottom = Physics2D.Raycast( colliderLeftBottom, Vector2.left, CollisionHitDistance * CollisionHitDistanceLeftLength );
            _collisionDirection.LeftTop = Physics2D.Raycast( colliderLeftTop, Vector2.left, CollisionHitDistance * CollisionHitDistanceLeftLength );
            #endregion

            #region Collision: Right
            var colliderRightBottom = new Vector2( _collider.bounds.max.x + offset, _collider.bounds.min.y );
            var colliderRightTop = new Vector2( _collider.bounds.max.x + offset, _collider.bounds.max.y );

            _collisionDirection.RightBottom = Physics2D.Raycast( colliderRightBottom, Vector2.right, CollisionHitDistance * CollisionHitDistanceRightLength );
            _collisionDirection.RightTop = Physics2D.Raycast( colliderRightTop, Vector2.right, CollisionHitDistance * CollisionHitDistanceRightLength );
            #endregion

            #region Draw Rays
            if( DrawCollisionsEnabled )
            {
                Debug.DrawRay( colliderBottomLeft, new Vector2( 0, -CollisionHitDistance * CollisionHitDistanceBottomLength ), Color.cyan );
                Debug.DrawRay( colliderBottomRight, new Vector2( 0, -CollisionHitDistance * CollisionHitDistanceBottomLength ), Color.cyan );
                Debug.DrawRay( colliderTopLeft, new Vector2( 0, CollisionHitDistance * CollisionHitDistanceTopLength ), Color.cyan );
                Debug.DrawRay( colliderTopRight, new Vector2( 0, CollisionHitDistance * CollisionHitDistanceTopLength ), Color.cyan );
                Debug.DrawRay( colliderLeftBottom, new Vector2( -CollisionHitDistance * CollisionHitDistanceLeftLength, 0 ), Color.cyan );
                Debug.DrawRay( colliderLeftTop, new Vector2( -CollisionHitDistance * CollisionHitDistanceLeftLength, 0 ), Color.cyan );
                Debug.DrawRay( colliderRightBottom, new Vector2( CollisionHitDistance * CollisionHitDistanceRightLength, 0 ), Color.cyan );
                Debug.DrawRay( colliderRightTop, new Vector2( CollisionHitDistance * CollisionHitDistanceRightLength, 0 ), Color.cyan );
            }
            #endregion
        }

        #region Other
        protected void SetVelocity( float? x = null, float? y = null )
        {
            var newX = x ?? _rigidbody.velocity.x;
            var newY = y ?? _rigidbody.velocity.y;
            _rigidbody.velocity.Set( newX, newY );
        }

        protected bool IsState( ExecuteState state )
        {
            return _executeState == state;
        }
        #endregion

        #region Properties
        protected bool IsOnGround { get { return _rigidbody.velocity.y == 0.0f && _collisionDirection.Bottom; } }
        protected bool IsFalling { get { return _rigidbody.velocity.y < 0.0f; } }
        #endregion
    }
}