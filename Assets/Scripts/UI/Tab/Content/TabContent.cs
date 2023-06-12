using UnityEngine;

namespace UI.Tab.Content
{
    public class TabContent : MonoBehaviour
    {
        /// <summary>
        /// When tab is open
        /// </summary>
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// When tab is close
        /// </summary>
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}