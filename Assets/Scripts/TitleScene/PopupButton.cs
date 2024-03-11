using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public class PopupButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject popupInstance;
        [SerializeField] private GameObject popupCanvasInstance;

        /// <summary>
        /// 버튼 클릭 시 팝업을 생성
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!popupInstance)
            {
                Debug.LogError($"{nameof(popupInstance)}이 설정X");
                return;
            }

            if (!popupCanvasInstance)
            {
                Debug.LogError($"{popupInstance}를 생성할 {popupCanvasInstance}가 정해지지 않았습니다.");
            }

            // 팝업 초기화
            Popup popup = popupInstance.GetComponent<Popup>();
            PopupCanvas popupCanvas = popupCanvasInstance.GetComponent<PopupCanvas>();
            popup.PopupCanvas = popupCanvas;

            Instantiate(popupInstance, popupCanvas.Root.transform);
        }
    }
}
