using Common;
using Game;
using Stadiums.Runners;
using UnityEngine;
using UnityEngine.Animations;

namespace Stadiums.ExtraFases
{

    public class Ball : MonoBehaviour
    {

        [SerializeField]
        private new Rigidbody2D rigidbody = null;
        [SerializeField]
        private new Collider2D collider = null;
        [SerializeField]
        private PositionConstraint positionConstraint = null;
        [SerializeField]
        private float uncatchableDuration = 0.25f;
        [SerializeField]
        private float shootPower = 10f;
        [SerializeField]
        private TriggerArea trigger = null;

        [SerializeField]
        private ParticleSystem goalEffect = null;

        private bool active = false;

        private Runner owner = null;
        private TimedAction uncatchable;

        public Collider2D GetCollider()
            => collider;

        public Vector2 GetVelocity()
            => rigidbody.velocity;

        public int GetTeam() => owner != null
            ? owner.Team
            : -1;


        private void Awake()
        {
            uncatchable.Set(uncatchableDuration);
            uncatchable.Finish();
            positionConstraint.constraintActive = false;
            rigidbody.simulated = false;
        }

        public void Activate()
        {
            active = true;
            rigidbody.simulated = true;
            trigger.Activate(OnTriggerEnter2D, null);
        }

        public void Deactivate()
        {
            Release();
            active = false;
            rigidbody.simulated = false;
            trigger.Deactivate();
        }

        public void SetPosition(SpawnPoint point)
        {
            rigidbody.transform.position = point.transform.position;
        }

        public void Shoot(Vector2 direction, float power)
        {
            if (owner != null)
            {
                owner.LoseBall();
                owner = null;
            }

            uncatchable.Set(uncatchableDuration);
            Release();
            rigidbody.velocity = direction * power * shootPower;
            rigidbody.simulated = true;
        }

        public void Release()
        {
            positionConstraint.constraintActive = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!active)
                return;

            if (!uncatchable.Completed
                || owner != null
                || collision.transform.parent == null
                || !collision.transform.parent.TryGetComponent(out Runner runner))
                return;

            owner = runner;
            rigidbody.velocity = Vector2.zero;
            runner.CatchBall(this, positionConstraint);
            positionConstraint.constraintActive = true;
        }

        private void FixedUpdate()
        {
            if (uncatchable.Completed)
                return;

            float dt = Time.fixedDeltaTime;
            uncatchable.Update(dt);
        }

        public void Goal()
        {
            goalEffect.Play();
            AudioManager.PlayGoal();
            active = false;
        }

    }

}