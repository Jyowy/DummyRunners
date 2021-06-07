using System.Collections.Generic;
using Common;
using Game;
using Player;
using Player.Skills;
using Runners;
using Stadiums.ExtraFases;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

namespace Stadiums.Runners
{

    [System.Serializable]
    public struct RunnerState
    {
        public CollisionState collision;
        public bool energyAvailable;
        public float energyRatio;
        public bool pulseAvailable;
        public bool shieldAvailable;
        public bool jumpAvailable;
    }

    [System.Serializable]
    public struct RunnerInput
    {
        public bool faceRight;
        public bool move;
        public Vector2 direction;
        public bool jump;
        public bool activateTurbo;
        public bool pulseActivation;
        public bool shieldActivation;
    }

    public struct RunnerOutput
    {
        public bool facingRight;
        public bool moved;
        public bool jumped;
        public bool onAir;
        public bool isClimbingCorner;
        public bool turboActive;
        public bool pulseActive;
        public bool shieldActive;
    }

    public class Runner : MonoBehaviour
    {

        [SerializeField]
        private RunnerData data = null;

        [SerializeField]
        private Transform turnableRoot = null;
        [SerializeField]
        private new Rigidbody2D rigidbody = null;
        [SerializeField]
        private CollisionDetector collisionDetector = null;
        [SerializeField]
        private Transform ballSlot = null;

        [SerializeField]
        private WalkModule walk = null;
        [SerializeField]
        private JumpModule jump = null;
        [SerializeField]
        private AirModule air = null;
        [SerializeField]
        private ClimbCornerModule climbCorner = null;
        [SerializeField]
        private TurboModule turbo = null;
        [SerializeField]
        private PulseModule pulse = null;
        [SerializeField]
        private ShieldModule shield = null;

        [SerializeField]
        private List<SpriteRenderer> renderers = null;
        [SerializeField]
        private TrailRenderer turboTrail = null;

        public string GetRunnerName() => data.runnnerName;

        private bool facingRight = true;

        private LayerMask scenaryLayerMask = 0;
        private LayerMask runnersLayerMask = 0;

        private readonly float stunSafeCooldownDuration = 0.125f;
        private TimedAction stunSafeCooldown;
        private TimedAction stun;

        private Ball ball = null;
        private int team = -1;

        public EnergyBattery GetEnergyBattery()
            => turbo.GetEnergyBattery();

        public PulseModule GetPulseModule()
            => pulse;
        public ShieldModule GetShieldModule()
            => shield;

        public Collider2D GetCollider() => collisionDetector.Collider;

        public bool IsStunned => !stun.Completed;

        public bool IsPulseActive => pulse.IsActive;
        public bool IsPointInReachOfPulse(Vector2 point)
        {
            return math.distancesq((Vector2)transform.position, point)
                <= data.pulseRadius * data.pulseRadius;
        }
        public bool IsDistanceInReachOfPulse(float distance)
            => distance <= data.pulseRadius;

        public void SetTeam(int team)
            => this.team = team;
        public int Team => team;

        private void Awake()
        {
            scenaryLayerMask = GameManager.GetScenaryLayerMark();
            runnersLayerMask = GameManager.GetRunnersLayerMark();

            SetData(data);

            stunSafeCooldown.Set(stunSafeCooldownDuration);
            stunSafeCooldown.Finish();

            SetColors();
        }

        public void SetColors()
        {
            Debug.LogFormat("SetColors to {0}", name);
            Color baseColor = GameUtils.GetRandomColor();
            Color secondaryColor = GameUtils.GetRandomColor();
            Color accesoryColor = GameUtils.GetRandomColor();
            Color glassColor = GameUtils.GetRandomColor();

            renderers.ForEach((renderer)
                =>
            {
                renderer.material.SetColor("_BaseColor", baseColor);
                renderer.material.SetColor("_SecondaryColor", secondaryColor);
                renderer.material.SetColor("_AccesoryColor", accesoryColor);
                renderer.material.SetColor("_GlassColor", glassColor);
            });

            secondaryColor *= 1.5f;
            secondaryColor.a = 1f;
            turboTrail.startColor = secondaryColor;
            secondaryColor.a = 0f;
            turboTrail.endColor = secondaryColor;
        }

        public void SetData(RunnerData data)
        {
            this.data = data;

            rigidbody.gravityScale = data.gravity;
            rigidbody.drag = data.friction;

            walk.SetBaseSpeed(data.baseSpeed);
            walk.SetStartTime(data.startWalkTime);

            jump.SetPower(data.jumpPower);
            air.SetHorizontalSpeed(data.onAirHorizontalSpeed);

            turbo.SetSpeedSpeed(data.turboMultiplier);

            pulse.SetPower(data.pulsePower);
            pulse.SetRadius(data.pulseRadius);
            pulse.SetStunDuration(data.pulseStunDuration);
            pulse.SetCooldownDuration(data.pulseCooldown);

            shield.SetDuration(data.shieldDuration);
            shield.SetCooldownDuration(data.shieldCooldown);
        }

        public void SetPosition(SpawnPoint spawnPoint)
        {
            transform.position = spawnPoint.transform.position;
            FaceRight(spawnPoint.FaceRight);
        }

        public void Restart()
        {
            rigidbody.gravityScale = data.gravity;
            rigidbody.drag = data.friction;
            rigidbody.velocity = Vector2.zero;

            stun.Finish();

            turbo.Restart();
            pulse.Restart();
            shield.Restart();
        }

        public void GetState(float dt, out RunnerState state)
        {
            state.collision = collisionDetector.GetCollision(dt);
            var battery = turbo.GetEnergyBattery();
            float energy = battery.GetCurrentValue();
            state.energyAvailable = energy >= battery.GetMinValue();
            state.energyRatio = energy / battery.GetMaxValue();

            state.jumpAvailable = jump.CanJump(state.collision);
            state.pulseAvailable = pulse.CanActivate();
            state.shieldAvailable = shield.CanActivate();
        }

        public void UpdateState(float dt, ref RunnerState state, ref RunnerInput input, out RunnerOutput output)
        {
            output.facingRight = facingRight;
            output.moved = false;
            output.jumped = false;
            output.isClimbingCorner = false;
            output.turboActive = turbo.IsActive;
            output.pulseActive = pulse.IsActive;
            output.shieldActive = shield.IsActive;

            var collision = state.collision;

            bool onFloor = collision.StoredFloor;
            output.onAir = !onFloor;

            CheckLanding(collision);

            if (!stunSafeCooldown.Completed)
            {
                stunSafeCooldown.Update(dt);
            }
            if (!stun.Completed)
            {
                stun.Update(dt);
                if (stun.Completed)
                {
                    StunFinished();
                }
                return;
            }

            CheckTurbo(input.activateTurbo);
            output.turboActive = turbo.IsActive;
            turboTrail.emitting = turbo.IsActive;

            if (input.shieldActivation)
            {
                ActivateShield();
                output.shieldActive = shield.IsActive;
            }

            if (input.pulseActivation)
            {
                ActivatePulse(input.direction);
                output.pulseActive = pulse.IsActive;
            }

            output.jumped = CheckJump(input, collision);
            if (jump.JumpRecentlyUsed)
                return;

            CheckCorner(collision);
            if (climbCorner.IsClimbing)
            {
                climbCorner.UpdateClimbing(rigidbody, collision, facingRight, scenaryLayerMask);
                output.isClimbingCorner = climbCorner.IsClimbing;
                return;
            }

            FaceRight(input.faceRight);
            output.facingRight = facingRight;

            bool move = input.move;
            if (move)
            {
                float turboMultiplier = turbo.GetSpeedTurboMultiplier();
                if (onFloor)
                {
                    Vector2 floorDirection = collision.floor.direction;
                    walk.Walk(dt, rigidbody, facingRight, floorDirection, turboMultiplier);
                }
                else
                {
                    if ((facingRight && collision.right.Contact)
                        || (!facingRight && collision.left.Contact))
                    {
                        move = false;
                    }
                    else
                    {
                        air.OnAir(rigidbody, facingRight, turboMultiplier);
                    }

                }
            }

            output.moved = move;
        }

        private void FaceRight(bool faceRight)
        {
            this.facingRight = faceRight;

            Vector2 localScale = turnableRoot.localScale;
            localScale.x = math.abs(localScale.x)
                * math.sign(faceRight ? 1f : -1f);
            turnableRoot.localScale = localScale;
        }

        private bool CheckJump(RunnerInput input, CollisionState collision)
        {
            bool jumped = false;

            if (input.jump
                && jump.CanJump(collision))
            {
                if (climbCorner.IsClimbing)
                    climbCorner.StopClimbing();

                jump.Jump(rigidbody, collision);
                jumped = true;

                if (math.abs(rigidbody.velocity.x) > 0.125f)
                {
                    FaceRight(rigidbody.velocity.x > 0f);
                }
            }

            return jumped;
        }

        private void CheckLanding(CollisionState collision)
        {
            if (!collision.floor.PrevContact
                   && collision.floor.Contact)
            {
                float xspeed = math.abs(rigidbody.velocity.x)
                    * (facingRight ? 1f : -1f);
                xspeed = math.max(xspeed, data.baseSpeed * 0.5f);
                rigidbody.velocity.Set(xspeed, 0f);
            }
        }

        private void CheckCorner(CollisionState collision)
        {
            if (climbCorner.CheckForCorner(collision, facingRight, scenaryLayerMask))
            {
                climbCorner.StartClimbing(rigidbody);
            }
        }

        private void CheckTurbo(bool activateTurbo)
        {
            if (activateTurbo
                && !turbo.IsActive)
            {
                turbo.Activate();
            }
            else if (!activateTurbo
                && turbo.IsActive)
            {
                turbo.Deactivate();
            }
        }

        private void ActivatePulse(Vector2 direction)
        {
            if (pulse.CanActivate()
                && !shield.IsActive)
            {
                pulse.Activate(runnersLayerMask);
                if (ball != null)
                {
                    Debug.LogFormat("Shoot ball in direction {0}", direction);
                    ball.Shoot(direction, data.pulsePower);
                }
            }
        }

        public bool ReceivePulse(Vector2 worldPosition, float power, float stunDuration)
        {
            if (shield.IsActive)
                return false;

            Vector2 direction = (rigidbody.position - worldPosition).normalized * power;
            if (ball != null)
            {
                ball.Shoot(direction, power);
            }

            if (!stunSafeCooldown.Completed)
                return true;

            rigidbody.velocity = direction * power;

            rigidbody.gravityScale = 0f;
            rigidbody.drag = 0f;

            float finalDuration = stunDuration * math.clamp(1f - data.pulseStunReduction, 0f, 1f);
            Stun(finalDuration);

            return true;
        }

        public void Stun(float duration)
        {
            stun.Set(duration);
            turbo.Deactivate();
        }

        private void StunFinished()
        {
            rigidbody.gravityScale = data.gravity;
            rigidbody.drag = data.friction;
        }

        private void ActivateShield()
        {
            if (shield.CanActivate())
            {
                shield.Activate();
            }
        }

        public bool HasTheBall()
            => ball != null;

        public void CatchBall(Ball ball, PositionConstraint positionConstraint)
        {
            this.ball = ball;
            ConstraintSource constraint = new ConstraintSource()
            {
                weight = 1f,
                sourceTransform = ballSlot.transform
            };
            positionConstraint.SetSource(0, constraint);
        }

        public void LoseBall()
        {
            ball = null;
        }

    }

}