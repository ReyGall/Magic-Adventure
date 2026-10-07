using MyGame.Damage_interface;
using UnityEngine;

namespace MyGame.damage
{
    public class DamageManager : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float health = 100f;

        [SerializeField]
        private bool destroyOnDeath = true;

        [SerializeField]
        private string entityName;

        public float CurrentHealth => health;

        private string EntityName => string.IsNullOrEmpty(entityName) ? gameObject.name : entityName;

        public void TakeDamage(float amount)
        {
            health -= amount;
            Debug.Log($"[{EntityName}] Took damage: {amount}. Health left: {health}");

            if (health <= 0f)
            {
                health = 0f;
                Debug.Log($"[{EntityName}] died.");

                if (destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public class Enemy : DamageManager
    {
    }
}
