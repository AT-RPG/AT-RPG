using UnityEngine;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private GameObject quitGamePopup;

    /// <summary>
    /// 팝업을 생성 및 초기화
    /// </summary>
    /// <param name="popupCanvasInstance">팝업이 생성될 RectTransform 인스턴스</param>
    public void OnButtonClick(GameObject popupCanvasInstance)
    {
        if (!quitGamePopup)
        {
            Debug.Log($"{nameof(quitGamePopup)}이 설정X");
        }

        // 팝업 초기화
        Popup popup = quitGamePopup.GetComponent<Popup>();
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

        Instantiate(quitGamePopup, popupCanvasInstance.transform);
    }
}
