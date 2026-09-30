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

                // 1. if key pressed
                if (action.WasPressedThisFrame())
                {
                    slot.spellComponent.Cast(); // for instant
                    slot.spellComponent.StartCharge(); // for charge
                }

                // 2. if key being holded
                if (action.IsPressed())
                {
                    slot.spellComponent.HoldCharge(); // for charging spells
                    slot.spellComponent.HoldCast(); // for holding spells
                }

                // 3. if key released
                if (action.WasReleasedThisFrame())
                {
                    slot.spellComponent.ReleaseCharge(); // launch charged spell
                    slot.spellComponent.StopCast(); // turning off holding spell
                }
            }
        }
    }
}
