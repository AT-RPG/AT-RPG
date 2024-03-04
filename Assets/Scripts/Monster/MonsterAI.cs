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
        Debug.Log("Ʈ���� ���� ����");
        if ((mask & 1 << other.gameObject.layer) != 0)
        {
            Debug.Log("if�� 1��° ����");
            if (myTarget == null)
            {
                Debug.Log("����Ÿ�� null ����");
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
