using UnityEngine;
using UnityEngine.EventSystems;

public class PopupButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject popupInstance;
    [SerializeField] private GameObject popupCanvasRoot;

    /// <summary>
    /// 버튼 클릭 시 팝업을 생성
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!popupInstance)
        {
            Debug.Log($"{nameof(popupInstance)}이 설정X");
        }

        // 팝업 초기화
        Popup popup = popupInstance.GetComponent<Popup>();
        if (!popup)
        {
            Debug.Log($"{nameof(popupInstance)}에 {nameof(popup)} Component는 필수");
        }
        else
        {
            popup.PopupCanvas = popupCanvasRoot.GetComponent<PopupCanvas>();
            if (!popup.PopupCanvas)
            {
                Debug.Log($"{nameof(popupCanvasRoot)}에 {nameof(popup.PopupCanvas)} Component는 필수");
            }
        }

        Instantiate(popupInstance, popupCanvasRoot.transform);
    }
}
