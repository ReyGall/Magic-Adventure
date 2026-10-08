using MyGame.manaControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyGame.UI
{
    public class ManaGUI : MonoBehaviour
    {
        [SerializeField]
        private Mana manaSource;

        [SerializeField]
        private string manaBarName = "ManaBar";

        [SerializeField]
        private string manaTextName = "ManaText";

        private UIDocument _document;
        private ProgressBar _manaBar;
        private Label _manaLabel;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null)
            {
                Debug.LogError("[ManaGUI] UIDocument is required on the same GameObject.");
                return;
            }

            VisualElement root = _document.rootVisualElement;
            _manaBar = root.Q<ProgressBar>(manaBarName);
            _manaLabel = root.Q<Label>(manaTextName);

            if (manaSource == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    manaSource = player.GetComponentInChildren<Mana>();
                }
            }
        }

        private void Update()
        {
            if (manaSource == null)
            {
                return;
            }

            if (_manaBar == null)
            {
                return;
            }

            float maxMana = 100f;
            float currentMana = Mathf.Clamp(manaSource.CheckMana(), 0f, maxMana);
            _manaBar.highValue = maxMana;
            _manaBar.value = currentMana;

            if (_manaLabel != null)
            {
                float percent = (currentMana / Mathf.Max(maxMana, 0.0001f)) * 100f;
                _manaLabel.text = $"{currentMana:0} / {maxMana:0} ({percent:0}%)";
            }
        }
    }
}
