using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Defines complete behaviour of the player
    /// </summary>
    public class PlayerAvatar : Entity
    {
        #region Members
        public float JumpForce = PlayerAvatarDefaultValues.JumpForce;
        public float HopForce = PlayerAvatarDefaultValues.HopForce;
        public float DashForce = PlayerAvatarDefaultValues.DashForce;
        public float DashDuration = PlayerAvatarDefaultValues.DashDuration;
        public float DashRecoveryDuration = PlayerAvatarDefaultValues.DashRecoveryDuration;
        public int MaxDashes = PlayerAvatarDefaultValues.MaxDashes;
        public float CrouchColliderScale = PlayerAvatarDefaultValues.CrouchColliderScale;
        public Transform CrouchPivot;

        private bool _controlsEnabled;
        private TimerManager _timerManager;
        private Vector2 _direction;
        private Vector2 _shotDirection;
        private int _dashesLeft;
        private Vector2 _storeVelocity;
        private Vector3 _storeColliderPosition;
        #endregion

        #region Behaviour
        private void Start()
        {
            _timerManager = new TimerManager();
            _isDirectionRight = true;
            _controlsEnabled = true;
            _dashesLeft = MaxDashes;
            SetState( Idle );
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

        /// <summary>
        /// Handle all transformations applied to and/or by <see cref="PlayerAvatar"/> physics
        /// </summary>
        private void HandlePhysics()
        {
            if( _collisionDirection.Bottom && IsOnGround )
            {
                _dashesLeft = MaxDashes;
            }
        }

        /// <summary>
        /// Handle all transformations applied to <see cref="PlayerAvatar"/> graphics
        /// </summary>
        private void HandleGraphics()
        {
            // Flip graphics

            if( _isDirectionRight && _rigidbody.velocity.x < 0
                || !_isDirectionRight && _rigidbody.velocity.x > 0 )
            {
                var mirrorScale = transform.localScale;
                mirrorScale.x *= -1;
                transform.localScale = mirrorScale;
                _isDirectionRight = !_isDirectionRight;
            }
        }

        /// <summary>
        /// Handle the controller's input to execute actions and/or set states
        /// </summary>
        private void HandleInputs()
        {
            //Directions
            _shotDirection.Set( Input.GetAxis( "HorizontalDirection" ), Input.GetAxis( "VerticalDirection" ) );
            _direction.Set( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) );

            if( _controlsEnabled )
            {
                // Jump
                if( IsOnGround && Input.GetButtonDown( "Jump" )
                    && ( _executeState != Hop && _executeState != Crouch && _executeState != Crawl ) )
                {
                    DoJump( JumpForce );
                }
                // Dash
                else if( Input.GetButtonDown( "Dash" ) && _dashesLeft > 0
                    && ( _executeState != Hop && _executeState != Crouch && _executeState != Crawl )
                    && _timerManager.IsOverOrNull( "DashRecovery" ) )
                {
                    SetState( Dash );
                }
            }
        }
        #endregion

        #region States
        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Idle"/> state
        /// </summary>
        private void Idle()
        {
            // On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "idle" );
                _animator.Play( "Idle" );
                _isStateEntryMode = false;
            }
            // On Update
            else if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
            {
                SetState( Walk );
            }
        }

        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Walk"/> state
        /// </summary>
        private void Walk()
        {
            // On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "walk" );
                _animator.Play( "Walk" );
                _isStateEntryMode = false;
            }
            else if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x ) && IsOnGround
                && ( _collisionDirection.RightTop && _isDirectionRight && !_collisionDirection.RightBottom
                    || _collisionDirection.LeftTop && !_isDirectionRight && !_collisionDirection.LeftBottom ) )
            {
                SetState( IdleToCrawl );
            }
            // On Update
            else if( !Utils.IsFloatEpsilonZero( _rigidbody.velocity.x )
                && ( _collisionDirection.RightBottom && _isDirectionRight && !_collisionDirection.RightTop
                    || _collisionDirection.LeftBottom && !_isDirectionRight && !_collisionDirection.LeftTop ) )
            {
                SetState( Hop );
            }
            else
            {
                var horizontalAxis = GetAxisAbsolute( "Horizontal" );
                _rigidbody.velocity = new Vector2( horizontalAxis * MoveSpeed, _rigidbody.velocity.y );

                if( horizontalAxis == 0.0f )
                {
                    SetState( Idle );
                }
            }
        }

        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Dash"/> state
        /// </summary>
        private void Dash()
        {
            // On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "dash" );
                _animator.Play( "Dash" );
                _timerManager.CreateTimer( "DashDuration", DashDuration );
                _rigidbody.gravityScale = 0;
                _rigidbody.velocity = Vector2.zero;

                if( Utils.IsFloatEpsilonZero( Input.GetAxis( "Horizontal" ) )
                    && Utils.IsFloatEpsilonZero( Input.GetAxis( "Vertical" ) ) )
                {
                    _rigidbody.AddForce( ( _isDirectionRight ? Vector2.right : Vector2.left ) * DashForce );
                }
                else
                {
                    _rigidbody.AddForce( _direction.normalized * DashForce );
                }
                _isStateEntryMode = false;
            }
            // On Update
            else if( _timerManager.IsOverOrNull( "DashDuration" ) )
            {
                --_dashesLeft;
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.gravityScale = _gravityScaleDefault;
                _timerManager.CreateTimer( "DashRecovery", DashRecoveryDuration );
                SetState( Idle );
            }
        }

        private void DashFreeze()
        {
            // On Entry
            if( _isStateEntryMode )
            {
                _isStateEntryMode = false;
            }
            // On Update
            else
            {

            }
        }

        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Hop"/> state
        /// </summary>
        private void Hop()
        {
            // On Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "hop" );
                _animator.Play( "Hop" );
                var horizontalAxis = GetAxisAbsolute( "Horizontal" );
                _rigidbody.velocity = new Vector2( horizontalAxis * MoveSpeed, 0.0f );
                _storeVelocity = _rigidbody.velocity;
                DoJump( HopForce );
                _isStateEntryMode = false;
                _controlsEnabled = false;
            }
            // On Update
            else if( _collisionDirection.Bottom && IsOnGround )
            {
                _controlsEnabled = true;
                if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
                {
                    if( _collisionDirection.RightBottom && _isDirectionRight && !_collisionDirection.RightTop
                    || _collisionDirection.LeftBottom && !_isDirectionRight && !_collisionDirection.LeftTop )
                    {
                        SetState( Hop );
                    }
                    else
                    {
                        SetState( Walk );
                    }
                }
                else
                {
                    _rigidbody.velocity = Vector2.zero;
                    SetState( Idle );
                }
            }
            else
            {
                if( !_collisionDirection.RightTop && !_collisionDirection.LeftTop )
                {
                    _rigidbody.velocity = new Vector2( _storeVelocity.x, _rigidbody.velocity.y );
                }
            }
        }

        /// <summary>
        /// Defines Entry actions for <see cref="Idle"/> to <see cref="Crawl"/> state
        /// </summary>
        private void IdleToCrawl()
        {
            // On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "idle to crawl" );
                _animator.Play( "Crawl" );
                _collider.transform.localScale = new Vector3( _collider.transform.localScale.x, _collider.transform.localScale.y * CrouchColliderScale, _collider.transform.localScale.z );
                _storeColliderPosition = _collider.transform.localPosition;
                _collider.transform.localPosition = CrouchPivot.localPosition;
                CollisionHitDistanceTopLength = 6.0f;
                _isStateEntryMode = false;
                SetState( Crawl );
            }
        }

        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Crouch"/> state
        /// </summary>
        private void Crouch()
        {
            // On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "crouch" );
                _animator.Play( "Crouch" );
                _isStateEntryMode = false;
            }
            // On Update
            else if( GetAxisAbsolute( "Horizontal" ) != 0.0f )
            {
                SetState( Crawl );
            }
        }

        /// <summary>
        /// Defines Entry, Update and Exit actions for <see cref="Crawl"/> state
        /// </summary>
        private void Crawl()
        {// On State Entry
            if( _isStateEntryMode )
            {
                Debug.Log( "crawl" );
                _animator.Play( "Crawl" );
                _isStateEntryMode = false;
            }
            // On Update
            else if( !_collisionDirection.Top )
            {
                _collider.transform.localScale = new Vector3( _collider.transform.localScale.x, _collider.transform.localScale.y / CrouchColliderScale, _collider.transform.localScale.z );
                _collider.transform.localPosition = _storeColliderPosition;
                CollisionHitDistanceTopLength = 1.0f;
                SetState( Walk );
            }
            else
            {
                float horizontalAxis = GetAxisAbsolute( "Horizontal" );
                _rigidbody.velocity = new Vector2( horizontalAxis * MoveSpeed, _rigidbody.velocity.y );

                if( horizontalAxis == 0.0f )
                {
                    SetState( Crouch );
                }
            }
        }
        #endregion

        #region Other

        /// <summary>
        /// Get absolute float value for a given axis defined by <paramref name="axisName"/>
        /// </summary>
        /// <param name="axisName">The name of the axis in Unity (Horizontal | Vertical)</param>
        private float GetAxisAbsolute( string axisName )
        {
            var axis = Input.GetAxis( axisName );
            if( axis > 0 )
                axis = 1.0f;
            else if( axis < 0 )
                axis = -1.0f;
            else
                axis = 0.0f;

            return axis;
        }

        /// <summary>
        /// Execute an <see cref="PlayerAvatar"/> jump with a given <paramref name="force"/>
        /// </summary>
        /// <param name="force">New magnitude of <see cref="Vector2.up"/></param>
        private void DoJump( float force )
        {
            _rigidbody.AddForce( Vector2.up * force );
        }
        #endregion
    }
}