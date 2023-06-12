using UnityEngine;

namespace Entity
{
    public abstract class EntityStatusBase : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private int level = 1;
        [SerializeField] private int maxHP = 100;

        [Header("Attributes")]
        [SerializeField] private int attack = 1;
        [SerializeField] private int defense = 1;
        [SerializeField] private int speed = 1;

        public virtual void Attack(EntityStatusBase defender)
        {
            // TODO: Calculate something
        }

        public virtual void Defense(EntityStatusBase attacker)
        {

        }
    }
}
