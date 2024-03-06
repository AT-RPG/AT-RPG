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




    private void OnTriggerEnter(Collider other) //������ Ʈ���� �۵�
    {
     
        if ((mask & 1 << other.gameObject.layer) != 0) //�浹�� ���̾ ���Ͱ� ������ ���̾����� �Ǵ�
        {
            
            if (myTarget == null) //Ÿ���� �������
            {
               
                myTarget = other.transform; //Ÿ���� �浹����� ��ġ�� �ٲ۴�
                findPlayer?.Invoke(myTarget); //�Ŀ� findPlayer �Լ� ����
            }
        }
    }

    private void OnTriggerExit(Collider other) //�浹�� �����
    {
        if (myTarget == other.transform) //Ÿ���� �÷��̾� �������
        {
            myTarget = null; //Ÿ���� �ʱ�ȭ
            lostPlayer?.Invoke(); //lostPlayer�Լ� ����
        }
    }
}
