using Unity.Mathematics;
using UnityEngine;

namespace Player
{

    public class AirModule : MonoBehaviour
    {

        [SerializeField]
        private float horizontalSpeed = 5f;

        public void SetHorizontalSpeed(float horizontalSpeed)
            => this.horizontalSpeed = horizontalSpeed;

        public void OnAir(Rigidbody2D rigidbody, bool facingRight, float multiplier)
        {
            float directionSign = facingRight
                ? 1f
                : -1f;

            float currentSpeed = rigidbody.velocity.x * directionSign;
            float xspeed = horizontalSpeed * multiplier;
            xspeed = math.max(xspeed, currentSpeed);

            Vector2 velocity = rigidbody.velocity;
            velocity.x = xspeed * directionSign;
            rigidbody.velocity = velocity;
        }

    }

}