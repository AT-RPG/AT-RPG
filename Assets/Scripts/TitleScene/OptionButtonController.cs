using UnityEngine;

public class OptionButtonController : MonoBehaviour
{
    [SerializeField] private GameObject optionPopup;

    public void OnButtonClick(RectTransform rectTransform)
    {
        if (!optionPopup)
        {
            Debug.Log($"{nameof(optionPopup)}이 설정X");
        }

        Instantiate(optionPopup, rectTransform);
    }
}
