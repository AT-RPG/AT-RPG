using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> fallingAct;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            fallingAct?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            fallingAct?.Invoke(false);
        }
    }
}
