using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManagerSettings", menuName = "Scriptable Object/UIManagerSettings")]
public class UIManagerSettings : ScriptableObject
{
    [SerializeField] public GameObject screenFade;
}
