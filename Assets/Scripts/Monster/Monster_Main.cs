using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

//일단 구현하고 구현되면 차후 스크립트 분리하기-----------
public class MonsterMain :  MonsterPorperty //애니메이터를 쓰기위해 애니메이터 제어 스크립트상속
{
    [SerializeField] Transform monResPos;
    MonsterInfo mon1 = new MonsterInfo("몬1",10.0f,3.0f,1,30.0f,0.5f, 30.0f); //몬스터 인포 참조 --방식 수정해야함
                                                                             //인포방식 가독성 매우안좋으므로 몬스터마다 개별 인포를 만들후 스탯을 사용하게끔 변경

    public GameObject Imp; //몬스터 리스폰을 위한 게임오브젝트 지정
    
    Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine rotate = null; //몬스터의 회전을 관리

    public MonsterAI monsterAI; //몬스터 ai기능을 참조
    private bool isTracking = false;     // 몬스터가 추적 중인지 판단

    bool runCheck = false;  //몬스터가 달리기 애니메이션을 출력하는 거리판단
    

    public enum State //유한상태머신 :: 스폰 -대기 -이동 -전투 -사망
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState=State.Create;//몬스터 초기 상태를 스폰으로 설정

    //Vector3 SpawnPos = new Vector3(30.0f, 1.0f, 30.0f);  //몬스터의 스폰위치설정
    float moveStartTime = 0.0f; //몬스터의 대기시간 
    float monsterSpwanTimer = 2.0f;//몬스터 리스폰대기시간
    Transform monsterTarget; //전투상태일때 플레이어를 추적
    void ChangeState(State s) //상태가 변할경우 변한상태를 전달받음
    {
        if (monsterState == s) return; //상태가 변하지 않는경우 예외처리
         monsterState = s;  //상태변경
        switch (monsterState)
        {
            case State.Create:  
                monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
                monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
                monsterCreate();
                break;

            case State.Idle: //몬스터가 대기상태
                monsterAnim.SetBool("Run", false);
                monsterAnim.SetBool("Move", false); //몬스터의 이동동작 애니메이션을 취소
                monsterIdle();
                break;
            case State.Move: //몬스터가 움직이는 상태
                monsterMove();
                break;
            case State.Battle: //몬스터가 전투상태
                monsterBattle();
                break;
            case State.Dead: //몬스터가 사망
                monsterDead();
                break;
        }
    }
    void monsterCreate()//몬스터 생성-몬스터가 사망한후 일정시간이후 다시 재스폰시킴
    {
        Debug.Log("몬스터 리스폰 성공");
        Instantiate(Imp); //임프의 사본을 생성한다
        transform.position = monResPos.position; //몬스터를 스폰위치로 이동시킨다
        ChangeState(State.Idle);//몬스터의 상태를 대기로 변경
    }
    void monsterIdle() //대기 상태
    {
        if (isTracking == false) //추적상태가 아닐경우
        {
            moveStartTime = Random.Range(2, 3); //대기시간을 랜덤으로 설정한후 이동하게 하기
            StartCoroutine(DelayChangeState(State.Move, moveStartTime));
        }
        else ChangeState(State.Move);//추적상태일 경우 대기없이 즉시 이동상태로 전환
    }
    IEnumerator DelayChangeState(State s, float m_delaytime) //정해진값만큼 딜레이후 상태를 움직임으로 변경
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

    //----------추적관련-------------------------------
    public void StartTracking(Transform target) //플레이어 발견
    {
        isTracking = true;
       
        StopCoroutine(move); //추적에 성공하면 이전에 실행중이던 이동코룬틴 중지
        monsterMove();//이동함수 즉시호출-플레이어를 계속 추적함
        monsterTarget = target; //전투상황시 플레이어추적위함 
    }

    public void StopTracking() //플레이어 놓침
    {
        isTracking = false;
        
    }

    //---------------이동관련---------------------------------------------------
    void monsterMove() //이동상태
    {

        Vector3 dir = Vector3.forward;

        Vector3 GetRndPos() //몬스터가 움직일 방향 랜덤지정
        {
            dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir; //360
            dir *= Random.Range(10.0f, 30.0f); //랜덤한 거리
            return monResPos.position + dir;   //스폰한위치로부터 거리를 더한다
        }

        Vector3 GetTrackingPos() // 플레이어의 위치로 방향지정
        {
            dir = monsterAI.myTarget.position - transform.position;
            return transform.position + dir; // 현재 위치에서 플레이어 방향으로 이동
        }


        if (isTracking)  //플레이어의 추적여부에따라 몬스터가 움직일방향을 정해준다
        {
            MoveToPos(GetTrackingPos());
            
        }
        else 
        {
            MoveToPos(GetRndPos());
         
        }

    }

    void MoveToPos(Vector3 target) //특정위치로 이동
    {
        if(move != null) //이미 이동중인경우 이전의 코룬틴을 정지시킴
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(monsterMoving(target)); //새로운 이동코룬틴
    }

   
    IEnumerator monsterMoving(Vector3 target)
    {
        Vector3 dir = target-transform.position; //목표위치까지 방향계산
        float dist = dir.magnitude; //목표위치까지 거리계산
        dir.Normalize(); //정규화

        //-----달리기 제어 스크립트------
        bool isRunning = dist >= 40.0f; // 거리에 따라 달리기 여부 판단
        if (isRunning) mon1.monsterMoveSpeed += 3.0f;//달리기 발생시 이동속도 증가
        monsterAnim.SetBool("Move", !isRunning);
        monsterAnim.SetBool("Run", isRunning);

        //--배틀 시작 제어 스크립트--
        if (isTracking)//플레이어를 추적중이고
        {
            if (dist < mon1.mRange)//플레이어와의 거리가 1 미만이면
            {
                //   ChangeState(State.Battle); //상태를 전투로 즉시 전환한다
                ChangeState(State.Dead); //몬스터 리스폰 실험//////////
            }
        }

        if (rotate != null) StopCoroutine(rotate); //이전회전 코룬틴 정지
        rotate = StartCoroutine(Rotating(dir)); //새로운 회전코룬틴 시작

        while (!Mathf.Approximately(dist, 0.0f))
        {
            
            float delta = mon1.monsterMoveSpeed * Time.deltaTime; 
             
            if (delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World); //이동
            
            yield return null;
        }
        ChangeState(State.Idle); //몬스터를 대기상태로 변경
        
    }
    IEnumerator Rotating(Vector3 dir) //몬스터를 주어진 방향으로 회전
    {
        float angle = Vector3.Angle(transform.forward, dir); //현재방향과 목표방향비교
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)//회전 방향 결정
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
            transform.Rotate(Vector3.up * rotDir * delta); //회전
            yield return null;
        }
    }
    //----------------------------------------------------------------------------------------------



    //-------전투 관련 스크립트 ----------------------------------------------
    void monsterBattle()  //전투상태
    {
        //---애니메이션을 전투로 변경
        monsterAnim.SetBool("Move", false); 
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", true);







    }

  
    /// ------사망 관련 스크립트---------------------------------------------------
    ///아이템 드랍+골드드랍+플레이어에게 주는 경험치 요소가 추가되어야함 <summary>
    /// 리스폰보다 디스트로이가 빠른경우 리스폰이 되지않는 현상생김
    /// </summary>
    void monsterDead() //사망상태
    {
        StopAllCoroutines();//모든 코룬틴 정지;
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", false);
        monsterAnim.SetTrigger("Dead"); //사망 애니메이션 실행
        StartCoroutine(DelaySpwanState(State.Create, monsterSpwanTimer)); //일정시간 딜레이후 생성상태로 전환시킨다
        Destroy(gameObject,200.0f*Time.deltaTime); //몬스터가 사망한경우 특정프레임이후 객체 삭제
    }
    IEnumerator DelaySpwanState(State s, float m_delaytime) //정해진값만큼 딜레이후 상태를 생성으로 변경
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

  
    // Start is called before the first frame update
    void Start()
    {
        //SpawnPos= transform.position; //시작과 동시에 생성위치를 기억
        ChangeState(State.Idle); //몬스터를 대기상태로 변경
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
