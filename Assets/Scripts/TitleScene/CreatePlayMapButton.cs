using UnityEngine;

namespace AT_RPG
{
    public class CreatePlayMapButton : MonoBehaviour
    {
        // 맵 플레이 버튼 인스턴스
        [SerializeField] private GameObject playMapButton;

        // 애니메이션 래퍼런스
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;

        private void Start()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
        }

        /// <summary>
        ///  맵 플레이 버튼을 생성
        /// </summary>
        /// <param name="scrollViewContents">mapButton이 생성될 RectTransform 인스턴스</param>
        public void OnButtonClick(GameObject scrollViewContents)
        {
            if (!playMapButton)
            {
                Debug.Log($"{nameof(playMapButton)}이 설정X");
            }

            Instantiate(playMapButton, scrollViewContents.transform);
        }
    }
}