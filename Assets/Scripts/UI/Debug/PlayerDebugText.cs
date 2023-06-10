using StateMachine.Player;
using TMPro;
using UnityEngine;

namespace UI.Debug
{
    public class PlayerDebugText : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine playerStateMachine;
        [SerializeField] private TMP_Text finalMoveText;
        [SerializeField] private TMP_Text currentMoveText;
        [SerializeField] private TMP_Text propertiesText;

        private void Update()
        {
            if(playerStateMachine == null)
            {
                playerStateMachine = GameManager.Instance.Player;
                return;
            }
            finalMoveText.text = "Apply Move: " + "("+ playerStateMachine.ApplyMovementX+ ", "+ playerStateMachine.ApplyMovementY + ","+ playerStateMachine.ApplyMovementZ+ ")";
            currentMoveText.text = "Current Move: " + playerStateMachine.CurrentMove.ToString();
            string sub = "Invalid";
            if(playerStateMachine.CurrentState.CurrentSubState != null)
            {
                sub = playerStateMachine.CurrentState.CurrentSubState.GetType().Name;
            }
            propertiesText.text = "Current State: " + playerStateMachine.CurrentState.GetType().Name + " Sub: " + sub;
        }
    }
}
