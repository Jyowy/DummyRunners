using Common;
using Game;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{

    public struct ButtonInfo
    {
        public bool Pressed { get; private set; }
        public bool Released { get; private set; }
        public bool Stored => !store.Completed;

        private TimedAction store;

        public void Initialize(float storeTime)
        {
            store.Set(storeTime);
            store.Finish();
        }

        public void Update(float dt, bool pressed)
        {
            Released = Pressed && !pressed;
            Pressed = pressed;
            if (!pressed)
            {
                store.Update(dt);
            }
            else
            {
                store.Restart();
            }
        }

        public void Use()
        {
            Pressed = false;
            Released = false;
            store.Finish();
        }

        public void Clear()
        {
            Use();
        }

    }

    public class PlayerInputInfo
    {
        public float x;
        public float y;
        public ButtonInfo jump;
        public ButtonInfo skill;
        public ButtonInfo interact;
        public ButtonInfo impulse;
        public ButtonInfo start;
        public ButtonInfo menu;
    }

    public class PlayerInputController : MonoBehaviour
    {

        private InputAction move = null;
        private InputAction jump = null;
        private InputAction skill = null;
        private InputAction interact = null;
        private InputAction impulse = null;
        private InputAction start = null;
        private InputAction menu = null;

        [SerializeField]
        private float minMovement = 0.125f;
        [SerializeField]
        private float storeButtonTime = 0.2f;

        private readonly PlayerInputInfo inputInfo = new PlayerInputInfo();

        private bool jumpPressed = false;
        private bool skillPressed = false;
        private bool interactPressed = false;
        private bool impulsePressed = false;
        private bool startPressed = false;
        private bool menuPressed = false;

        private void Awake()
        {
            var playerInputMap = GameManager.GetPlayerActionMap();
            move = playerInputMap.FindAction("Move");
            jump = playerInputMap.FindAction("Jump");
            skill = playerInputMap.FindAction("Skill");
            interact = playerInputMap.FindAction("Interact");
            impulse = playerInputMap.FindAction("Impulse");
            start = playerInputMap.FindAction("Start");
            menu = playerInputMap.FindAction("Menu");
        }

        private void OnEnable()
        {
            inputInfo.jump.Initialize(storeButtonTime);
            inputInfo.skill.Initialize(storeButtonTime);
            inputInfo.interact.Initialize(storeButtonTime);
            inputInfo.impulse.Initialize(storeButtonTime);
            inputInfo.start.Initialize(storeButtonTime);
            inputInfo.menu.Initialize(storeButtonTime);

            jump.performed += OnJump;
            skill.performed += OnSkill;
            interact.performed += OnInteract;
            impulse.performed += OnImpulse;
            start.performed += OnStart;
            menu.performed += OnMenu;
        }

        private void OnDisable()
        {
            inputInfo.jump.Clear();
            inputInfo.skill.Clear();
            inputInfo.interact.Clear();
            inputInfo.impulse.Clear();
            inputInfo.start.Clear();
            inputInfo.menu.Clear();

            jump.performed -= OnJump;
            skill.performed -= OnSkill;
            interact.performed -= OnInteract;
            impulse.performed -= OnImpulse;
            start.performed -= OnStart;
            menu.performed -= OnMenu;
        }

        public PlayerInputInfo GetInputInfo(float dt)
        {
            Vector2 movement = move.ReadValue<Vector2>();
            if (math.abs(movement.x) < minMovement)
                movement.x = 0f;
            if (math.abs(movement.y) < minMovement)
                movement.y = 0f;
            inputInfo.x = movement.x;
            inputInfo.y = movement.y;

            inputInfo.jump.Update(dt, jumpPressed);
            inputInfo.skill.Update(dt, skillPressed);
            inputInfo.interact.Update(dt, interactPressed);
            inputInfo.impulse.Update(dt, impulsePressed);
            inputInfo.start.Update(dt, startPressed);
            inputInfo.menu.Update(dt, menuPressed);

            jumpPressed = false;
            skillPressed = false;
            interactPressed = false;
            impulsePressed = false;
            startPressed = false;
            menuPressed = false;

            return inputInfo;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            jumpPressed = true;
        }

        private void OnSkill(InputAction.CallbackContext context)
        {
            skillPressed = true;
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            interactPressed = true;
        }

        private void OnImpulse(InputAction.CallbackContext context)
        {
            impulsePressed = true;
        }

        private void OnStart(InputAction.CallbackContext context)
        {
            startPressed = true;
        }

        private void OnMenu(InputAction.CallbackContext context)
        {
            menuPressed = true;
        }

    }

}