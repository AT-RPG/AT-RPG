using UnityEngine;
using DG.Tweening;

namespace AT_RPG
{
    public class PopupAnimation : MonoBehaviour
    {
        public RectTransform        PopupTransform;
        public CanvasGroup          PopupCanvasGroup;
        public Vector2              StartPosition = new Vector2(0, -Screen.height);
        public Vector2              EndPosition = Vector2.zero;

        public float                PopupDuration = 0.5f;
        public float                FadeDuration = 0.5f;

        void Start()
        {
            PopupTransform.anchoredPosition = StartPosition;
            PopupCanvasGroup.alpha = 0;

            ShowPopup();
        }

        public void ShowPopup()
        {
            // 팝업을 보이게 함
            PopupTransform.gameObject.SetActive(true);
            // 위로 올라오는 애니메이션 적용
            PopupTransform.DOAnchorPos(EndPosition, PopupDuration).SetEase(Ease.OutBack);
            // 페이드인 효과 적용
            PopupCanvasGroup.DOFade(1, PopupDuration); // 투명도를 0에서 1로 변경
        }

        public void HidePopup()
        {
            // 팝다운 애니메이션 적용
            PopupTransform.DOAnchorPos(StartPosition, PopupDuration).SetEase(Ease.InBack);
            // 페이드아웃 효과 적용
            PopupCanvasGroup.DOFade(0, PopupDuration).OnComplete(() => {
                // 애니메이션이 완료된 후 팝업을 비활성화
                PopupTransform.gameObject.SetActive(false);
            });
        }
    }
}