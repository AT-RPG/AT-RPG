using UnityEngine;

public class QuitButtonController : MonoBehaviour
{
    [SerializeField] private GameObject quitGamePopup;

    public void OnButtonClick(RectTransform rectTransform)
    {
        if (!quitGamePopup)
        {
            Debug.Log($"{nameof(quitGamePopup)}이 설정X");
        }

        Instantiate(quitGamePopup, rectTransform);
    }
}
