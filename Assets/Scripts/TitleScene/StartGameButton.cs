using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject     startGamePopup;
    [SerializeField] private GameObject     popupCanvasRoot;

    /// <summary>
    /// 버튼 클릭 시 팝업을 생성
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!startGamePopup)
        {
            Debug.Log($"{nameof(startGamePopup)}이 설정X");
        }

        // 팝업 초기화
        Popup popup = startGamePopup.GetComponent<Popup>();
        if (!popup)
        {
            Debug.Log($"팝업 인스턴스에 {nameof(popup)} Component는 필수");
        }
        else
        {
            popup.PopupCanvas = popupCanvasRoot.GetComponent<PopupCanvas>();
            if (!popup.PopupCanvas)
            {
                Debug.Log($"팝업 캔버스 인스턴스에 {nameof(popup.PopupCanvas)} Component는 필수");
            }
        }

        Instantiate(startGamePopup, popupCanvasRoot.transform);
    }
}
