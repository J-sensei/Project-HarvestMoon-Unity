using Entity;
using System;
using UnityEngine;

namespace Combat
{
    public class CombatAnimationController : MonoBehaviour
    {
        private Action _onAttack;
        public Action OnAttack { get { return _onAttack; } set { _onAttack = value; } }
        public Action OnHurtFinish { get; set; }
        public Action OnAttackFinish { get; set; }

        /// <summary>
        /// Call when certain attack frame is played
        /// </summary>
        public void AttackFrame()
        {
            _onAttack?.Invoke();

            if(GameUIManager.Instance != null)
                GameUIManager.Instance.UpdatePlayerStatusUI((PlayerStatus)CombatManager.Instance.Player.CharacterStatus);
        }

        public void HurtFinishFrame()
        {
            OnHurtFinish?.Invoke();
        }

        public void AttackFinish()
        {
            OnAttackFinish?.Invoke();
            Debug.Log("Attack Finish Frame");
        }
    }
}
