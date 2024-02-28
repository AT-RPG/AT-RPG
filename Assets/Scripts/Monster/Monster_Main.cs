using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterMain : MonoBehaviour  //������ ���,������ ������
{
    MonsterInfo mon1 = new MonsterInfo("��1",10.0f,15.0f,1,30.0f,0.5f, 30.0f);
   // MonsterInfo mon2 = new MonsterInfo();
    public enum State //���ѻ��¸ӽ� :: ���� -��� -�̵� -���� -���
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State m_State=State.Create;//���� �ʱ� ���¸� �������� ����

    Vector3 SpawnPos;  //������ ������ġ�� ����
    float MoveStartTime = 0.0f; //������ ���ð� 
    void ChangeState(State s) //���°� ���Ұ�� ���ѻ��¸� ���޹���
    {
        if (m_State == s) return; //���°� ������ �ʴ°�� ����ó��
         m_State = s; 
        switch (m_State)
        {
            case State.Idle: //���Ͱ� ������
                m_Idle();
                break;
            case State.Move: //���Ͱ� �����̴� ����
              
                m_Move();
                break;
            case State.Battle: //���Ͱ� ��������
                m_Battle();
                break;
            case State.Dead: //���Ͱ� ���
                m_Dead();
                break;
        }
    }


    void m_Idle()
    {
        MoveStartTime = Random.Range(2, 6); //���ð����� ��ȯ���� 2-6�� ����� �����̰� �ϱ�
        StartCoroutine(DelayChangeState(State.Move,MoveStartTime));
    }
    IEnumerator DelayChangeState(State s, float m_delaytime) //����������ŭ �������� ���¸� ���������� ����
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

    void m_Move()
    {

    }
    Vector3 GetRndPos()
    {
        Vector3 dir = Vector3.forward;
        dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
        dir *= Random.Range(0.0f, 3.0f);
        return SpawnPos + dir;
    }
    void m_Battle() 
    {

    }
    void m_Dead()
    {
        //�÷��̾�� ��带 �ְų� �������� ����� ���� ����
        //gold += Random.Range(1, 10);
        //StopAllCoroutines();//��� �ڷ�ƾ ����;
        Destroy(gameObject,10.0f*Time.deltaTime); //���Ͱ� ����Ѱ�� Ư������������ ��ü ����
    }


    // Start is called before the first frame update
    void Start()
    {
        SpawnPos= transform.position; //���۰� ���ÿ� ������ġ�� ���
        ChangeState(State.Idle); //���͸� �����·� ����
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
