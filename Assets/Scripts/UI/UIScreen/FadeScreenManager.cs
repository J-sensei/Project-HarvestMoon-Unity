using UnityEngine;
using Utilities;

namespace UI.UIScreen
{
    /// <summary>
    /// Manage to fade the screen
    /// </summary>
    public class FadeScreenManager : Singleton<FadeScreenManager>
    {
        [Tooltip("Fade Panel reference")]
        [SerializeField] private FadePanel fadePanel;
        [Tooltip("Loading Panel reference")]
        [SerializeField] private SpriteAnimationUI loadingPanel;

        public FadePanel FadePanel { get { return fadePanel; } }
        protected override void AwakeSingleton()
        {
            if(fadePanel == null)
            {
                fadePanel = transform.GetComponentInChildren<FadePanel>();
            }

            if (loadingPanel == null)
            {
                loadingPanel = transform.GetComponentInChildren<SpriteAnimationUI>();
            }
        }

        public void Loading(bool v)
        {
            if (v)
            {
                loadingPanel.gameObject.SetActive(v);
                loadingPanel.Play();
            }
            else
            {
                loadingPanel.Stop();
                loadingPanel.gameObject.SetActive(v);
            }
        }
    }
}
