using MyGame.Damage_interface;
using MyGame.manaControl;
using MyGame.SpellsCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame.SpellShield
{
    public class Shield : SpellBase, IDamageable
    {
        [SerializeField]
        private GameObject _shieldVfxPrefab;

        [SerializeField]
        private Transform _shieldSpawnPoint;

        private GameObject _activeShieldInstance;

        private float _shieldCooldown = 1f;

        private float _shieldDestroyedCooldown = 8f;

        private float _shieldManaMultiplier = 0.4f;

        public float CurrentHealth => _manaRef != null ? _manaRef.CheckMana() : 0f;

        private float _nextShieldTime;

        protected override void Awake()
        {
            base.Awake();
            _manaCost = 10f;
        }

        public override void Cast()
        {
            if (_activeShieldInstance == null && Time.time >= _nextShieldTime)
            {
                if (SpellCheck())
                {
                    ActivateShield();
                }
            }
        }

        public override void HoldCast()
        {
            if (_activeShieldInstance != null)
            {
                if (!_manaRef.ManaDrain(10f * Time.deltaTime))
                {
                    Destroy(_activeShieldInstance);
                    _activeShieldInstance = null;
                    _nextShieldTime = Time.time + _shieldCooldown;
                }
            }
            else if (Time.time >= _nextShieldTime)
            {
                if (SpellCheck())
                {
                    ActivateShield();
                }
            }
        }

        public override void StopCast()
        {
            if (_activeShieldInstance != null)
            {
                Destroy(_activeShieldInstance);
                _activeShieldInstance = null;
                _nextShieldTime = Time.time + _shieldCooldown;
            }
        }

        private void ActivateShield()
        {
            if (_activeShieldInstance != null)
            {
                Destroy(_activeShieldInstance);
            }

            if (_shieldVfxPrefab != null && _shieldSpawnPoint != null)
            {
                Vector3 localOffset = new Vector3(0f, 0.5f, 0f);
                Vector3 shieldSpawnPoint =
                    _shieldSpawnPoint.position + _shieldSpawnPoint.TransformDirection(localOffset);
                _activeShieldInstance = Instantiate(
                    _shieldVfxPrefab,
                    shieldSpawnPoint,
                    _shieldSpawnPoint.rotation,
                    _shieldSpawnPoint
                );
            }
        }

        public void TakeDamage(float damage)
        {
            float manaBurn = damage * _shieldManaMultiplier;

            if (!_manaRef.ManaDrain(manaBurn))
            {
                if (_activeShieldInstance != null)
                {
                    Destroy(_activeShieldInstance);
                    _activeShieldInstance = null;
                    _nextShieldTime = Time.time + _shieldDestroyedCooldown;
                }
            }
        }
    }
}
