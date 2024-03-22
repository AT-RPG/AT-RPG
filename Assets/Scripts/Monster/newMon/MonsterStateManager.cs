using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.AI;

public class MonsterStateManager : MonsterBattle
{

    /// <summary>
    /// 컴포넌트 추가
    /// </summary>
    private MonsterMoveManager monsterMoveManager; //이동관리 컴포넌트 추가
    private MonsterBattleManager monsterBattleManager; //전투관리 컴포넌트 추가
    [SerializeField] private MonsterAI monsterAI; //플레이어 추적 판단여부 컴포넌트 추가

    /// <summary>
    /// 스크립트 내에서 사용할 변수
    /// </summary>
    private Vector3 monTargetPos=Vector3.zero; //타겟의 위치를 지정(이동 목적지/플레이어 위치)
    private bool isInBattle=false;

    /// <summary>
    /// 풀 관리 스크립트
    /// </summary>
    private IObjectPool<MonsterStateManager> MonsterPool;
    public void setManagedPool(IObjectPool<MonsterStateManager> pool)
    {
        MonsterPool = pool; //몬스터 풀설정
    }
    public void destroyMosnter()
    {
        ChangeState(MonsterState.Idle);
        MonsterPool.Release(this); //몬스터 풀반환
    }
    private void Awake() //초기화
    {
        monsterAI = GetComponentInChildren<MonsterAI>();
        monsterAI.findPlayer.AddListener(StartTracking);
        monsterAI.lostPlayer.AddListener(StopTracking);

        monsterMoveManager = GetComponent<MonsterMoveManager>();
        monsterBattleManager = GetComponent<MonsterBattleManager>();
        MonsterIdle();
    }
    void OnEnable()
    {
    }

    /// <summary>
    /// 상태 머신
    /// </summary>
    private enum MonsterState
    {
        Idle,
        Moving, 
        Battle, 
        Dead    
    }
    [SerializeField]
    private MonsterState monState=MonsterState.Idle;

    private void ChangeState(MonsterState newState)
    {
        if(monState == newState) return;
        monState= newState;

        switch (monState)
        {
            case MonsterState.Idle:
                MonsterIdle();
                break;
            case MonsterState.Moving:
                monsterAnim.SetBool("Move", true);
                monsterMoveManager.Move(monTargetPos, isInBattle,mStat.monsterRange,mStat.longAttack,mStat.monsterMoveSpeed);
                break;
            case MonsterState.Battle:
                monsterBattleManager.Attack();
                break;
                case MonsterState.Dead:
                break;
        }
    }

    /// <summary>
    /// 추적 관리 스크립트
    /// </summary>
    public void StartTracking(Transform target)
    {
        monTargetPos = target.position;
        isInBattle = true;
        ChangeState(MonsterState.Battle);
    }
    public void StopTracking()
    {
        isInBattle = false;
        ChangeState(MonsterState.Idle);
    }

    /// <summary>
    /// idle상태 스크립트
    /// </summary>
    private void MonsterIdle()
    {
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Move", false);
        mStat.monsterIdleTime = Random.Range(2, 4);
        Invoke("ChangeStateMove", mStat.monsterIdleTime);
    }
    private void ChangeStateMove()
    {
        ChangeState(MonsterState.Moving);
    }

    /// <summary>
    /// 이동관련 보조 스크립트
    /// </summary>
    public void MoveComplete()
    {
        if (isInBattle == true)
        {
            ChangeState(MonsterState.Battle);
        }
        else
            ChangeState(MonsterState.Idle);
    }

    // Start is called before the first frame update
    void Start()
    {
      //  ChangeState(MonsterState.Idle);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
