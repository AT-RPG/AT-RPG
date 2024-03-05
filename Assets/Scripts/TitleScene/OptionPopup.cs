using AT_RPG;
using UnityEngine;

public class OptionPopup : Popup
{
    [SerializeField] private FadeCanvasAnimation fadeAnimation;
    [SerializeField] private PopupCanvasAnimation popupAnimation;
    [SerializeField] private BlurCanvasAnimation blurAnimation;

    private void Awake()
    {
        if (!popupCanvas)
        {
            return;
        }

        popupCanvas.Popups.Push(this);
    }

    private void Start()
    {
        fadeAnimation.StartFade();
        popupAnimation.StartPopup();
        blurAnimation.StartFade();
    }

    private void Update()
    {
        OnEscapeKeyPressed();
    }

    private void OnEscapeKeyPressed()
    {
        // 종료 버튼이 눌리면
        if (!isEscapePressed &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            // 팝업 캔버스에 등록된 경우, Stack(순차적으로 팝업 종료)이용
            if (popupCanvas)
            {
                if (popupCanvas.Popups.Pop().GetInstanceID() == GetInstanceID())
                {
                    Animate();
                }
                else
                {
                    return;
                }
            }
            // 팝업 단일로 이용하는 경우
            else
            {
                Animate();
            }
        }
    }

    private void Animate()
    {
        isEscapePressed = true;

        popupAnimation.EndPopup();
        fadeAnimation.EndFade(() =>
        {
            Destroy(gameObject);
        });
        blurAnimation.EndFade();
    }
}
