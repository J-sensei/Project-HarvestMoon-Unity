using GameDateTime;
using Inventory;
using Inventory.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using Utilities;

public class GameUIManager : Singleton<GameUIManager>, ITimeChecker
{
    [SerializeField] private InventorySlot equipedItem;

    [Header("Texts")]
    [SerializeField] private TMP_Text seasonText;
    [SerializeField] private TMP_Text timeText;

    protected override void AwakeSingleton()
    {
        //GameTimeManager.Instance.AddListener(this); // No need to remove as it will persists throughout the game
        StartCoroutine(Initialize());
    }

    /// <summary>
    /// Make sure the game time manager is initialized before subcribe to the ITimeChecker
    /// </summary>
    /// <returns></returns>
    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.AddListener(this);
        }

    }

    public void UpdateEquipedItem(ItemSlot data)
    {
        if(equipedItem != null)
        {
            equipedItem.Display(data);
        }
        else
        {
            Debug.LogWarning("[Game UI Manager] Equiped Item Slot is null");
        }
    }

    #region ITimeChecker
    public void ClockUpdate(GameTime gameTime)
    {
        seasonText.text = gameTime.CurrentSeason.ToString() + " " + gameTime.Day;
        timeText.text = gameTime.TimeString();
    }

    public void NewDay(GameTime gameTime)
    {
        
    }
    #endregion
}
