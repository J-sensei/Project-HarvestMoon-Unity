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
        protected int _hp;
        private bool _requestDefense = false;

        /// <summary>
        /// Maximum HP of the character
        /// </summary>
        public int MaxHP { get { return maxHP; } }
        /// <summary>
        /// Current HP of the character
        /// </summary>
        public int HP { get { return _hp; } }
        /// <summary>
        /// Get the HP value within 0 and 1 range
        /// </summary>
        public float HP01
        {
            get
            {
                return (float)((float)_hp / (float)maxHP);
            }
        }

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
        /// Get defense for the next attack
        /// </summary>
        public void RequestDefense()
        {
            _requestDefense = true;
        }
        public void CancelDefense()
        {
            _requestDefense = false;
        }

        public virtual void Load(CharacterStatusBase source)
        {
            _hp = source._hp; // Currently on hp is using
        }

        /// <summary>
        /// Attack an target
        /// </summary>
        /// <param name="defender"></param>
        public virtual void Attack(CharacterStatusBase defender)
        {
            CombatManager.Instance.Camera.Shake();

            int attackPoint = 0;
            if (defender._requestDefense)
            {
                attackPoint = Math.Clamp((attack - defender.defense), 0, int.MaxValue);
                defender._hp -= attackPoint; // Minus the defense point
                defender._requestDefense = false; // Reset
            }
            else
            {
                attackPoint = attack;
                defender._hp -= attackPoint;
            }

            if (damageUI != null)
            {
                DamageUI d = Instantiate(damageUI, transform.position, Quaternion.identity);
                d.Play(transform.position, attackPoint);
            }

            defender.OnDamage?.Invoke(); // Call action to make when defender is on damage

            if (defender._hp <= 0)
            {
                defender.OnDie?.Invoke();
                return;
            }
        }

        /// <summary>
        /// Attack with specific damage 
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public virtual void Attack(CharacterStatusBase defender, int damage)
        {
            CombatManager.Instance.Camera.Shake();

            int attackPoint = 0;
            if (defender._requestDefense)
            {
                attackPoint = Math.Clamp((damage - defender.defense), 0, int.MaxValue);
                defender._hp -= attackPoint; // Minus the defense point
                defender._requestDefense = false; // Reset
            }
            else
            {
                attackPoint = damage;
                defender._hp -= attackPoint;
            }
            if (damageUI != null)
            {
                DamageUI d = Instantiate(damageUI, defender.transform.position, Quaternion.identity);
                d.Play(defender.transform.position, attackPoint);
            }

            defender.OnDamage?.Invoke(); // Call action to make when defender is on damage

            if (defender._hp <= 0)
            {
                defender.OnDie?.Invoke();
                return;
            }
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
