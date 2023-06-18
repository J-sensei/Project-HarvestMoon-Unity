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
            Debug.Log("Load Player Status: HP: " + save.hp + " ST: " + save.stamina);
            _hp = save.hp;
            _stamina = save.stamina;

            GameUIManager.Instance.UpdatePlayerStatusUI(this);
        }
    }
}