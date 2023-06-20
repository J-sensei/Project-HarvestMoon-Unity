using Entity;
using GameDateTime;
using Inventory;
using Inventory.UI;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using Utilities;

public class GameUIManager : Singleton<GameUIManager>, ITimeChecker
{
    [Header("Player Status")]
    [SerializeField] private ProgressBar hpProgress;
    [SerializeField] private ProgressBar staminaProgress;

    [Header("Inventory")]
    [SerializeField] private InventorySlot equipedItem;

    [Header("Texts")]
    [SerializeField] private TMP_Text seasonText;
    [SerializeField] private TMP_Text timeText;

    [Header("GameObject")]
    [SerializeField] private GameObject timePanel;

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
        UpdatePlayerStatusUI(GameManager.Instance.Player.PlayerStatus);
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

    public void UpdatePlayerStatusUI(PlayerStatus playerStatus)
    {
        hpProgress.UpdateValues(0, playerStatus.MaxHP);
        staminaProgress.UpdateValues(0, playerStatus.MaxStamina);

        hpProgress.UpdateValue(playerStatus.HP);
        staminaProgress.UpdateValue(playerStatus.Stamina);
    }

    public void UpdatePlayerStatusUI(CharacterStatusBase playerStatus)
    {
        hpProgress.UpdateValues(0, playerStatus.MaxHP);

        hpProgress.UpdateValue(playerStatus.HP);
    }

    public void DisableMenu(bool v)
    {
        timePanel.SetActive(!v);
    }

    public bool TimePanelActiveSelf
    {
        get { return timePanel.activeSelf; }
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
