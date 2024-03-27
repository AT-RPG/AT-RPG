using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using AT_RPG;
using UnityEngine.AI;
public class MonsterMain : CommonBattle
{
  
    private IObjectPool<MonsterMain> MonsterPool;

    public void setManagedPool(IObjectPool<MonsterMain> pool)
    {
        MonsterPool = pool; //몬스터 풀설정
    }
    public void destroyMosnter()
    {
        //초기화 함수 추가
    //    InitializeMonster();
        MonsterPool.Release(this); //몬스터 풀반환
    }
    public void InitializeMonster()
    {
        transform.position = StartspawnPos.transform.position; //위치 초기화
    }
    private void Awake() //초기화
    {
        ChangeState(State.Create);
    }
    void OnEnable()
    {
        ChangeState(State.Create);
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        monAgent = GetComponent<NavMeshAgent>();
        if (myTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, myTarget.position);
            if (distanceToPlayer <= mStat.monsterRange)
            {
                StartTracking(myTarget);
            }
        }
    }


    [SerializeField] Transform monResPos;
    [SerializeField] GameObject StartspawnPos;

    public Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine deleyMove = null; //몬스터의 움직임을 관리
    public Coroutine battleState = null;


    public NavMeshAgent monAgent;

    public MonsterAI monsterAI;
    private bool isTracking = false;
    
    public MonsterStat mStat;

    [System.Serializable]
    public struct MonsterStat
    {
        public float monsterRange;
        public float monsterIdleTime;
        public float monsterRunSpeed;
    }



    //몬스터 상태
    public enum State
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    //몬스터의 기본상태 설정
    public State monsterState = State.Create;

    //몬스터 상태변경
    void ChangeState(State newState)
    {
        if (monsterState == newState) return;
        
        monsterState = newState;
        switch (monsterState)
        {
            case State.Create:
                createMonster();
                break;
            case State.Idle:
                idleState();
                break;
            case State.Move:
                moveState();
                break;
            case State.Battle:
                battleState=StartCoroutine(BattleState());
                break;
            case State.Dead:
                break;
        }
    }

    //몬스터 생성
    void createMonster()
    {
        monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
        monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
        transform.position = transform.parent.position; //스폰위치 설정
        base.Initialize();
        ChangeState(State.Idle);
    }

    //몬스터 대기 상태
    void idleState()
    {
        monAgent.ResetPath();
        myAnim.SetBool("Run", false);
        myAnim.SetBool("Move", false);
        mStat.monsterIdleTime = Random.Range(2, 4);
        deleyMove = StartCoroutine(DelayChangeState(State.Move, mStat.monsterIdleTime));
    }
    //일정시간후 상태변경
    IEnumerator DelayChangeState(State newState, float m_delaytime)
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(newState);
    }


    //몬스터 이동상태
    void moveState()
    {
        Vector3 dir = Vector3.forward;
        SetRndDir(dir);
    }

    //랜덤한 방향설정
    void SetRndDir(Vector3 dir)
    {
        Vector3 GetRndPos()
        {
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
            dir *= Random.Range(10.0f, 50.0f);
            return monResPos.position + dir;
        }
        MoveToPos(GetRndPos());
    }

    //목표위치로 이동
    void MoveToPos(Vector3 target)
    {
        myAnim.SetBool("BackWalk", false);
        if (move != null)
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(MovingCoroutine(target));
    }

    void IsRunning(float dist)//달리기
    {
        float runOk = 30.0f;
        if (dist >= runOk)
        {
            monAgent.speed = mStat.monsterRunSpeed;
            myAnim.SetBool("Move", false);
            myAnim.SetBool("Run", true);
        }
        else
        {
            monAgent.speed = baseBattleStat.moveSpeed;
            myAnim.SetBool("Run", false);
            myAnim.SetBool("Move", true);
        }

    }
    public IEnumerator MovingCoroutine(Vector3 target)
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target);
            IsRunning(dist);
            monAgent.SetDestination(target);
            yield return null;
            if (dist <= 0.1f)
            {
                break;
            }
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

    
    public void StartTracking(Transform target)
    {
        if (monsterState != State.Dead)
        {
            StopCoroutine(deleyMove);
            myTarget = target;
            ChangeState(State.Battle);
        }
    }
    //몬스터 플레이어 놓침
    public void StopTracking()
    {
        if (monsterState != State.Dead)
        {
            if (move != null) StopCoroutine(move);
            if (battleState != null) StopCoroutine(battleState);
            myTarget = null;
            ChangeState(State.Idle);
        }
    }

    public IEnumerator BattleState()
    {
        while (myTarget != null)
        {
            Vector3 battletarget = myTarget.transform.position;
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
                break;
            }
        }
        
    }

    public virtual void AttackPlayer()
    {
    }
    public virtual void AttackDeleay()
    {
    }



    //몬스터 사망상태
    public void deadState()
    {
        myTarget = null;
        StopAllCoroutines();
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        ChangeState(State.Dead);
        Invoke("destroyMosnter", 3f); //풀 릴리스 호출
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Initialize();
        ChangeState(State.Idle);
    }

    private void Update()
    {

    }

}