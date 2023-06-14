using Combat;
using System;
using UI.Combat;
using UnityEngine;

namespace Entity
{
    public class CharacterStatusBase : MonoBehaviour
    {
        [Header("Status")]
        [Header("Level")]
        [SerializeField] private int level = 1;
        [Tooltip("How the level affect the values of the character such as hp, attack and speed. Value = level * levelMultiplier")]
        [SerializeField] private float levelMultiplier = 1.1f;

        [Header("HP")]
        [SerializeField] private int maxHP = 100;

        [Header("Exp")]
        [SerializeField] private int baseExp = 100;
        [Tooltip("How many exp required to level up")]
        [SerializeField] private float expMultiplier = 1.1f;

        [Header("Attributes")]
        [SerializeField] private int attack = 1;
        [SerializeField] private int defense = 1;
        [SerializeField] private int speed = 1;

        [Header("UI")]
        [SerializeField] private DamageUI damageUI;

        /// <summary>
        /// Current hp
        /// </summary>
        private int _hp;

        /// <summary>
        /// Maximum HP of the character
        /// </summary>
        public int MaxHP { get { return maxHP; } }
        /// <summary>
        /// Current HP of the character
        /// </summary>
        public int HP { get { return _hp; } }
        /// <summary>
        /// Speed value of the character
        /// </summary>
        public int Speed { get { return speed; } }
        /// <summary>
        /// When character get hit
        /// </summary>
        public Action OnDamage { get; set; }
        /// <summary>
        /// When character die
        /// </summary>
        public Action OnDie { get; set; }

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
            if (damageUI != null)
            {
                DamageUI d = Instantiate(damageUI, transform.position, Quaternion.identity);
                d.Play(transform.position, attack);
            }

            CombatManager.Instance.Camera.Shake();

            // TODO: Calculate something
            defender._hp -= attack;

            if(defender._hp <= 0)
            {
                defender.OnDie?.Invoke();
                return;
            }

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
