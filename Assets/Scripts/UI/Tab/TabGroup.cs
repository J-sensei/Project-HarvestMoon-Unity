using System.Collections.Generic;
using UnityEngine;

namespace UI.Tab
{
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> _tabButtons;
        [SerializeField] private List<GameObject> contents;

        [Header("Tab States UI")]
        [SerializeField] private Sprite idle;
        [SerializeField] private Sprite hover;
        [SerializeField] private Sprite active;
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;

        private TabButton _selectedTab;

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
            for(int i = 0; i < contents.Count; i++)
            {
                if(i == index)
                {
                    contents[i].SetActive(true);
                }
                else
                {
                    contents[i].SetActive(false);
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
