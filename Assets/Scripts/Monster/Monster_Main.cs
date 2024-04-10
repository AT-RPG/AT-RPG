using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using AT_RPG;
using UnityEngine.AI;
using System.IO;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

/// <summary>
/// 몬스터 공통부분 관리스크립트
/// </summary>
public class MonsterMain : CommonBattle
{

    private IObjectPool<MonsterMain> MonsterPool;

    public void setManagedPool(IObjectPool<MonsterMain> pool)
    {
        MonsterPool = pool; //몬스터 풀설정
    }
    public void destroyMosnter()
    {
        if (myHpBar != null)
        {
            Destroy(myHpBar.gameObject);
            myHpBar = null; // 체력바 참조 해제
        }

        MonsterPool.Release(this); //몬스터 풀반환
    }
    private void Awake() //초기화
    {
        ChangeState(State.Create);
    }
    void OnEnable()
    {
        

        string path = Application.dataPath + "/Resources/Monster/JJappalWorld_MonsterInfoData.csv";
            LoadMonsterStatsFromCSV(path);


        ChangeState(State.Create);
        GetComponent<Collider>().enabled = true;
    
        monAgent = GetComponent<NavMeshAgent>();
        if (myTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, myTarget.position);
            if (distanceToPlayer <= mStat.monsterRange)
            {
                StartTracking(myTarget);
            }
        }
        base.Initialize();
        GameObject hpbar = Resources.Load<GameObject>("Monster/HpBar");
        GameObject obj = Instantiate(hpbar, SceneData.Instance.hpBarsTransform);
        myHpBar = obj.GetComponent<MonsterHpBar>();
        myHpBar.myTarget = hpViewPos;
        base.changeHpAct.AddListener(myHpBar.ChangeHpSlider);
    }


    [SerializeField] Transform monResPos;
    [SerializeField] GameObject StartspawnPos;

    public Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine deleyMove = null; //몬스터의 움직임을 관리
    public Coroutine battleState = null;
    public Coroutine trackPlayerOnDamage=null;

    public Transform hpViewPos; //hp바의 위치 지정
    MonsterHpBar myHpBar;

    public NavMeshAgent monAgent;

    public MonsterAI monsterAI;
    private bool isTracking = false;

    //스탯처리
    public int MonsterIndex;
    private float monsterIdleTime;

    [SerializeField]
    protected MonsterStat mStat;

    public GameObject deathVFX; //생성할 이펙트를 등록 합니다. 

    public struct MonsterStat
    {
        public string monsterName;
        public float monsterRange;
        public float monsterRunSpeed;
        public string monsterType;
    }

    void LoadMonsterStatsFromCSV(string fileContent) //csv 파일에서 스탯가져오기
    {
        StreamReader reader = new StreamReader(fileContent); //파일 읽기
        string line;

        // 첫 번째 줄은 스탯의 이름을 작성했으므로 넘긴다
        reader.ReadLine();

        while ((line = reader.ReadLine()) != null)
        {
            // 쉼표로 구분된 데이터를 파싱하여 배열로 저장
            string[] data = line.Split(',');
            float monsterIndex = float.Parse(data[0]);
            string monsterName = data[1];
            string monsterType = data[2];
            int maxHP = int.Parse(data[3]);
            int attackPoint = int.Parse(data[4]);
            float attackDeley = float.Parse(data[5]);
            float skillCooltime = float.Parse(data[6]);
            float monsterRange = float.Parse(data[7]);
            float moveSpeed = float.Parse(data[8]);
            float monsterRunSpeed = float.Parse(data[9]);

            if (monsterIndex == MonsterIndex) //해당줄의 인덱스랑 현재몬스터의 인덱스가 일치하면 스탯부여
            {
                // 추출한 스탯을 mStat만 현재 값이 안들어가는 상황
                mStat = new MonsterStat();
                baseBattleStat = new BaseBattleStat();
                mStat.monsterName = monsterName;
                mStat.monsterType = monsterType;
                mStat.monsterRange = monsterRange;
                mStat.monsterRunSpeed = monsterRunSpeed;

                baseBattleStat.moveSpeed = moveSpeed;
                baseBattleStat.maxHP = maxHP;
                baseBattleStat.attackPoint = attackPoint;
                baseBattleStat.skillCooltime = skillCooltime;
                baseBattleStat.attackDeley = attackDeley;
                break;
            }
        }
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
                battleState = StartCoroutine(BattleState());
                break;
            case State.Dead:
                break;
        }
    }


    public IEnumerator TrackPlayerOnDamage() //피해를 입으면 즉시 플레이어를 추적
    {
        float MaxHp = baseBattleStat.maxHP;
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (MaxHp > baseBattleStat.maxHP)
            {
                string playerTag = "Player";
                GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
                if (playerObject != null)
                {
                    Transform playerTransform = playerObject.transform;
                    StartTracking(playerTransform);
                }
                break;
            }
        }
    }

    //몬스터 생성
    void createMonster()
    {
        monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
        monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
        transform.position = transform.parent.position; //스폰위치 설정
      //  trackPlayerOnDamage = StartCoroutine(TrackPlayerOnDamage()); //피해감지 코룬틴 시작
        ChangeState(State.Idle);
    }

    //몬스터 대기 상태
    void idleState()
    {
        if(trackPlayerOnDamage == null) //전투상태로 들어가서 꺼진상태라면
        {
            trackPlayerOnDamage = StartCoroutine(TrackPlayerOnDamage()); //피해감지 코룬틴 시작
        }
        monAgent.ResetPath();
        myAnim.SetBool("Run", false);
        myAnim.SetBool("Move", false);
        monsterIdleTime = Random.Range(2, 4);
        deleyMove = StartCoroutine(DelayChangeState(State.Move, monsterIdleTime));
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
       // StopCoroutine(trackPlayerOnDamage);//피해감지 코룬틴 정지
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
                if (move != null) StopCoroutine(move);
                monAgent.ResetPath();
                AttackPlayer();
                break;
            }
        }

    }

    public virtual void AttackPlayer()
    {
       
    }
    public virtual void AttackDelay()
    {
        
    }



    //몬스터 사망상태
    public void deadState()
    {
        myTarget = null;
        StopAllCoroutines();
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        monAgent.ResetPath();
        ChangeState(State.Dead);
        StartCoroutine(deadAnimation());
       // Invoke("destroyMosnter", 3f); //풀 릴리스 호출
    }


    IEnumerator deadAnimation()
    {
        GameObject myVfx = Instantiate(deathVFX);  // 이펙트 게임오브젝트를 생성 합니다. 
        myVfx.transform.position = this.gameObject.transform.position;  // 이펙트 포지션
        myVfx.transform.rotation = Quaternion.identity;  // 이펙트 로테이션
        yield return new WaitForSeconds(2.0f);  // 2초 기다립니다.
        Destroy(myVfx);
        destroyMosnter(); 
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