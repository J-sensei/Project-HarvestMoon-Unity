using UnityEngine;

namespace Entity
{
    public class PlayerStatus : CharacterStatusBase
    {
        [Header("Player Status")]
        [Tooltip("Stamina bar to use tool and skill, use up all stamina will go to sleep or die?")]
        [SerializeField] private int maxStamina = 100;
        private int _stamina;

        public int MaxStamina { get { return maxStamina; } }
        public int Stamina { get { return _stamina; } }

        protected override void CharacterStatusAwake()
        {
            _stamina = maxStamina;
        }
    }
}