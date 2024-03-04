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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거 엔터 진입");
        if ((mask & 1 << other.gameObject.layer) != 0)
        {
            Debug.Log("if문 1번째 진입");
            if (myTarget == null)
            {
                Debug.Log("마이타겟 null 진입");
                myTarget = other.transform;
                findPlayer?.Invoke(myTarget);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (myTarget == other.transform)
        {
            myTarget = null;
            lostPlayer?.Invoke();
        }
    }
}
