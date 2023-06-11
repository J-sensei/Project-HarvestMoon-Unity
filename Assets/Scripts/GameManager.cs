using StateMachine.Player;
using UnityEngine;
using Utilities;
using TopDownCamera;

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
                GameObject go = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG);
                if(go != null)
                {
                    player = go.GetComponent<PlayerStateMachine>();
                }
            }
            return player;
        } 
    }

    [SerializeField] private TopDownCamera.TopDownCamera _topDownCamera;
    public TopDownCamera.TopDownCamera Camera
    {
        get
        {
            if(_topDownCamera == null)
            {
                //_topDownCamera = UnityEngine.Camera.main.GetComponent<TopDownCamera.TopDownCamera>();
                _topDownCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TopDownCamera.TopDownCamera>();
            }
            return _topDownCamera;
        }
    }

    protected override void AwakeSingleton()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG);
            if (go != null)
            {
                player = go.GetComponent<PlayerStateMachine>();
            }
        }

        if(_topDownCamera == null)
        {
            _topDownCamera = UnityEngine.Camera.main.GetComponent<TopDownCamera.TopDownCamera>();
        }
    }
}
