using AT_RPG;
using UnityEngine;
using UnityEngine.Rendering;

public class StartGamePopup : MonoBehaviour
{
    [Header("팝업 설정")]
    [SerializeField] PopupAnimation popupAnimation;

    private void Start()
    {
        popupAnimation.StartBackgroundBlur();
        popupAnimation.ShowPopup();
    }

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
       {
            popupAnimation.EndBackgroundBlur();
            popupAnimation.HidePopup();
        }
    }
}
