using StateMachine.Player;
using UnityEngine;
using Utilities;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerStateMachine player;
    public PlayerStateMachine Player { get { return player; } }

    protected override void AwakeSingleton()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        }
    }

    private void Update()
    {
        // Keep player reference available
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        }
    }
}
