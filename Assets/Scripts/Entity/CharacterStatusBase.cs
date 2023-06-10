using UnityEngine;

namespace Entity
{
    public abstract class CharacterStatusBase : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private int level = 1;
        [SerializeField] private int maxHP = 100;

        [Header("Attributes")]
        [SerializeField] private int attack = 1;
        [SerializeField] private int defense = 1;

        public virtual void Attack(CharacterStatusBase status)
        {
            // TODO: Calculate something
        }
    }
}
