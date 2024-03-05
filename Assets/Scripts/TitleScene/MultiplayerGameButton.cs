using UnityEngine;

public class MultiplayerGameButton : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerGamePopup;

    /// <summary>
    /// 팝업을 생성 및 초기화
    /// </summary>
    /// <param name="popupCanvasInstance">팝업이 생성될 RectTransform 인스턴스</param>
    public void OnButtonClick(GameObject popupCanvasInstance)
    {
        if (!multiplayerGamePopup)
        {
            Debug.Log($"{nameof(multiplayerGamePopup)}이 설정X");
        }

        // 팝업 초기화
        Popup popup = multiplayerGamePopup.GetComponent<Popup>();
        if (!popup)
        {
            Debug.Log($"팝업 인스턴스에 {nameof(popup)} Component는 필수");
        }
        else
        {
            popup.PopupCanvas = popupCanvasInstance.GetComponent<PopupCanvas>();
            if (!popup.PopupCanvas)
            {
                Debug.Log($"팝업 캔버스 인스턴스에 {nameof(popup.PopupCanvas)} Component는 필수");
            }
        }

        Instantiate(multiplayerGamePopup, popupCanvasInstance.transform);
    }
}
