using UnityEngine;

namespace Entity
{
    public class CharacterStatusBase : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private int level = 1;
        [SerializeField] private int maxHP = 100;

        [Header("Attributes")]
        [SerializeField] private int attack = 1;
        [SerializeField] private int defense = 1;
        [SerializeField] private int speed = 1;

        public int Speed { get { return speed; } }

        public virtual void Attack(CharacterStatusBase defender)
        {
            // TODO: Calculate something
            defender.maxHP -= attack;
        }

        public virtual void Defense(CharacterStatusBase attacker)
        {

        }
    }
}
