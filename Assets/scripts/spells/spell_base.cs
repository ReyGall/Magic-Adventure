using UnityEngine;
using MyGame.manaControl;
namespace MyGame.SpellBase
{
    
    public class SpellBase : MonoBehaviour
    {
        
    [SerializeField] protected float _manaCost = 0f;
    [SerializeField] protected float _damage = 0f;
    [SerializeField] protected bool _spellCast = false;
    protected Mana _manaRef;
    protected virtual void Awake()
        {
            
            _manaRef = GetComponent<Mana>();
        }
    
    public bool SpellCheck(bool spellCast)
        {
            _spellCast = false;
            if (_manaCost <= _manaRef.CheckMana())
            {
                _manaRef.ManaDrain(_manaCost);
                spellCast = true;
            }

            return spellCast;
        }

    }

}
