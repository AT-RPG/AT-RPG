using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ŭ���������� �����丵->��Ʋ����->����
/// </summary>
public class MonsterMain : MonsterPorperty //�ִϸ����͸� �������� �ִϸ����� ���� ��ũ��Ʈ���
{
    [SerializeField] Transform monResPos;
    MonsterInfo mon1 = new MonsterInfo("��1", 10.0f, 3.0f, 1, 30.0f, 0.5f, 10.0f); //���� ���� ���� --��� �����ؾ���
                                                                                  //������� ������ �ſ�������Ƿ� ���͸��� ���� ������ ������ ������ ����ϰԲ� ����

    public GameObject Imp; //���� �������� ���� ���ӿ�����Ʈ ����

    Coroutine move = null; //������ �������� ����
    Coroutine rotate = null; //������ ȸ���� ����
    Coroutine chasing = null; //������ ������ ����


    public MonsterAI monsterAI; //���� ai����� ����
    private bool isTracking = false;

    public enum State //���ѻ��¸ӽ� :: ���� -��� -�̵� -���� -���
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState = State.Create;//���� �ʱ� ���¸� �������� ����

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
                monsterCreate();
                break;
            case State.Idle: //���Ͱ� ������
                monsterIdle();
                break;
            case State.Move: //���Ͱ� �����̴� ����
                monsterMove();
                break;
            case State.Battle: //���Ͱ� ��������
                StartCoroutine(monsterBattle(monsterTarget.position));
                break;
            case State.Dead: //���Ͱ� ���
                monsterDead();
                break;
        }
    }
    void monsterCreate()//���� ����-���Ͱ� ������� �����ð����� �ٽ� �罺����Ŵ
    {
        Instantiate(Imp); //������ �纻�� �����Ѵ�
        transform.position = monResPos.position; //���͸� ������ġ�� �̵���Ų��
        ChangeState(State.Idle);//������ ���¸� ���� ����
    }

    /// <summary>
    /// ������ ������
    /// �÷��̾ �������ϰ�� ������ ��� �̵����·���ȯ/�ƴҰ�� �����ð� �������Ŀ� �̵����·� ��ȯ
    /// </summary>
    void monsterIdle()
    {
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Move", false);

        moveStartTime = Random.Range(2, 4);
        Debug.Log($"{moveStartTime}�̵���ǥ ����");
        StartCoroutine(DelayChangeState(State.Move, moveStartTime));
    }
    IEnumerator DelayChangeState(State s, float m_delaytime)
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }


    /// <summary>
    /// �̵����� ���� �Լ�
    /// </summary>
    void monsterMove()
    {
        Vector3 dir = Vector3.forward;
        monsterDirection(dir);
    }

    void monsterDirection(Vector3 dir)//������ �ι� ��ǥ ����
    {
        Vector3 GetRndPos()
        {
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
            dir *= Random.Range(10.0f, 30.0f);
            return monResPos.position + dir;
        }
        MoveToPos(GetRndPos());
    }
    void MoveToPos(Vector3 target) //��ǥ�� �̵�����
    {
        if (move != null) //�̹� �̵����ΰ�� ������ �ڷ�ƾ�� ������Ŵ
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(monsterMoving(target)); //���ο� �̵��ڷ�ƾ
    }

    void IsRunning(float dist)//�޸���
    {
        float runOk = 40.0f;
        if (dist >= runOk)
        {
            mon1.monsterMoveSpeed = 6.0f;
            monsterAnim.SetBool("Move", false);
            monsterAnim.SetBool("Run", true);
        }
        else
        {
            mon1.monsterMoveSpeed = 3.0f;
            monsterAnim.SetBool("Run", false);
            monsterAnim.SetBool("Move", true);
        }

    }
    IEnumerator monsterMoving(Vector3 target)//������ �̵�
    {
        //��ǥ ��ġ������ ����� �Ÿ������ ����ȭ
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        //�Ÿ��� ���� �޸���/���� �Ǵ�
        //if (dist <= mon1.mRange)

        //ȸ���ڷ�ƾ�� ����/�����
        if (rotate != null) StopCoroutine(rotate);
        rotate = StartCoroutine(Rotating(dir));


        while (dist > 0.1f)
        {
            float delta = mon1.monsterMoveSpeed * Time.deltaTime;
            if (delta > dist) delta = dist;
            dist -= delta;
            IsRunning(dist);
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        if (isTracking == false)
        {
            ChangeState(State.Idle);
        }
    }
    IEnumerator Rotating(Vector3 dir) //���͸� ȸ��
    {
        float angle = Vector3.Angle(transform.forward, dir); //�������� ��ǥ�����
        float rotDir = 1.0f;
        //ȸ�� ���� ����
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -1.0f;
        }
        //ȸ�� ����
        while (!Mathf.Approximately(angle, 0.0f))
        {
            float delta = 360.0f * Time.deltaTime;
            if (delta > angle)
            {
                delta = angle;
            }
            angle -= delta;
            transform.Rotate(Vector3.up * rotDir * delta);
            yield return null;
        }
    }

    /// <summary>
    /// ������ �������� �Լ�
    /// </summary>
    public void StartTracking(Transform target)
    {
        monsterTarget = target;
        StopAllCoroutines();
        ChangeState(State.Battle);
    }
    public void StopTracking()
    {
        monsterTarget = null;
        StopAllCoroutines();
        ChangeState(State.Idle);
    }


    /// <summary>
    /// -------���� ���� ��ũ��Ʈ ----------------------------------------------
    /// Ÿ���� �������Ǹ� Ÿ���� ��ǥ�� �����Ŀ� ��Ʋ�Լ� ȣ��
    /// ���� Ÿ�ٰ� �Ÿ��� ��鼭 ��Ÿ����� �ָ������������� �̵��Լ� ȣ��
    /// ���� Ÿ���� ��Ÿ��ȿ� ������ ����
    /// </summary>


    IEnumerator monsterBattle(Vector3 battletarget)  //��������
    {
      
        Vector3 dir = battletarget - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        while (mon1.mRange < dist)
        {
            MoveToPos(battletarget);
            //dir = battletarget - transform.position;
            //dist = dir.magnitude;
            yield return null;
        }
    }
    

    /// ------��� ���� ��ũ��Ʈ---------------------------------------------------
    ///������ �����ڷ� ������ ��ũ��Ʈ�� �и��Ұ�
    /// </summary>
    void monsterDead() //�������
    {
        StopAllCoroutines();//��� �ڷ�ƾ ����;
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", false);
        monsterAnim.SetTrigger("Dead"); //��� �ִϸ��̼� ����
        StartCoroutine(DelaySpwanState(State.Create, monsterSpwanTimer)); //�����ð� �������� �������·� ��ȯ��Ų��
        Destroy(gameObject, 200.0f * Time.deltaTime); //���Ͱ� ����Ѱ�� Ư������������ ��ü ����
    }

    IEnumerator DelaySpwanState(State s, float m_delaytime) //����������ŭ �������� ���¸� �������� ����
    {
        yield return new WaitForSeconds(m_delaytime);
        // gameObject.SetActive(false);
        ChangeState(s);
    }


    // Start is called before the first frame update
    void Start()
    {
        monsterAI.findPlayer.AddListener(StartTracking); //����AI ��ũ��Ʈ�� findPlayer�� �߻��Ұ�� StartTracking �޼��带 ȣ��
        monsterAI.lostPlayer.AddListener(StopTracking);  //�÷��̾ ��������� ���º���
        ChangeState(State.Idle); //���͸� �����·� ����
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
