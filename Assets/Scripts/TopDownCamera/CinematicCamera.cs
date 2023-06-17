using Combat;
using System.Collections.Generic;
using UI.UIScreen;
using UnityEngine;
using Utilities;

namespace TopDownCamera
{
    /// <summary>
    /// Move the camera in cinematic way
    /// </summary>
    public class CinematicCamera : Singleton<CinematicCamera>
    {
        [SerializeField] TopDownCamera topDownCamera;
        [SerializeField] private List<CameraMoveData> intro;
        [SerializeField] private CameraMoveData normal;
        [SerializeField] private CameraMoveData win;
        private float _timer;
        private int currentIntro = 0;

        private bool _finishIntro = false;
        private bool _pause = false;
        private bool _win = false;

        protected override void AwakeSingleton()
        {
            if (topDownCamera != null)
            {
                topDownCamera = GetComponent<TopDownCamera>();
            }
            
            topDownCamera.Height = intro[0].height;
            topDownCamera.Distance = intro[0].distance;
            topDownCamera.Angle = intro[0].angle;
            _timer = intro[0].stayTime;
            topDownCamera.UpdateTargetAndInitialize(intro[0].target);
        }

        [Header("Rotation")]
        [SerializeField] private float rotateAngle = 0.5f;

        private void Update()
        {
            // Keep rotating around the player after win
            if (_win)
            {
                float vel = 0f;
                topDownCamera.Angle = Mathf.SmoothDamp(topDownCamera.Angle, topDownCamera.Angle + rotateAngle, ref vel, Time.deltaTime);
                if (topDownCamera.Angle < 0)
                {
                    topDownCamera.Angle = 360f;
                }
                else if (topDownCamera.Angle > 360f)
                {
                    topDownCamera.Angle = 0;
                }
            }
            if (_finishIntro || _pause) return;

            _timer -= Time.deltaTime;
            if(_timer <= 0)
            {
                if(++currentIntro < intro.Count)
                {
                    if (intro[currentIntro].fade)
                    {
                        _pause = true;
                        FadeScreenManager.Instance.FadePanel.FadeDuration = 1.5f;
                        FadeScreenManager.Instance.FadePanel.OnFinish.AddListener(() =>
                        {
                            topDownCamera.Height = intro[currentIntro].height;
                            topDownCamera.Distance = intro[currentIntro].distance;
                            topDownCamera.Angle = intro[currentIntro].angle;
                            _timer = intro[currentIntro].stayTime;
                            topDownCamera.UpdateTargetAndInitialize(intro[currentIntro].target);
                        });
                        FadeScreenManager.Instance.FadePanel.FadeOutIn(() =>
                        {
                            _pause = false;
                        });
                        return;
                    }
                    topDownCamera.Height = intro[currentIntro].height;
                    topDownCamera.Distance = intro[currentIntro].distance;
                    topDownCamera.Angle = intro[currentIntro].angle;
                    _timer = intro[currentIntro].stayTime;
                    topDownCamera.UpdateTarget(intro[currentIntro].target);
                }
                else
                {
                    if (normal.fade)
                    {
                        _pause = true;
                        FadeScreenManager.Instance.FadePanel.FadeDuration = 1.5f;
                        FadeScreenManager.Instance.FadePanel.OnFinish.AddListener(() =>
                        {
                            topDownCamera.Height = normal.height;
                            topDownCamera.Distance = normal.distance;
                            topDownCamera.Angle = normal.angle;
                            _timer = normal.stayTime;
                            topDownCamera.UpdateTargetAndInitialize(normal.target);
                        });
                        FadeScreenManager.Instance.FadePanel.FadeOutIn(() =>
                        {
                            _pause = false;
                            _finishIntro = true;
                            CombatManager.Instance.Start = true;
                        });
                        return;
                    }

                    topDownCamera.Height = normal.height;
                    topDownCamera.Distance = normal.distance;
                    topDownCamera.Angle = normal.angle;
                    _timer = normal.stayTime;
                    topDownCamera.UpdateTargetAndInitialize(normal.target);

                    _finishIntro = true;
                    CombatManager.Instance.Start = true;
                }
            }
        }

        public void WinCamera()
        {
            if (win.fade)
            {
                _pause = true;
                FadeScreenManager.Instance.FadePanel.FadeDuration = 1.25f;
                FadeScreenManager.Instance.FadePanel.OnFinish.AddListener(() =>
                {
                    topDownCamera.Height = win.height;
                    topDownCamera.Distance = win.distance;
                    topDownCamera.Angle = win.angle;
                    _timer = win.stayTime;
                    topDownCamera.UpdateTargetAndInitialize(win.target);
                    CombatManager.Instance.Win();
                });
                FadeScreenManager.Instance.FadePanel.FadeOutIn(() =>
                {
                    _win = true;
                });
                return;
            }
        }
    }

}