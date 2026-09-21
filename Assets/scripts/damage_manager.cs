using MyGame.Damage_interface;
using UnityEngine;

namespace MyGame.damage
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float health = 100f;
        public float CurrentHealth => health;

        public void TakeDamage(float amount)
        {
            health -= amount;
        }
    }
}
