using UnityEngine;

namespace MyGame.manaControl
{
    public class Mana : MonoBehaviour
    {
        private float _mana = 100f;
        private float _maxMana = 100f;
        private bool _cast = false;
        private float _regenRate = 5f;

        private void Update()
        {
            ManaRegen();
        }

        public bool ManaDrain(float amount)
        {
            _cast = false;

            if (_mana >= amount)
            {
                _mana -= amount;
                _cast = true;
            }

            return _cast;
        }

        public float CheckMana()
        {
            return _mana;
        }

        public float ManaRegen()
        {
            if (_mana < _maxMana)
            {
                _mana += Time.deltaTime * _regenRate;
                if (_mana > _maxMana)
                {
                    _mana = _maxMana;
                }
            }

            return _mana;
        }
    }
}
