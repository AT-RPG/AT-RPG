using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MonsterMain :MonsterBattle, MDamage
{
    [SerializeField] Transform monResPos;
   
    Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine deleyMove = null; //몬스터의 움직임을 관리
    Coroutine rotate = null; //몬스터의 회전을 관리

    public MonsterAI monsterAI; //몬스터 ai기능을 참조
    private bool isTracking = false;
    float moveStartTime = 0.0f; //몬스터의 대기시간 
    Transform monsterTarget; //전투상태일때 플레이어를 추적

    public enum State //유한상태머신 :: 스폰 -대기 -이동 -전투 -사망
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState = State.Create;//몬스터 초기 상태를 스폰으로 설정

    void ChangeState(State s) //상태가 변할경우 변한상태를 전달받음
    {
        if (monsterState == s) return; //상태가 변하지 않는경우 예외처리
        monsterState = s;  //상태변경
        switch (monsterState)
        {
            case State.Create:
                monsterCreate();
                break;
            case State.Idle: //몬스터가 대기상태
                monsterIdle();
                break;
            case State.Move: //몬스터가 움직이는 상태
                monsterMove();
                break;
            case State.Battle: //몬스터가 전투상태
                StartCoroutine(monsterBattle());
                break;
            case State.Dead: //몬스터가 사망
                monsterDead();
                break;
        }
    }
    void monsterCreate()//몬스터 생성-몬스터가 사망한후 일정시간이후 다시 재스폰시킴  
    {
        //Instantiate(Imp); //임프의 사본을 생성한다
        ChangeState(State.Idle);//몬스터의 상태를 대기로 변경
    }

    /// <summary>
    /// 몬스터의 대기상태
    /// 플레이어를 추적중일경우 대기없이 즉시 이동상태로전환/아닐경우 일정시간 딜레이후에 이동상태로 전환
    /// </summary>
    void monsterIdle()
    {
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Move", false);

        moveStartTime = Random.Range(2, 4);
        deleyMove = StartCoroutine(DelayChangeState(State.Move, moveStartTime));
    }
    IEnumerator DelayChangeState(State s, float m_delaytime)
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }


    /// <summary>
    /// 이동관련 제어 함수
    /// </summary>
    void monsterMove()
    {
        Vector3 dir = Vector3.forward;
        monsterDirection(dir);
    }

    void monsterDirection(Vector3 dir)
    {
        Vector3 GetRndPos()
        {
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
            dir *= Random.Range(10.0f, 30.0f);
            return monResPos.position + dir;
        }
        MoveToPos(GetRndPos());
    }
    void MoveToPos(Vector3 target) //목표로 이동시작
    {
        if (move != null) //이미 이동중인경우 이전의 코룬틴을 정지시킴
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(monsterMoving(target)); //새로운 이동코룬틴
    }

    void IsRunning(float dist)//달리기
    {
        float runOk = 40.0f;
        if (dist >= runOk)
        {
            mStat.monsterMoveSpeed = 6.0f;
            monsterAnim.SetBool("Move", false);
            monsterAnim.SetBool("Run", true);
        }
        else
        {
            mStat.monsterMoveSpeed = 3.0f;
            monsterAnim.SetBool("Run", false);
            monsterAnim.SetBool("Move", true);
        }

    }
    IEnumerator monsterMoving(Vector3 target)//몬스터의 이동
    {
        //목표 위치까지의 방향과 거리계산후 정규화
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        //회전코루틴의 정지/재시작
        if (rotate != null) StopCoroutine(rotate);
        rotate = StartCoroutine(Rotating(dir));


        while (dist > 0.1f)
        {
            float delta = mStat.monsterMoveSpeed * Time.deltaTime;
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
        else
        {
            ChangeState(State.Battle);
        }
    }
    IEnumerator Rotating(Vector3 dir) //몬스터를 회전
    {
        float angle = Vector3.Angle(transform.forward, dir); //현재방향과 목표방향비교
        float rotDir = 1.0f;
        //회전 방향 결정
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -1.0f;
        }
        //회전 시작
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
    /// 몬스터의 추적제어 함수
    /// </summary>
    public void StartTracking(Transform target)
    {
        StopCoroutine(deleyMove);
        monsterTarget = target;
        ChangeState(State.Battle);
    }
    public void StopTracking()
    {
        monsterAnim.SetBool("IsAttack", false);
        monsterTarget = null;
        ChangeState(State.Idle);
    }


    /// <summary>
    /// -------전투 관련 스크립트 ----------------------------------------------
    /// </summary>
    
    

    IEnumerator monsterBattle()  //전투상태
    {
        while (monsterTarget != null)
        {
            Vector3 battletarget = monsterTarget.transform.position;
            Vector3 dir = battletarget - transform.position;
            float dist = dir.magnitude;

            if (mStat.monsterRange < dist) //사거리보다 길면 플레이어에게 이동
            {
                MoveToPos(battletarget);
                yield return null;
            }
            else
            {
                attackPlayer();
                yield return new WaitForSeconds(3.0f);
            }
        }
    }

    void attackPlayer()
    {
        
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetTrigger("NormalAttack");

        if (move != null)
        {
            StopCoroutine(move);
        }
    }

    public void TakeDamage(float dmg)
    {
        // MDamage 인터페이스의 TakeDamage 메서드를 호출하여 데미지를 전달합니다.
        if (monsterTarget != null && monsterTarget.GetComponent<MDamage>() != null)
        {
            monsterTarget.GetComponent<MDamage>().TakeDamage(dmg);
        }
    }



    /// ------사망 관련 스크립트---------------------------------------------------
    ///리
    /// </summary>
    void monsterDead() //사망상태
    {
        StopAllCoroutines();//모든 코룬틴 정지;
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetTrigger("Dead"); //사망 애니메이션 실행
        Destroy(gameObject, 200.0f * Time.deltaTime); //몬스터가 사망한경우 특정프레임이후 객체 삭제
    }

    // Start is called before the first frame update
    void Start()
    {
        monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
        monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
        ChangeState(State.Idle); //몬스터를 대기상태로 변경
    }

    // Update is called once per frame
    void Update()
    {

    }
}
