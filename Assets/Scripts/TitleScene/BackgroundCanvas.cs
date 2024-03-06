using AT_RPG;
using UnityEngine;

public class BackgroundCanvas : MonoBehaviour
{
    [SerializeField] private FadeCanvasAnimation    fadeAnimation;
    [SerializeField] private PopupCanvasAnimation   popupAnimation;

    // Start is called before the first frame update
    void Start()
    {
        fadeAnimation.StartFade();
        popupAnimation.StartPopup();
    }
}
