using Inventory;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class GameUIManager : Singleton<GameUIManager>
{
    [SerializeField] private InventorySlot equipedItem;

    [Header("Texts")]
    [SerializeField] private TMP_Text seasonText;
    [SerializeField] private TMP_Text timeText;

    protected override void AwakeSingleton()
    {
        
    }

    public void UpdateEquipedItem(ItemData data)
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
}
