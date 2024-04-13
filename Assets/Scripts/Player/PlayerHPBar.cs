using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private Slider mySlider;
    [SerializeField] private Image fillImage;
    private bool isTweening = false;
    
    public void ChangeBar(float v)
    {
        if (!isTweening)
        {
            isTweening = true;
            float currentValue = mySlider.value;
            DOTween.To(() => currentValue, x => currentValue = x, v, 0.5f)
                .OnUpdate(() => {
                    mySlider.value = currentValue;
                    UpdateFillColor(currentValue);
                })
                .OnComplete(() => { isTweening = false; });
        }
    }

    private void UpdateFillColor(float value)
    {
        if(value >= 0.7f)
        {
            fillImage.color = Color.green;
        }
        else if(value >= 0.4f && value < 0.7f)
        {
            fillImage.color = Color.yellow;
        }
        else if(value >= 0.0f && value < 0.4f)
        {
            fillImage.color = Color.red;
        }
    }
}
