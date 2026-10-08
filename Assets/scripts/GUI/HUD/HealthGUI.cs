using MyGame.damage;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyGame.UI
{
    public class HealthGUI : MonoBehaviour
    {
        [SerializeField]
        private DamageManager healthSource;

        [SerializeField]
        private string healthBarName = "HealthBar";

        [SerializeField]
        private string healthTextName = "HealthText";

        private UIDocument _document;
        private ProgressBar _healthBar;
        private Label _healthLabel;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null)
            {
                Debug.LogError("[HealthGUI] UIDocument is required on the same GameObject.");
                return;
            }

            VisualElement root = _document.rootVisualElement;
            _healthBar = root.Q<ProgressBar>(healthBarName);
            _healthLabel = root.Q<Label>(healthTextName);

            if (healthSource == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    healthSource = player.GetComponentInChildren<DamageManager>();
                }
            }
        }

        private void Update()
        {
            if (healthSource == null)
            {
                return;
            }

            if (_healthBar == null)
            {
                return;
            }

            float maxHealth = 100f;
            float currentHealth = Mathf.Clamp(healthSource.CurrentHealth, 0f, maxHealth);
            _healthBar.highValue = maxHealth;
            _healthBar.value = currentHealth;

            if (_healthLabel != null)
            {
                float percent = (currentHealth / Mathf.Max(maxHealth, 0.0001f)) * 100f;
                _healthLabel.text = $"{currentHealth:0} / {maxHealth:0} ({percent:0}%)";
            }
        }
    }
}
