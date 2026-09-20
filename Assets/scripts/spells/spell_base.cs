using MyGame.manaControl;
using UnityEngine;

namespace MyGame.SpellsCore
{
    public class SpellBase : MonoBehaviour
    {
        [SerializeField]
        protected float _manaCost = 0f;

        [SerializeField]
        protected float _damage = 0f;

        [SerializeField]
        protected bool _spellCast = false;
        protected Mana _manaRef;

        protected virtual void Awake()
        {
            _manaRef = GetComponent<Mana>();
        }

        public bool SpellCheck()
        {
            _spellCast = false;
            if (_manaCost <= _manaRef.CheckMana())
            {
                _manaRef.ManaDrain(_manaCost);
                _spellCast = true;
            }

            return _spellCast;
        }
    }
}
