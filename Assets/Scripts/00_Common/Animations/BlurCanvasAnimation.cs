using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace AT_RPG
{
    public class BlurCanvasAnimation : MonoBehaviour
    {
        // 포스트 프로세싱 인스턴스
        public Volume TargetVolume;

        // 블러 애니메이션이 몇 초안에 이루어지는가?
        public float Duration = 0.5f;

        /// <summary>
        /// 블러 점차 나타나게
        /// </summary>
        public void StartFade(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            float startWeight = TargetVolume.weight;

            DOVirtual.Float(startWeight, 1, Duration, value =>
            {
                TargetVolume.weight = value;
            }).SetEase(Ease.Linear).onComplete += tweenCompleteCallback;
        }

        /// <summary>
        /// 블러 점차 사라지게
        /// </summary>
        public void EndFade(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            float startWeight = TargetVolume.weight;

            DOVirtual.Float(startWeight, 0, Duration, value =>
            {
                TargetVolume.weight = value;
            }).SetEase(Ease.Linear).onComplete += tweenCompleteCallback;
        }
    }
}