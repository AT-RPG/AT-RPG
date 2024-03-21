using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.AI;

public class MonsterMain : MonsterBattle, MDamage
{
    private IObjectPool<MonsterMain> MonsterPool;

    public void setManagedPool(IObjectPool<MonsterMain> pool)
    {
        MonsterPool = pool; //몬스터 풀설정
    }
    public void destroyMosnter()
    {
        //초기화 함수 추가
        InitializeMonster();
        MonsterPool.Release(this); //몬스터 풀반환
    }
    public void InitializeMonster()
    {
        transform.position = StartspawnPos.transform.position; //위치 초기화
    }
    private void Awake() //초기화
    {
        transform.position = transform.parent.position; //스폰위치 설정
        monNav = GetComponent<NavMeshAgent>(); //네브메쉬 할당
        monNav.speed = mStat.monsterMoveSpeed;
        monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
        monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
        idleState();
    }
    void OnEnable()
    {
        monNav.speed = mStat.monsterMoveSpeed;
        transform.position = transform.parent.position; //스폰위치 설정
        idleState();
    }


    [SerializeField] Transform monResPos;
    [SerializeField] GameObject StartspawnPos;

    Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine deleyMove = null; //몬스터의 움직임을 관리

    public MonsterAI monsterAI; 
    private bool isTracking = false;
    Transform monsterTarget;

    NavMeshAgent monNav;

    //몬스터 상태
    public enum State 
    {
        Idle,
        Move,
        Battle,
        Dead
    }
    //몬스터의 기본상태 설정
    public State monsterState;

    //몬스터 상태변경
    void ChangeState(State newState) 
    {
        if (monsterState == newState) return; 
        monsterState = newState; 
        switch (monsterState)
        {
            case State.Idle: 
                idleState();
                break;
            case State.Move:
                SetRndDir();
                break;
            case State.Battle:
                StartCoroutine(battleState());
                break;
            case State.Dead:
                deadState();
                break;
        }
    }

    //몬스터 생성
   

    //몬스터 대기 상태
    void idleState()
    {
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Move", false);
        mStat.monsterIdleTime = Random.Range(2, 4);
        deleyMove = StartCoroutine(DelayChangeState(State.Move, mStat.monsterIdleTime));
    }
    //일정시간후 상태변경
    IEnumerator DelayChangeState(State newState, float m_delaytime)
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(newState);
    }


    //랜덤한 방향설정
    void SetRndDir()
    {
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(10.0f, 30.0f);
        randomDirection += monResPos.position; 
        NavMeshHit hit = new NavMeshHit();   // NavMesh 상에서 가장 가까운 유효한 위치로 설정

        if (NavMesh.SamplePosition(randomDirection, out hit, 30.0f, NavMesh.AllAreas))
        {
            MoveToPos(hit.position);  // 유효한 위치가 발견되면 해당 위치를 목표 위치로 설정
        }
        else
        {
            SetRndDir();
        }
    }
    void MoveToPos(Vector3 target)
    {
       
        if (move != null)
        {
            StopCoroutine(move);
        }
       
        move = StartCoroutine(MovingCoroutine(target));
    }
    IEnumerator MovingCoroutine(Vector3 target)
    {
        monNav.SetDestination(target);
        while (monNav.remainingDistance > monNav.stoppingDistance)
        {
            yield return null;
            IsRunning(monNav.remainingDistance);
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

    void IsRunning(float remainingDistance)//달리기
    {
        float runOk = 40.0f;
        if (remainingDistance >= runOk)
        {

            monNav.speed = mStat.monsterRunSpeed;
            monsterAnim.SetBool("Move", false);
            monsterAnim.SetBool("Run", true);
        }
        else
        {
            monNav.speed=mStat.monsterMoveSpeed;
            monsterAnim.SetBool("Run", false);
            monsterAnim.SetBool("Move", true);
        }

    }


    //몬스터 플레이어 발견
    public void StartTracking(Transform target)
    {
        StopCoroutine(deleyMove);
        monsterTarget = target;
        ChangeState(State.Battle);
    }
    //몬스터 플레이어 놓침
    public void StopTracking()
    {
        monsterAnim.SetBool("IsAttack", false);
        monsterTarget = null;
        ChangeState(State.Idle);
    }

    //몬스터 전투상태
    IEnumerator battleState()  
    {
        while (monsterTarget != null)
        {
            Vector3 battletarget = monsterTarget.transform.position;
            Vector3 dir = battletarget - transform.position;
            float dist = dir.magnitude;

            if (mStat.monsterRange < dist) 
            {
                MoveToPos(battletarget);
                yield return null;
            }
            else
            {
                AttackPlayer();
                yield return new WaitForSeconds(mStat.monsterRpm);
            }
        }
    }

    void AttackPlayer()
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetTrigger("NormalAttack");
        if (mStat.longAttack == true)
        {
          
            //원거리 공격실행
        }
    }

    //데미지 받기
    public void TakeDamage(float dmg)
    {
        // MDamage 인터페이스의 TakeDamage 메서드를 호출하여 데미지를 전달합니다.
        if (monsterTarget != null && monsterTarget.GetComponent<MDamage>() != null)
        {
            monsterTarget.GetComponent<MDamage>().TakeDamage(dmg);
        }
    }



    //몬스터 사망상태
    void deadState()
    {
        StopAllCoroutines();
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetTrigger("Dead");
        Invoke("destroyMosnter", 5f); //풀 릴리스 호출
    }




    // Start is called before the first frame update
    void Start()
    {
        ChangeState(State.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) //생성
        {
            ChangeState(State.Dead);
        }
       
    }
}
