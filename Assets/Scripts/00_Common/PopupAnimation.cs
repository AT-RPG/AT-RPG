using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Rendering;

namespace AT_RPG
{
    public class PopupAnimation : MonoBehaviour
    {
        public Volume               volume;
        public float                backgroundBlurEventDuration;
        public RectTransform        PopupTransform;
        public CanvasGroup          PopupCanvasGroup;
        public Vector2              StartPosition = new Vector2(0, -Screen.height);
        public Vector2              EndPosition = Vector2.zero;

        public float                PopupDuration = 0.5f;
        public float                FadeDuration = 0.5f;

        private void Awake()
        {
            PopupTransform.anchoredPosition = StartPosition;
            PopupCanvasGroup.alpha = 0;
        }

        /// <summary>
        /// 1. 페이드인 효과 적용
        /// 2. 위로 올라오는 애니메이션 적용
        /// </summary>
        public void ShowPopup()
        {
            PopupTransform.gameObject.SetActive(true);
            PopupTransform.DOAnchorPos(EndPosition, PopupDuration).SetEase(Ease.OutBack);
            PopupCanvasGroup.DOFade(1, PopupDuration); 
        }

        /// <summary>
        /// 1. 페이드 아웃 효과 적용
        /// 2. 아래로 내려가는 애니메이션 적용
        /// </summary>
        public void HidePopup()
        {
            PopupTransform.DOAnchorPos(StartPosition, PopupDuration).SetEase(Ease.InBack);
            PopupCanvasGroup.DOFade(0, PopupDuration);
        }

        /// <summary>
        /// 백그라운드 캔버스에 블러 효과를 시작
        /// </summary>
        public void StartBackgroundBlur()
        {
            StartCoroutine(FadeInBlur(backgroundBlurEventDuration));
        }

        private IEnumerator FadeInBlur(float duration)
        {
            float t = 0f;

            while (t <= backgroundBlurEventDuration)
            {
                volume.weight = Mathf.Lerp(volume.weight, 1, t / backgroundBlurEventDuration);
                t += Time.deltaTime;

                yield return null;
            }
        }

        /// <summary>
        /// 백그라운드 캔버스에 블러 효과를 종료
        /// </summary>
        public void EndBackgroundBlur()
        {
            StartCoroutine(FadeOutBlur(backgroundBlurEventDuration));
        }

        private IEnumerator FadeOutBlur(float duration)
        {
            float t = 0f;

            while (t <= backgroundBlurEventDuration)
            {
                volume.weight = Mathf.Lerp(volume.weight, 0, t / backgroundBlurEventDuration);
                t += Time.deltaTime;

                yield return null;
            }
        }
    }
}