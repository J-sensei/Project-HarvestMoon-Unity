using DG.Tweening;
using System;
using UI.Combat;
using UI.Tooltip;
using UnityEngine;
using Utilities;

namespace Inventory.UI
{
    // Manage items use in battle
    public class BattleInventoryManager : Singleton<BattleInventoryManager>
    {
        [Header("Inventory System")]
        [Tooltip("Game object that holds multiple inventory slot of items")]
        [SerializeField] private GameObject itemHolder;

        private CanvasGroup _canvasGroup;
        private Vector3 _originalPosition;
        private float _duration = 0.15f;
        private float _tweenMoveDistance = 40f;

        /// <summary>
        /// Item inventory slot
        /// </summary>
        private CombatInventorySlot[] _itemSlots;

        protected override void AwakeSingleton()
        {
            // Polish stuff
            _canvasGroup = GetComponent<CanvasGroup>();
            _originalPosition = transform.position;

            // Initialize item slots
            _itemSlots = GetComponentsInChildren<CombatInventorySlot>();
        }

        private void Start()
        {
            // Assign IDs
            for (int i = 0; i < _itemSlots.Length; i++)
            {
                //_toolSlots[i].SetId(i);
                _itemSlots[i].SetId(i);
            }
        }

        public void Toggle(bool v)
        {
            if (!v) // Close
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClose, 1);

                // Tween
                Vector3 pos = _originalPosition;
                pos.y -= _tweenMoveDistance;

                transform.DOMove(pos, _duration);

                _canvasGroup.alpha = 1f;
                _canvasGroup.DOFade(0f, _duration).OnComplete(() =>
                {
                    TooltipManager.Instance.Hide(); // Reset the tooltip to solve any tooltip ui bug (not active/deactive correctly)
                    gameObject.SetActive(v);
                });

                CombatUIManager.Instance.ToggleActionUI(true);
            }
            else // Open
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.menuOpen, 1);

                // Tween
                gameObject.SetActive(v);
                Vector3 pos = _originalPosition;
                pos.y += _tweenMoveDistance;
                transform.position = pos;

                transform.DOMove(_originalPosition, _duration);

                _canvasGroup.alpha = 0f;
                _canvasGroup.DOFade(1f, _duration);

                CombatUIManager.Instance.ToggleActionUI(false);
                UpdateInventoryUI();
            }
        }

        /// <summary>
        /// Update UI display information
        /// </summary>
        public void UpdateInventoryUI()
        {
            // Get the item data
            //ItemData[] tools = InventoryManager.Instance.Tools;
            ItemSlot[] items = InventoryManager.Instance.ItemSlots;

            // Update the inventory slot using the items data
            //UpdateInventory(tools, _toolSlots);
            UpdateInventory(items, _itemSlots);
        }

        /// <summary>
        /// Update inventory slots in the inventory UI
        /// </summary>
        /// <param name="data"></param>
        /// <param name="slots"></param>
        private void UpdateInventory(ItemSlot[] data, InventorySlot[] slots)
        {
            // Assume that item data and slot has same length, this will not give any trouble
            //Debug.Log(data + "    " + slots);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Display(data[i]); // Update the item display in the inventory slot
            }
        }
    }
}
