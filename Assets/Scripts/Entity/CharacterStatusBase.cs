using System;
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

        /// <summary>
        /// Current hp
        /// </summary>
        private int _hp;

        public int MaxHP { get { return maxHP; } }
        public int HP { get { return _hp; } }
        public int Speed { get { return speed; } }
        public Action OnDamage { get; set; }

        private void Awake()
        {
            _hp = maxHP;
            CharacterStatusAwake();
        }

        /// <summary>
        /// Awake function for children inherited this character status base class
        /// </summary>
        protected virtual void CharacterStatusAwake()
        {

        }

        /// <summary>
        /// Attack an target
        /// </summary>
        /// <param name="defender"></param>
        public virtual void Attack(CharacterStatusBase defender)
        {
            // TODO: Calculate something
            defender._hp -= attack;
            defender.OnDamage?.Invoke(); // Call action to make when defender is on damage
        }

        /// <summary>
        /// Defense an target
        /// </summary>
        /// <param name="attacker"></param>
        public virtual void Defense(CharacterStatusBase attacker)
        {

        }
    }
}
