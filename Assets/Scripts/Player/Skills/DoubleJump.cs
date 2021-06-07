using Runners;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Skills
{

    public class DoubleJump : Skill
    {

        public override SkillEnum GetSkillType()
            => SkillEnum.DoubleJump;

        [SerializeField]
        private float power = 15f;

        private bool alreadyUsed = false;

        public bool CanActivate(CollisionState collision)
        {
            return IsAvailable()
                && !alreadyUsed
                && !collision.floor.Contact
                && HasEnoughEnergy();
        }

        public void Activate(Rigidbody2D rigidbody, float x, CollisionState collision)
        {
            if (!CanActivate(collision))
                return;

            alreadyUsed = true;
            ConsumeEnergy();

            Vector2 direction = new Vector2(x, 1f).normalized;
            Vector2 newVelocity = direction * power;
            rigidbody.velocity = newVelocity;
        }

        public void RestoreUse() => alreadyUsed = false;

    }

}