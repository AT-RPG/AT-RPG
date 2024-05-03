using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageClick : MonoBehaviour, IPointerClickHandler
{
    public Image emptyImage; // 빈 이미지에 대한 참조
    //public TextMeshProUGUI priceText; // TextMeshPro로 변경된 변수 타입
    public TextMeshProUGUI imageName; // TextMeshPro로 변경된 변수 타입

    public void SetEmptyImage(Image emptyImage)
    {
        this.emptyImage = emptyImage;
    }
    public void SetPriceText(TextMeshProUGUI priceText) // TextMeshProUGUI로 변경
    {
        this.priceText = priceText;
    }
    /*public void SetImageName(TextMeshProUGUI imageName) // TextMeshProUGUI로 변경
    {
        this.imageName = imageName;
    }*/

    public void OnPointerClick(PointerEventData eventData)
    {
        Image clickedImage = GetComponent<Image>();
        TextMeshProUGUI clikedPricetext = GetComponentInChildren<TextMeshProUGUI>(); // 변경된 부분
       // TextMeshProUGUI clickedImagetext = GetComponentInChildren<TextMeshProUGUI>(); // 변경된 부분
        if (clickedImage != null && emptyImage != null)
        {
            emptyImage.sprite = clickedImage.sprite;
            priceText.text = clikedPricetext.text;
            //imageName.text = clickedImagetext.text;
        }
    }
}
