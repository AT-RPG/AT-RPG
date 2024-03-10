using UnityEngine;

public class Popup : MonoBehaviour
{
    protected bool isEscapePressed = false;

    // 팝업이 생성될 RectTransform 인스턴스
    protected PopupCanvas popupCanvas = null;

    public PopupCanvas PopupCanvas
    {
        get
        {
            return popupCanvas;
        }
        set
        {
            popupCanvas = value;
        }
    }
}
