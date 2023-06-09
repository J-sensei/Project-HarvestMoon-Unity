using UnityEngine;
using Utilities;

namespace UI.FadeScreen
{
    /// <summary>
    /// Manage to fade the screen
    /// </summary>
    public class FadeScreenManager : Singleton<FadeScreenManager>
    {
        [Tooltip("Fade Panel reference")]
        [SerializeField] private FadePanel fadePanel;

        public FadePanel FadePanel { get { return fadePanel; } }
        protected override void AwakeSingleton()
        {
            if(fadePanel == null)
            {
                fadePanel = transform.GetComponentInChildren<FadePanel>();
            }
        }
    }
}
