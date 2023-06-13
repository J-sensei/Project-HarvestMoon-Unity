using System;
using UnityEngine;

namespace Combat
{
    public class CombatAttackController : MonoBehaviour
    {
        private Action _onAttack;

        public void Initialize(Action onAttack)
        {
            _onAttack = onAttack;
        }

        /// <summary>
        /// Call when certain attack frame is played
        /// </summary>
        public void AttackFrame()
        {
            _onAttack?.Invoke();
        }
    }
}
