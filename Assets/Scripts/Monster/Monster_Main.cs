using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MonsterMain : MonoBehaviour  //������ ���,������ ������
{
    MonsterInfo mon1 = new MonsterInfo("��1",10.0f,15.0f,1,30.0f,0.5f, 30.0f);
    MonsterInfo mon2 = new MonsterInfo("��2", 10.0f, 30.0f, 1, 30.0f, 0.5f, 30.0f);
   
    public int monsterNum = 1;
    Coroutine move = null; //������ �������� ����
    Coroutine rotate = null; //������ ȸ���� ����
   
    public enum State //���ѻ��¸ӽ� :: ���� -��� -�̵� -���� -���
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState=State.Create;//���� �ʱ� ���¸� �������� ����

    Vector3 SpawnPos;  //������ ������ġ�� ����
    float MoveStartTime = 0.0f; //������ ���ð� 
    void ChangeState(State s) //���°� ���Ұ�� ���ѻ��¸� ���޹���
    {
        if (monsterState == s) return; //���°� ������ �ʴ°�� ����ó��
         monsterState = s;  //���º���
        switch (monsterState)
        {
            case State.Idle: //���Ͱ� ������
                monsterIdle();
                break;
            case State.Move: //���Ͱ� �����̴� ����
                monsterMove();
                break;
            case State.Battle: //���Ͱ� ��������
                monsterBattle();
                break;
            case State.Dead: //���Ͱ� ���
                monsterDead();
                break;
        }
    }


    void monsterIdle() //��� ����
    {
        MoveStartTime = Random.Range(2, 3); //���ð��� �������� �������� �̵��ϰ� �ϱ�
        StartCoroutine(DelayChangeState(State.Move,MoveStartTime));
    }
    IEnumerator DelayChangeState(State s, float m_delaytime) //����������ŭ �������� ���¸� ���������� ����
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

    void monsterMove() //�̵�����
    {
        Vector3 GetRndPos() //���Ͱ� ������ ���� ��������
        {
            Vector3 dir = Vector3.forward;
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir; //360
            dir *= Random.Range(10.0f, 50.0f); //������ �Ÿ�
            return SpawnPos + dir;   //��������ġ�κ��� �Ÿ��� ���Ѵ�
        }
        MoveToPos(GetRndPos());

    }
    void MoveToPos(Vector3 target) //Ư����ġ�� �̵�
    {
        if(move != null) //�̹� �̵����ΰ�� ������ �ڷ�ƾ�� ������Ŵ
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(monsterMoving(target)); //���ο� �̵��ڷ�ƾ
    }

    ///--------�̵����� �߰��ؾߵǴ� �κ�----------- <summary>
    /// --�÷��̾� �߽߰� Ÿ���� �÷��̾�� ����/�÷��̾� �߰��� �����Ÿ��̻� �־������ 
    /// �̵��ӵ��� �߰��� ������Ų�Ŀ� ������ ����� �޸���� ����
    /// �ӵ��� ���͸���  ���̵α�
   
    IEnumerator monsterMoving(Vector3 target)
    {
        Vector3 dir = target-transform.position; //��ǥ��ġ���� ������
        float dist = dir.magnitude; //��ǥ��ġ���� �Ÿ����
        dir.Normalize(); //����ȭ

        if (rotate != null) StopCoroutine(rotate); //����ȸ�� �ڷ�ƾ ����
        rotate = StartCoroutine(Rotating(dir)); //���ο� ȸ���ڷ�ƾ ����

        while (!Mathf.Approximately(dist, 0.0f))
        {
            
            float delta = mon1.monsterMoveSpeed * Time.deltaTime; 
             
            if (delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World); //�̵�
            
            yield return null;
        }
        ChangeState(State.Idle); //���͸� �����·� ����
        
    }
    IEnumerator Rotating(Vector3 dir) //���͸� �־��� �������� ȸ��
    {
        float angle = Vector3.Angle(transform.forward, dir); //�������� ��ǥ�����
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)//ȸ�� ���� ����
        {
            rotDir = -1.0f;
        }

        while (!Mathf.Approximately(angle, 0.0f))
        {
            float delta = 360.0f * Time.deltaTime;
            if (delta > angle)
            {
                delta = angle;
            }
            angle -= delta;
            transform.Rotate(Vector3.up * rotDir * delta); //ȸ��
            yield return null;
        }
    }





        void monsterBattle()  //��������
    {

    }
    void monsterDead() //�������
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
