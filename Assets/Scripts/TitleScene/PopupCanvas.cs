using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCanvas : MonoBehaviour
{
    private Stack<Popup> popups
        = new Stack<Popup>();

    public Stack<Popup> Popups
    {
        get
        {
            return popups;
        }
        set
        {
            popups = value;
        }
    }
}
