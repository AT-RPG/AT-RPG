using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private Slider mySlider;
    [SerializeField] private Image fillImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBar(float v)
    {
        mySlider.value = v;

        if(mySlider.value >= 0.7f)
        {
            fillImage.color = Color.green;
        }
        else if(mySlider.value >= 0.4f && mySlider.value < 0.7f)
        {
            fillImage.color = Color.yellow;
        }
        else if(mySlider.value >= 0.0f && mySlider.value < 0.4f)
        {
            fillImage.color = Color.red;
        }
    }
}
