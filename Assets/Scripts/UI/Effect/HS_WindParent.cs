using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_WindParent : MonoBehaviour
{
    [SerializeField] private GameObject[] winds;

    private void OnEnable() 
    {
        foreach (var wind in winds)
        {
            wind.SetActive(true);
        }
    }
}
