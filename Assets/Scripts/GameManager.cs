using StateMachine.Player;
using UnityEngine;
using Utilities;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerStateMachine player;
    public PlayerStateMachine Player 
    { 
        get 
        {
            // Keep player reference available
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG).GetComponent<PlayerStateMachine>();
            }
            return player;
        } 
    }

    protected override void AwakeSingleton()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG).GetComponent<PlayerStateMachine>();
        }
    }
}
