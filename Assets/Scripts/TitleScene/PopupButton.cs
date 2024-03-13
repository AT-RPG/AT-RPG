using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public class PopupButton : MonoBehaviour, IPointerClickHandler
    {
        // 인스턴싱할 팝업
        [SerializeField] private GameObject popupInstance;
        [SerializeField] private GameObject popupCanvasInstance;

        /// <summary>
        /// 버튼 클릭 시 팝업을 생성
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 팝업 초기화
            Popup popup = popupInstance.GetComponent<Popup>();
            PopupCanvas popupCanvas = popupCanvasInstance.GetComponent<PopupCanvas>();
            popup.PopupCanvas = popupCanvas;

            Instantiate(popupInstance, popupCanvas.Root.transform);
        }

        /// <summary>
        /// 팝업을 생성하고 초기화
        /// </summary>
        public void OnInstantiatePopup()
        {

        }
    }
}
