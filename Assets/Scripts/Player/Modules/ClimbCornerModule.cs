using Common;
using Runners;
using UnityEngine;

namespace Player
{

    public class ClimbCornerModule : MonoBehaviour
    {

        [SerializeField]
        private float height = 0.1f;
        [SerializeField]
        private float distance = 0.075f;
        [SerializeField]
        private float climbSpeed = 5f;
        [SerializeField]
        private float walkSpeed = 5f;

        public bool IsClimbing => cornerState != CornerState.Nothing;

        private enum CornerState
        {
            Nothing,
            Climbing,
            Walking
        }

        private CornerState cornerState = CornerState.Nothing;

        public bool CheckForCorner(CollisionState collision, bool facingRight, LayerMask collisionLayers)
        {
            if (IsClimbing
                || collision.Ceil)
                return false;

            Bounds bounds = collision.collider.bounds;

            Vector2 startPoint = bounds.center;
            float totalHeight = bounds.size.y * 0.5f + height;

            if (facingRight)
                startPoint.x += bounds.extents.x + distance;
            else
                startPoint.x -= (bounds.extents.x + distance);
            startPoint.y += bounds.extents.y + height;

            float cornerDistance = PhysicsUtils.GetDistance(startPoint, Vector2.down, totalHeight, collisionLayers);
            bool cornerFound = cornerDistance > 0f
                && cornerDistance < totalHeight;

            if (cornerFound)
            {
                Debug.DrawLine(startPoint, startPoint + Vector2.down * totalHeight, Color.yellow, 2f);
            }

            return cornerFound;
        }

        public void StartClimbing(Rigidbody2D rigidbody)
        {
            if (IsClimbing)
                return;

            cornerState = CornerState.Climbing;
            rigidbody.velocity = Vector2.zero;
        }

        public void StopClimbing()
        {
            if (!IsClimbing)
                return;

            CornerClimbed();
        }

        public void UpdateClimbing(Rigidbody2D rigidbody, CollisionState collision, bool facingRight, LayerMask collisionLayers)
        {
            if (!IsClimbing)
                return;

            Bounds bounds = collision.collider.bounds;
            if (cornerState == CornerState.Climbing)
            {
                Vector2 startPoint = bounds.center;
                float totalHeight = bounds.size.y + height * 2f;
                float thresholdHeight = bounds.size.y + height * 1.5f;

                if (facingRight)
                    startPoint.x += bounds.extents.x + distance;
                else
                    startPoint.x -= (bounds.extents.x + distance);
                startPoint.y += bounds.extents.y + height;

                float cornerDistance = PhysicsUtils.GetDistance(startPoint, Vector2.down, totalHeight, collisionLayers);
                Debug.DrawLine(startPoint, startPoint + Vector2.down * totalHeight, Color.magenta, 0.2f);

                rigidbody.velocity = Vector2.up * climbSpeed;
                if (cornerDistance > thresholdHeight)
                {
                    cornerState = CornerState.Walking;
                }
                else if (collision.Ceil)
                {
                    StopClimbing();
                }
            }

            if (cornerState == CornerState.Walking)
            {
                Vector2 startPoint = bounds.center;
                float totalWidth = bounds.extents.x + distance;
                float thresholdWidth = bounds.extents.x;

                if (facingRight)
                    startPoint.x -= totalWidth;
                else
                    startPoint.x += totalWidth;
                startPoint.y -= (bounds.extents.y + distance);

                Vector2 direction = facingRight
                    ? Vector2.right
                    : Vector2.left;
                float floorDistance = PhysicsUtils.GetDistance(startPoint, direction, totalWidth, collisionLayers);
                Debug.DrawLine(startPoint, startPoint + direction * totalWidth, Color.cyan, 0.2f);

                rigidbody.velocity = direction * walkSpeed;
                if (floorDistance > thresholdWidth)
                {
                    CornerClimbed();
                }
            }
        }

        private void CornerClimbed()
        {
            cornerState = CornerState.Nothing;
        }

    }

}