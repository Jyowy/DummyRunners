using Common;
using UnityEngine;

namespace Runners
{

    public struct CollisionInfo
    {

        public bool Contact { get; private set; }
        public bool PrevContact { get; private set; }
        public bool StoredContact => !store.Completed;

        public Vector2 direction;

        private TimedAction store;

        public void Initialize(float storeTime)
        {
            store.Set(storeTime);
            store.Finish();
        }

        public void CollisionDetected(bool collision)
        {
            PrevContact = Contact;
            Contact = collision;
            if (collision)
            {
                store.Restart();
            }
        }

        public void Update(float dt)
        {
            store.Update(dt);
        }

    }

    public struct CollisionState
    {
        public Collider2D collider;

        public CollisionInfo floor;
        public CollisionInfo left;
        public CollisionInfo right;
        public CollisionInfo top;

        public bool Floor => floor.Contact;
        public bool StoredFloor => floor.StoredContact;

        public bool StoredFloorOrWalls =>
            floor.StoredContact
            || left.StoredContact
            || right.StoredContact;

        public bool FloorOrWalls =>
            floor.Contact
            || left.Contact
            || right.Contact;

        public bool Ceil => top.Contact;

        public void Initialize(Collider2D collider, float storeTime)
        {
            this.collider = collider;
            floor.Initialize(storeTime);
            left.Initialize(storeTime);
            right.Initialize(storeTime);
            top.Initialize(storeTime);
        }
    }

    public class CollisionDetector : MonoBehaviour
    {

        [SerializeField]
        private new Collider2D collider = null;
        [SerializeField]
        private LayerMask collisionLayers = 0;

        [SerializeField]
        private float horizontalLength = 0.25f;
        [SerializeField]
        private float verticalLength = 0.25f;
        [SerializeField]
        private float maxDistanceToHorizontalContact = 0.1f;
        [SerializeField]
        private float maxDistanceToVerticalContact = 0.05f;
        [SerializeField]
        private float storeContactTime = 0.1f;
        [SerializeField]
        private float widthRatio = 0.85f;
        [SerializeField]
        private float heightRatio = 0.5f;

        CollisionState collisionInfo;

        public bool floorDebug;
        public bool leftDebug;
        public bool rightDebug;
        public bool topDebug;

        public Collider2D Collider => collider;

        private void Awake()
        {
            collisionInfo.Initialize(collider, storeContactTime);
        }

        public CollisionState GetCollision(float dt)
        {
            var bounds = collider.bounds;
            Vector2 center = bounds.center;
            float hwidth = bounds.extents.x * widthRatio;
            float hheight = bounds.extents.y * heightRatio;

            float maxDistanceToVerticalContact = this.maxDistanceToVerticalContact + bounds.extents.y;
            float maxDistanceToHorizontalContact = this.maxDistanceToHorizontalContact + bounds.extents.x;

            CheckCollision(ref collisionInfo.floor, center, Vector2.down, hwidth, bounds.extents.y + verticalLength, true, collisionLayers, maxDistanceToVerticalContact, true);
            CheckCollision(ref collisionInfo.top, center, Vector2.up, hwidth, bounds.extents.y + verticalLength, false, collisionLayers, maxDistanceToVerticalContact);
            CheckCollision(ref collisionInfo.left, center, Vector2.left, hheight, bounds.extents.x + horizontalLength, false, collisionLayers, maxDistanceToHorizontalContact);
            CheckCollision(ref collisionInfo.right, center, Vector2.right, hheight, bounds.extents.x + horizontalLength, true, collisionLayers, maxDistanceToHorizontalContact);

            CollisionState playerCollisionInfo = collisionInfo;

            collisionInfo.floor.Update(dt);
            collisionInfo.top.Update(dt);
            collisionInfo.left.Update(dt);
            collisionInfo.right.Update(dt);

            floorDebug = playerCollisionInfo.floor.StoredContact;
            topDebug = playerCollisionInfo.top.StoredContact;
            leftDebug = playerCollisionInfo.left.StoredContact;
            rightDebug = playerCollisionInfo.right.StoredContact;

            return playerCollisionInfo;
        }

        private void CheckCollision(ref CollisionInfo collision, Vector2 center, Vector2 direction, float width, float length, bool clockwise, LayerMask collisionLayers, float maxDistanceToContact, bool calculateDirection = false)
        {
            Vector2 normal = GeometryUtils.GetNormal(direction, clockwise);

            Vector2 firstPoint = center - normal * width;
            Vector2 centerPoint = center;
            Vector2 lastPoint = center + normal * width;

            float firstHitDistance = PhysicsUtils.GetDistance(firstPoint, direction, length, collisionLayers);
            float centerHitDistance = PhysicsUtils.GetDistance(centerPoint, direction, length, collisionLayers);
            float lastHitDistance = PhysicsUtils.GetDistance(lastPoint, direction, length, collisionLayers);

            bool contact = firstHitDistance < maxDistanceToContact
                || centerHitDistance < maxDistanceToContact
                || lastHitDistance < maxDistanceToContact;

            collision.CollisionDetected(contact);

            if (calculateDirection)
            {
                Vector2 point1;
                if (firstHitDistance < Mathf.Infinity)
                    point1 = firstPoint + direction * firstHitDistance;
                else if (centerHitDistance < Mathf.Infinity)
                    point1 = centerPoint + direction * centerHitDistance;
                else
                    point1 = lastPoint + direction * lastHitDistance;

                Vector2 point2;
                if (lastHitDistance < Mathf.Infinity)
                    point2 = lastPoint + direction * lastHitDistance;
                else if (centerHitDistance < Mathf.Infinity)
                    point2 = centerPoint + direction * centerHitDistance;
                else
                    point2 = firstPoint + direction * firstHitDistance;

                collision.direction = (point2 - point1).normalized;
                if (collision.direction.sqrMagnitude < 0.1f)
                {
                    collision.direction = normal;
                }
            }
        }

    }

}