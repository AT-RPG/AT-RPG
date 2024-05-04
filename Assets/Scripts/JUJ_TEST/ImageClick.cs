using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageClick : MonoBehaviour, IPointerClickHandler
{
    public Image emptyImage; // 빈 이미지에 대한 참조
    public GameObject PopupPnael;
    public void SetEmptyImage(Image emptyImage)
    {
        this.emptyImage = emptyImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PopupPnael.SetActive(true);
        Image clickedImage = GetComponent<Image>();
        if (clickedImage != null && emptyImage != null)
        {
            emptyImage.sprite = clickedImage.sprite;
        }

    }
}