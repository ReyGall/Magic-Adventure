using MyGame.Damage_interface;
using MyGame.manaControl;
using MyGame.SpellsCore;
using UnityEngine;

namespace MyGame.SpellZoltraak
{
    public class Zoltraak : SpellBase
    {
        [SerializeField]
        private GameObject _vfxPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private LayerMask _enemyLayer;

        [SerializeField]
        private GameObject _vfxPrefab2;

        [SerializeField]
        private Transform _spawnPoint2;

        [Header("Charge Settings")]
        private float _currentChargeTime = 0f;

        [SerializeField]
        private float _maxChargeTime = 3f; // max charge time

        [SerializeField]
        private float _manaDrainPerSecond = 15f; // holding cost

        [Header("Damage & Radius Scaling")]
        [SerializeField]
        private float _minDamage = 30f;

        [SerializeField]
        private float _maxDamage = 100f;

        [SerializeField]
        private float _minRadius = 0.6f;

        [SerializeField]
        private float _maxRadius = 2.0f;

        [Header("VFX Scale Scaling")]
        [SerializeField]
        private Vector3 _minVfxScale = new Vector3(1f, 1f, 1f);

        [SerializeField]
        private Vector3 _maxVfxScale = new Vector3(2.5f, 2.5f, 2.5f);

        private float _defaultDistance = 10f;
        private bool _isCharging = false;
        public bool IsCharging => _isCharging;

        protected override void Awake()
        {
            base.Awake();
            _manaCost = 30f;
            _damage = _minDamage;

            if (_spawnPoint == null)
            {
                Transform found = transform.Find("PlayerArmature"); 
                _spawnPoint = found != null ? found : transform; 
            }

            if (_spawnPoint2 == null)
            {
                Transform found = transform.Find("PlayerArmature");
                _spawnPoint2 = found != null ? found : _spawnPoint; 
            }
        }

        public override void StartCharge()
        {
            if (SpellCheck())
            {
                _currentChargeTime = 0f;
                _isCharging = true;
            }
        }

        public override void HoldCharge()
        {
            if (!_isCharging)
                return;

            if (_currentChargeTime < _maxChargeTime)
            {
                if (_manaRef != null && _manaRef.ManaDrain(_manaDrainPerSecond * Time.deltaTime))
                {
                    _currentChargeTime += Time.deltaTime;

                    if (_currentChargeTime >= _maxChargeTime)
                    {
                        ExecuteRelease(true);
                    }
                }
                else
                {
                    ExecuteRelease(true);
                }
            }
        }

        public override void ReleaseCharge()
        {
            if (!_isCharging)
                return;
            ExecuteRelease(false);
        }

        private void ExecuteRelease(bool forced)
        {
            _isCharging = false;

            float chargePercent = Mathf.Clamp01(_currentChargeTime / _maxChargeTime);

            float finalDamage = Mathf.Lerp(_minDamage, _maxDamage, chargePercent);
            float finalRadius = Mathf.Lerp(_minRadius, _maxRadius, chargePercent);
            Vector3 finalScale = Vector3.Lerp(_minVfxScale, _maxVfxScale, chargePercent);

            SpawnAndCast(finalDamage, finalRadius, finalScale);

            _currentChargeTime = 0f;
        }

        private void SpawnAndCast(float damage, float radius, Vector3 vfxScale)
        {
            // first vfx
            Vector3 localOffset = new Vector3(0f, 0f, 0f);
            Vector3 spawnPosition =
                _spawnPoint.position + _spawnPoint.TransformDirection(localOffset);
            Quaternion spawnRotation = _spawnPoint.rotation * Quaternion.Euler(0f, 0f, 0f);
            GameObject vfxInstance = Instantiate(_vfxPrefab, spawnPosition, spawnRotation);
            vfxInstance.transform.localScale = vfxScale;
            Destroy(vfxInstance, 3f);

            // second vfx
            Vector3 localOffset2 = new Vector3(0f, 1f, 1.5f);
            Vector3 spawnPosition2 =
                _spawnPoint2.position + _spawnPoint2.TransformDirection(localOffset2);
            Quaternion spawnRotation2 = _spawnPoint2.rotation * Quaternion.Euler(0f, 270f, 0f);
            GameObject vfxInstance2 = Instantiate(_vfxPrefab2, spawnPosition2, spawnRotation2);
            vfxInstance2.transform.localScale = vfxScale;
            Destroy(vfxInstance2, 3f);

            // raycast
            RaycastHit hit;
            Debug.DrawRay(
                _spawnPoint2.position,
                -_spawnPoint2.right * _defaultDistance,
                Color.red,
                1f
            );

            if (
                Physics.SphereCast(
                    _spawnPoint2.position,
                    radius,
                    _spawnPoint2.forward,
                    out hit,
                    _defaultDistance,
                    _enemyLayer
                )
            )
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                    {
                        damageable.TakeDamage(damage);
                    }
                }
            }
        }
    }
}
