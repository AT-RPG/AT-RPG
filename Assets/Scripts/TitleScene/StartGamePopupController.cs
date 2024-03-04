using AT_RPG;
using UnityEngine;

public class StartGamePopupController : MonoBehaviour
{
    [SerializeField] private FadeCanvasAnimation fadeAnimation;
    [SerializeField] private PopupCanvasAnimation popupAnimation;
    [SerializeField] private BlurCanvasAnimation blurAnimation;

    private void Start()
    {
        fadeAnimation.StartFade();
        popupAnimation.StartPopup();
        blurAnimation.StartFade();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popupAnimation.EndPopup();
            fadeAnimation.EndFade(() =>
            {
                Destroy(gameObject);
            });                 
            blurAnimation.EndFade();
        }
    }
}
