using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterAI : MonoBehaviour
{
    public UnityEvent<Transform> findPlayer;
    public UnityEvent lostPlayer;
    public LayerMask mask;
    public Transform myTarget;

    private void OnTriggerEnter(Collider other) //콜라이더 충돌감지
    {
     
        if ((mask & 1 << other.gameObject.layer) != 0) 
        {
            
            if (myTarget == null) 
            {
               
                myTarget = other.transform; 
                findPlayer?.Invoke(myTarget); 
            }
        }
    }

    private void OnTriggerExit(Collider other) //콜라이더 충돌해제
    {
        if (myTarget == other.transform) 
        {
            myTarget = null; 
            lostPlayer?.Invoke(); 
        }
    }
}
