using UnityEngine;

public class StartGameButtonController : MonoBehaviour
{
    [SerializeField] private GameObject     startGamePopup;

    public void OnButtonClick(RectTransform rectTransform)
    {
        if (!startGamePopup)
        {
            Debug.Log($"{nameof(startGamePopup)}이 설정X");
        }

        Instantiate(startGamePopup, rectTransform);
    }
}
