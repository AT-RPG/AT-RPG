using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private FadeCanvasAnimation    fadeAnimation;
    [SerializeField] private PopupCanvasAnimation   popupAnimation;

    // Start is called before the first frame update
    void Start()
    {
        fadeAnimation.StartFade();
        popupAnimation.StartPopup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
