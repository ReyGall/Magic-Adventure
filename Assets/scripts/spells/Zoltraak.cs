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

        private float _radius = 0.6f;
        private float _defaultDistance = 10f;

        protected override void Awake()
        {
            base.Awake();
            _manaCost = 30f;
            _damage = 50f;
        }

        public override void Cast()
        {
            if (SpellCheck())
            {
                Vector3 localOffset = new Vector3(0f, 0f, 0f);
                Vector3 spawnPosition =
                    _spawnPoint.position + _spawnPoint.TransformDirection(localOffset);
                Quaternion spawnRotation = _spawnPoint.rotation * Quaternion.Euler(0f, 0f, 0f);
                GameObject vfxInstance = Instantiate(_vfxPrefab, spawnPosition, spawnRotation);
                Destroy(vfxInstance, 3f);
                Vector3 localOffset2 = new Vector3(0f, 1f, 1.5f);
                Vector3 spawnPosition2 =
                    _spawnPoint2.position + _spawnPoint2.TransformDirection(localOffset2);
                Quaternion spawnRotation2 = _spawnPoint2.rotation * Quaternion.Euler(0f, 270f, 0f);
                GameObject vfxInstance2 = Instantiate(_vfxPrefab2, spawnPosition2, spawnRotation2);
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
                        _radius,
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
                            damageable.TakeDamage(_damage);
                        }
                    }
                }
                Destroy(vfxInstance2, 3f);
            }
        }
    }
}
