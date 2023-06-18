using System.Collections.Generic;
using UI.Tab.Content;
using UI.Tooltip;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace UI.Tab
{
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> _tabButtons;
        [SerializeField] private List<TabContent> contents;
        private TabContent _previousContent;

        [Header("Tab States UI")]
        [SerializeField] private Sprite idle;
        [SerializeField] private Sprite hover;
        [SerializeField] private Sprite active;
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;

        private InputControls _inputControls;
        private TabButton _selectedTab;
        private int _selectedIndex;

        //private void Start()
        //{
        //    for (int i = 0; i < contents.Count; i++)
        //    {
        //        if (!contents[i].gameObject.activeSelf)
        //        {
        //            contents[i].gameObject.SetActive(true);
        //            contents[i].gameObject.SetActive(false);
        //            break;
        //        }
        //    }
        //}

        private void Awake()
        {
            _inputControls = new();
            _inputControls.UI.TabLeft.started += OnTabLeft;
            _inputControls.UI.TabRight.started += OnTabRight;
        }

        private void OnTabRight(InputAction.CallbackContext context)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick);
            _selectedIndex++;
            if (_selectedIndex > contents.Count - 1) _selectedIndex = 0;

            SetTab(_selectedIndex);
            TooltipManager.Instance.Hide();
        }
        private void OnTabLeft(InputAction.CallbackContext context)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick);
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = contents.Count - 1;

            SetTab(_selectedIndex);
            TooltipManager.Instance.Hide();
        }

        private void OnEnable()
        {
            _inputControls.UI.TabLeft.Enable();
            _inputControls.UI.TabRight.Enable();
        }

        private void OnDisable()
        {
            _inputControls.UI.TabLeft.Disable();
            _inputControls.UI.TabRight.Disable();
        }

        public void Close()
        {
            _previousContent = null;
        }

        public void SetTab(int index)
        {
            if (index < 0 || index >= contents.Count)
            {
                UnityEngine.Debug.LogWarning("[Tab Group] Invalid index!");
                return;
            }

            TabButton resultButton = null;
            foreach(TabButton button in _tabButtons)
            {
                int i = button.transform.GetSiblingIndex();
                if(i == index)
                {
                    resultButton = button;
                    break;
                }
            }

            if(resultButton == null)
            {
                UnityEngine.Debug.LogWarning("[Tab Group] Cannot find the tab button!");
                return;
            }
            OnTabSelect(resultButton);
        }

        public void Add(TabButton button)
        {
            if(_tabButtons == null)
            {
                _tabButtons = new();
            }

            _tabButtons.Add(button);
        }

        /// <summary>
        /// Tab hover
        /// </summary>
        /// <param name="button"></param>
        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if(_selectedTab == null || button != _selectedTab)
            {
                button.SetSprite(hover);
                button.SetColor(hoverColor);
            }
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelect(TabButton button)
        {
            if (_selectedTab != null) _selectedTab.Deselect();
            _selectedTab = button;
            _selectedTab.Select();

            ResetTabs();
            button.SetSprite(active);
            button.SetColor(activeColor);

            int index = button.transform.GetSiblingIndex();
            _selectedIndex = index;

            int prevIndex = -1;
            if (_previousContent != null && _previousContent.transform.GetSiblingIndex() != index)
            {
                prevIndex = _previousContent.transform.GetSiblingIndex();
                _previousContent.Close();
            }

            // Change contents
            for (int i = 0; i < contents.Count; i++)
            {
                if(_previousContent != null && contents[i] == _previousContent)
                {
                    if (i == index) 
                        continue;
                }

                if(i == index)
                {
                    contents[i].Open();
                    _previousContent = contents[i];
                }
                else if((prevIndex >= 0 && prevIndex != i) || prevIndex < 0)
                {
                    contents[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetTabs()
        {
            foreach(TabButton button in _tabButtons)
            {
                if (_selectedTab != null && _selectedTab == button) continue;

                button.SetSprite(idle);
                button.SetColor(idleColor);
            }
        }
    }
}
