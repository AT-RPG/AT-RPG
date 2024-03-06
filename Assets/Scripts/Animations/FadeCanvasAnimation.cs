using UnityEngine;
using DG.Tweening;
using System;

namespace AT_RPG
{
    public class FadeCanvasAnimation : MonoBehaviour
    {
        // 페이드 애니메이션 타겟
        public CanvasGroup TargetCanvasGroup;

        public float StartAlpha = 1.0f;

        public float EndAlpha = 1.0f;

        // 페이드 애니메이션이 몇 초안에 이루어지는가?
        public float Duration = 0.5f;

        /// <summary>
        /// 캔버스 그룹 점차 나타나게
        /// </summary>
        public void StartFade(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            TargetCanvasGroup.alpha = StartAlpha;
            TargetCanvasGroup.DOFade(EndAlpha, Duration).onComplete += tweenCompleteCallback;
        }

        /// <summary>
        /// 캔버스 그룹 점차 사라지게
        /// </summary>
        public void EndFade(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            TargetCanvasGroup.alpha = EndAlpha;
            TargetCanvasGroup.DOFade(StartAlpha, Duration).onComplete += tweenCompleteCallback;
        }
    }
}
