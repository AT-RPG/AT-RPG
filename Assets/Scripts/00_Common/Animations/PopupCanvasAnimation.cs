using System;
using UnityEngine;
using DG.Tweening;

namespace AT_RPG
{
    public class PopupCanvasAnimation : MonoBehaviour
    {
        // 팝업 애니메이션 타겟
        public RectTransform        TargetTransform;

        // 팝업 애니메이션 시작 시 위치
        public Vector2              StartPosition = new Vector2(0, -Screen.height);

        // 팝업 애니메이션 종료 시 위치
        public Vector2              EndPosition = Vector2.zero;

        // 팝업 애니메이션이 몇 초안에 이루어지는가?
        public float                Duration = 0.5f;


        /// <summary>
        /// 위로 올라오는 애니메이션 적용
        /// </summary>
        public void StartPopup(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            TargetTransform.anchoredPosition = StartPosition;
            TargetTransform.DOAnchorPos(EndPosition, Duration).SetEase(Ease.OutBack).onComplete
                += tweenCompleteCallback; ;
        }

        /// <summary>
        /// 아래로 내려가는 애니메이션 적용
        /// </summary>
        public void EndPopup(Action completeCallback = null)
        {
            TweenCallback tweenCompleteCallback = () => completeCallback?.Invoke();

            TargetTransform.anchoredPosition = EndPosition;
            TargetTransform.DOAnchorPos(StartPosition, Duration).SetEase(Ease.InBack).onComplete
                += tweenCompleteCallback;
        }
    }
}