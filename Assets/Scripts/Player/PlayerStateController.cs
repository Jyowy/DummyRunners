using Unity.Mathematics;
using UnityEngine;

namespace Player
{

    public struct PlayerState
    {
        public bool IsFacingRight { get; private set; }
        public bool OrientationChanged { get; private set; }

        public void SetFacingRight(bool right)
        {
            if (IsFacingRight == right)
            {
                OrientationChanged = false;
                return;
            }

            IsFacingRight = right;
            OrientationChanged = true;
            Velocity = velocity;
        }

        public bool lookingUp;
        public bool lookingDown;

        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                PrevVelocity = velocity;

                velocity = value;
                Speed = math.abs(velocity.x);
                SignedSpeed = Speed;
                if ((IsFacingRight && velocity.x < 0f)
                    || (!IsFacingRight && velocity.x > 0f))
                {
                    SignedSpeed = -Speed;
                }
            }
        }

        private Vector2 velocity;
        public float Speed { get; private set; }
        public float SignedSpeed { get; private set; }
        public Vector2 PrevVelocity { get; private set; }
    }

    public class PlayerStateController
    {

        private PlayerState playerState;

        public PlayerState GetPlayerState(Rigidbody2D rigidbody, SpriteRenderer sprite, float y)
        {
            bool facingRight = !sprite.flipX;
            playerState.SetFacingRight(facingRight);

            bool lookingUp = y > 0f;
            playerState.lookingUp = lookingUp;
            bool lookingDown = y < 0f;
            playerState.lookingDown = lookingDown;

            playerState.Velocity = rigidbody.velocity;

            return playerState;
        }

    }

}