using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace UI.Tooltip
{
    public class TooltipManager : Singleton<TooltipManager>
    {
        [SerializeField] private Tooltip _tooltip;
        private LTDescr _delayTween;

        /// <summary>
        /// Is tooltip showing to the player
        /// </summary>
        public bool ShowingTooltip { get; private set; }

        protected override void AwakeSingleton()
        {
            
        }

        public void Show(string description, string title = "")
        {
            _delayTween = LeanTween.delayedCall(0.25f, () => {
                _tooltip.SetText(description, title);
                _tooltip.Show();
            });

            ShowingTooltip = true;
        }

        public void Hide()
        {
            if(_delayTween != null)
                LeanTween.cancel(_delayTween.uniqueId);
            _tooltip.Hide();

            ShowingTooltip = false;
        }
    }
}
