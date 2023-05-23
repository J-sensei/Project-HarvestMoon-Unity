using GameDateTime;
using System.Collections;
using UnityEngine;

namespace Interactable
{
    public class Bed : MonoBehaviour, IInteractable
    {
        [SerializeField] FadePanel fadePanel;
        [SerializeField] private Outline outline;
        const float initOutlineTime = 0.05f;

        private bool _fadeIn = false;
        private bool _fadeOut = false;

        private void Start()
        {
            if(outline == null)
            {
                outline = GetComponent<Outline>();
            }
            outline.enabled = false;
            // For some reason this can make the setting applied
            StartCoroutine(InitOutline(outline));
        }

        IEnumerator InitOutline(Outline outline)
        {
            yield return new WaitForSeconds(initOutlineTime);
            outline.enabled = true;
            yield return new WaitForSeconds(initOutlineTime);
            outline.enabled = false;
        }

        public void Interact()
        {
            fadePanel.FadeOut();
            _fadeIn = true;
            fadePanel.OnFinish.AddListener(FadeIn);
            //GameTimeManager.Instance.Sleep();
        }

        private void FadeIn()
        {
            fadePanel.OnFinish.RemoveAllListeners();
            GameTimeManager.Instance.Sleep();
//            GameTimeManager.Instance.PauseTime(true);
            _fadeIn = false;

            fadePanel.FadeIn();
            _fadeOut = true;
            fadePanel.OnFinish.AddListener(FadeOut);
        }

        private void FadeOut()
        {
            fadePanel.OnFinish.RemoveAllListeners();
//            GameTimeManager.Instance.PauseTime(false);
            Debug.Log("Execute sleep");


            _fadeOut = false;
        }

        //private void Update()
        //{
        //    if(_fadeOut == false)
        //    {
        //        GameTimeManager.Instance.PauseTime(false);
        //    }
        //}

        public void OnSelect(bool v)
        {
            outline.enabled = v;
        }
    }
}
