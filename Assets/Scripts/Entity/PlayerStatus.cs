using UnityEngine;

namespace Entity
{
    [System.Serializable]
    public struct PlayerStatusSave
    {
        public int hp;
        public int stamina;

        public PlayerStatusSave(int hp, int stamina)
        {
            this.hp = hp;
            this.stamina = stamina;
        }
    }

    public class PlayerStatus : CharacterStatusBase
    {
        [Header("Player Status")]
        [Tooltip("Stamina bar to use tool and skill, use up all stamina will go to sleep or die?")]
        [SerializeField] private int maxStamina = 100;
        private int _stamina;

        public int MaxStamina { get { return maxStamina; } }
        public int Stamina { get { return _stamina; } }
        public PlayerStatusSave StatusSave { get
            {
                return new PlayerStatusSave(_hp, _stamina);
            } 
        }

        protected override void CharacterStatusAwake()
        {
            _stamina = maxStamina;
        }

        public override void Load(CharacterStatusBase source)
        {
            base.Load(source);
            _stamina = ((PlayerStatus)source)._stamina;

            GameUIManager.Instance.UpdatePlayerStatusUI(this);
        }

        public void Load(PlayerStatusSave save)
        {
            _hp = save.hp;
            _stamina = save.stamina;

            GameUIManager.Instance.UpdatePlayerStatusUI(this);
        }

        public void Reset()
        {
            _hp = MaxHP;
            _stamina = maxStamina;
        }

        /// <summary>
        /// Update the player status and update the the game ui
        /// </summary>
        /// <param name="hp">Amount of hp to minus</param>
        /// <param name="stamina">Amount of stamina to minus</param>
        public void UpdateStatus(int hp = -1, int stamina = -1)
        {
            if(hp > 0)
            {
                _hp -= hp;
            }

            if(stamina > 0)
            {
                _stamina -= stamina;
            }

            GameUIManager.Instance.UpdatePlayerStatusUI(this);
        }

        /// <summary>
        /// Fully recover both hp and stamina of the player
        /// </summary>
        /// <param name="multiplier">From 0 - 1, how much hp and stamina need the player to recover from the max value</param>
        public void Recover(float multiplier = 1f)
        {
            _hp = (int)(MaxHP * multiplier);
            _stamina = (int)(MaxStamina * multiplier);
            GameUIManager.Instance.UpdatePlayerStatusUI(this);
        }
    }
}