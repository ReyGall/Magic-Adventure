using UnityEngine;

namespace MyGame.Damage_interface
{
    public interface IDamageable
    {
        float CurrentHealth { get; }

        void TakeDamage(float amount);
    }
}
