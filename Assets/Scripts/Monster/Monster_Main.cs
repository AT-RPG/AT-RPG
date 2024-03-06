using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

//�ϴ� �����ϰ� �����Ǹ� ���� ��ũ��Ʈ �и��ϱ�-----------
public class MonsterMain :  MonsterPorperty //�ִϸ����͸� �������� �ִϸ����� ���� ��ũ��Ʈ���
{
    [SerializeField] Transform monResPos;
    MonsterInfo mon1 = new MonsterInfo("��1",10.0f,3.0f,1,30.0f,0.5f, 30.0f); //���� ���� ���� --��� �����ؾ���
                                                                             //������� ������ �ſ�������Ƿ� ���͸��� ���� ������ ������ ������ ����ϰԲ� ����

    public GameObject Imp; //���� �������� ���� ���ӿ�����Ʈ ����
    
    Coroutine move = null; //������ �������� ����
    Coroutine rotate = null; //������ ȸ���� ����

    public MonsterAI monsterAI; //���� ai����� ����
    private bool isTracking = false;     // ���Ͱ� ���� ������ �Ǵ�

    bool runCheck = false;  //���Ͱ� �޸��� �ִϸ��̼��� ����ϴ� �Ÿ��Ǵ�
    

    public enum State //���ѻ��¸ӽ� :: ���� -��� -�̵� -���� -���
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState=State.Create;//���� �ʱ� ���¸� �������� ����

    //Vector3 SpawnPos = new Vector3(30.0f, 1.0f, 30.0f);  //������ ������ġ����
    float moveStartTime = 0.0f; //������ ���ð� 
    float monsterSpwanTimer = 2.0f;//���� ���������ð�
    Transform monsterTarget; //���������϶� �÷��̾ ����
    void ChangeState(State s) //���°� ���Ұ�� ���ѻ��¸� ���޹���
    {
        if (monsterState == s) return; //���°� ������ �ʴ°�� ����ó��
         monsterState = s;  //���º���
        switch (monsterState)
        {
            case State.Create:  
                monsterAI.findPlayer.AddListener(StartTracking); //����AI ��ũ��Ʈ�� findPlayer�� �߻��Ұ�� StartTracking �޼��带 ȣ��
                monsterAI.lostPlayer.AddListener(StopTracking);  //�÷��̾ ��������� ���º���
                monsterCreate();
                break;

            case State.Idle: //���Ͱ� ������
                monsterAnim.SetBool("Run", false);
                monsterAnim.SetBool("Move", false); //������ �̵����� �ִϸ��̼��� ���
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
    void monsterCreate()//���� ����-���Ͱ� ������� �����ð����� �ٽ� �罺����Ŵ
    {
        Debug.Log("���� ������ ����");
        Instantiate(Imp); //������ �纻�� �����Ѵ�
        transform.position = monResPos.position; //���͸� ������ġ�� �̵���Ų��
        ChangeState(State.Idle);//������ ���¸� ���� ����
    }
    void monsterIdle() //��� ����
    {
        if (isTracking == false) //�������°� �ƴҰ��
        {
            moveStartTime = Random.Range(2, 3); //���ð��� �������� �������� �̵��ϰ� �ϱ�
            StartCoroutine(DelayChangeState(State.Move, moveStartTime));
        }
        else ChangeState(State.Move);//���������� ��� ������ ��� �̵����·� ��ȯ
    }
    IEnumerator DelayChangeState(State s, float m_delaytime) //����������ŭ ������� ���¸� ���������� ����
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

    //----------��������-------------------------------
    public void StartTracking(Transform target) //�÷��̾� �߰�
    {
        isTracking = true;
       
        StopCoroutine(move); //������ �����ϸ� ������ �������̴� �̵��ڷ�ƾ ����
        monsterMove();//�̵��Լ� ���ȣ��-�÷��̾ ��� ������
        monsterTarget = target; //������Ȳ�� �÷��̾��������� 
    }

    public void StopTracking() //�÷��̾� ��ħ
    {
        isTracking = false;
        
    }

    //---------------�̵�����---------------------------------------------------
    void monsterMove() //�̵�����
    {

        Vector3 dir = Vector3.forward;

        Vector3 GetRndPos() //���Ͱ� ������ ���� ��������
        {
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir; //360
            dir *= Random.Range(10.0f, 30.0f); //������ �Ÿ�
            return monResPos.position + dir;   //��������ġ�κ��� �Ÿ��� ���Ѵ�
        }

        Vector3 GetTrackingPos() // �÷��̾��� ��ġ�� ��������
        {
            dir = monsterAI.myTarget.position - transform.position;
            return transform.position + dir; // ���� ��ġ���� �÷��̾� �������� �̵�
        }


        if (isTracking)  //�÷��̾��� �������ο����� ���Ͱ� �����Ϲ����� �����ش�
        {
            MoveToPos(GetTrackingPos());
            
        }
        else 
        {
            MoveToPos(GetRndPos());
         
        }

    }

    void MoveToPos(Vector3 target) //Ư����ġ�� �̵�
    {
        if(move != null) //�̹� �̵����ΰ�� ������ �ڷ�ƾ�� ������Ŵ
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(monsterMoving(target)); //���ο� �̵��ڷ�ƾ
    }

   
    IEnumerator monsterMoving(Vector3 target)
    {
        Vector3 dir = target-transform.position; //��ǥ��ġ���� ������
        float dist = dir.magnitude; //��ǥ��ġ���� �Ÿ����
        dir.Normalize(); //����ȭ

        //-----�޸��� ���� ��ũ��Ʈ------
        bool isRunning = dist >= 40.0f; // �Ÿ��� ���� �޸��� ���� �Ǵ�
        if (isRunning) mon1.monsterMoveSpeed += 3.0f;//�޸��� �߻��� �̵��ӵ� ����
        monsterAnim.SetBool("Move", !isRunning);
        monsterAnim.SetBool("Run", isRunning);

        //--��Ʋ ���� ���� ��ũ��Ʈ--
        if (isTracking)//�÷��̾ �������̰�
        {
            if (dist < mon1.mRange)//�÷��̾���� �Ÿ��� 1 �̸��̸�
            {
                //   ChangeState(State.Battle); //���¸� ������ ��� ��ȯ�Ѵ�
                ChangeState(State.Dead); //���� ������ ����//////////
            }
        }

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
    //----------------------------------------------------------------------------------------------



    //-------���� ���� ��ũ��Ʈ ----------------------------------------------
    void monsterBattle()  //��������
    {
        //---�ִϸ��̼��� ������ ����
        monsterAnim.SetBool("Move", false); 
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", true);







    }

  
    /// ------��� ���� ��ũ��Ʈ---------------------------------------------------
    ///������ ���+�����+�÷��̾�� �ִ� ����ġ ��Ұ� �߰��Ǿ���� <summary>
    /// ���������� ��Ʈ���̰� ������� �������� �����ʴ� �������
    /// </summary>
    void monsterDead() //�������
    {
        StopAllCoroutines();//��� �ڷ�ƾ ����;
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", false);
        monsterAnim.SetTrigger("Dead"); //��� �ִϸ��̼� ����
        StartCoroutine(DelaySpwanState(State.Create, monsterSpwanTimer)); //�����ð� ������� �������·� ��ȯ��Ų��
        Destroy(gameObject,200.0f*Time.deltaTime); //���Ͱ� ����Ѱ�� Ư������������ ��ü ����
    }
    IEnumerator DelaySpwanState(State s, float m_delaytime) //����������ŭ ������� ���¸� �������� ����
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

  
    // Start is called before the first frame update
    void Start()
    {
        //SpawnPos= transform.position; //���۰� ���ÿ� ������ġ�� ���
        ChangeState(State.Idle); //���͸� �����·� ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
