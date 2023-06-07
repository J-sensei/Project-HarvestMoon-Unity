using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameMenu : Singleton<GameMenu>
{
    protected override void AwakeSingleton()
    {
        
    }

    public void ToggleGameMenu(bool v)
    {
        gameObject.SetActive(v);
    }
}
