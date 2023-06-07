using Inventory.UI;
using UI.Tab;
using UI.Tooltip;
using UnityEngine;
using Utilities;

public class GameMenu : Singleton<GameMenu>
{
    [SerializeField] private TabGroup tabGroup;
    protected override void AwakeSingleton()
    {
        
    }

    public void ToggleGameMenu(bool v)
    {
        if (!v)
        {
            TooltipManager.Instance.Hide();
            InventoryUIManager.Instance.ResetInventorySlots();
        }

        gameObject.SetActive(v);
        if (v && tabGroup)
        {
            tabGroup.SetTab(0); // 0 => First Tab
        }
    }

    public void ToggleInventory(bool v)
    {
        ToggleGameMenu(v);
        if (v && tabGroup)
        {
            tabGroup.SetTab(1); // 1 => Inventory Tab
        }
    }
}
