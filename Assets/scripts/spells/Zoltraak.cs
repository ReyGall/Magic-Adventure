using MyGame.manaControl;
using MyGame.SpellsCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame.SpellZoltraak
{
    public class Zoltraak : SpellBase
    {
        [SerializeField]
        private GameObject _vfxPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private GameObject _vfxPrefab2;

        [SerializeField]
        private Transform _spawnPoint2;

        protected override void Awake()
        {
            base.Awake();
            _manaCost = 30f;
            _damage = 50f;
        }

        private void Update()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
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
                    Quaternion spawnRotation2 =
                        _spawnPoint2.rotation * Quaternion.Euler(0f, 270f, 0f);
                    GameObject vfxInstance2 = Instantiate(
                        _vfxPrefab2,
                        spawnPosition2,
                        spawnRotation2
                    );
                    Destroy(vfxInstance2, 3f);
                }
            }
        }
    }
}
