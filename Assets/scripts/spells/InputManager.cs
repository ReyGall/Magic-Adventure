using MyGame.SpellsCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mygame.InputManager
{
    public class InputManager : MonoBehaviour
    {
        private InputActions _inputActions;

        [System.Serializable]
        public struct AbilitySlot
        {
            public string actionName; // link to ability №n
            public SpellBase spellComponent; // link to spell on player
        }

        [SerializeField]
        private AbilitySlot[] abilitySlots;

        private void Awake()
        {
            //initialization
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            // turning on input
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            // turning off input
            _inputActions.Disable();
        }

        private void Update()
        {
            foreach (var slot in abilitySlots)
            {
                if (string.IsNullOrEmpty(slot.actionName))
                    continue;
                if (slot.spellComponent == null)
                    continue;

                var action = _inputActions.FindAction(slot.actionName);
                if (action == null)
                    continue;

                // 1. if just pressed in this frame
                if (action.WasPressedThisFrame())
                {
                    slot.spellComponent.Cast();
                }

                // 2. if holds
                if (action.IsPressed())
                {
                    slot.spellComponent.HoldCast();
                }

                // 3. if not pressed
                if (action.WasReleasedThisFrame())
                {
                    slot.spellComponent.StopCast();
                }
            }
        }
    }
}
