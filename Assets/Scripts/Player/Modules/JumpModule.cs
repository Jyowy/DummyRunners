using Common;
using UnityEngine;

namespace Runners
{

    public class JumpModule : MonoBehaviour
    {

        [SerializeField]
        private float power = 7.5f;
        [SerializeField]
        private float cooldownDuration = 0.125f;

        private TimedAction cooldown;

        public bool JumpRecentlyUsed => !cooldown.Completed;

        public void SetPower(float power)
            => this.power = power;

        private void Start()
        {
            cooldown.Set(cooldownDuration);
            cooldown.Finish();
        }

        public bool CanJump(CollisionState collision)
        {
            return !JumpRecentlyUsed
                && collision.StoredFloorOrWalls;
        }

        public void Jump(Rigidbody2D rigidbody, CollisionState collision)
        {
            if (!CanJump(collision))
                return;

            bool floorJump = collision.floor.StoredContact;

            if (floorJump)
            {
                Vector2 velocity = rigidbody.velocity;
                velocity.y = power;
                rigidbody.velocity = velocity;
            }
            else
            {
                Vector2 direction = collision.left.StoredContact
                    ? Vector2.right
                    : Vector2.left;
                direction += Vector2.up;
                rigidbody.velocity = direction * power;
            }

            cooldown.Restart();
        }

        private void Update()
        {
            if (!cooldown.Completed)
            {
                cooldown.Update(Time.deltaTime);
            }
        }

    }

}