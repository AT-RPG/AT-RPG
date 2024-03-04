using UnityEngine;

public class MultiplayerGameButtonController : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerGamePopup;

    public void OnButtonClick(RectTransform rectTransform)
    {
        if (!multiplayerGamePopup)
        {
            Debug.Log($"{nameof(multiplayerGamePopup)}이 설정X");
        }

        Instantiate(multiplayerGamePopup, rectTransform);
    }
}
