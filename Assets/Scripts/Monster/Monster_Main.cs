using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class MonsterMain :  MonsterPorperty //인포를 상속,스탯을 가져옴
{
    MonsterInfo mon1 = new MonsterInfo("몬1",10.0f,15.0f,1,30.0f,0.5f, 30.0f);
   
   
    public int monsterNum = 1;
    Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine rotate = null; //몬스터의 회전을 관리

    public MonsterAI monsterAI; //몬스터 ai기능을 참조
    public float trackingDistance = 10f; // 몬스터가 추적하는 최대 거리
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
   
    Vector3 SpawnPos;  //몬스터의 스폰위치를 저장
    float MoveStartTime = 0.0f; //몬스터의 대기시간 
    void ChangeState(State s) //상태가 변할경우 변한상태를 전달받음
    {
        if (monsterState == s) return; //상태가 변하지 않는경우 예외처리
         monsterState = s;  //상태변경
        switch (monsterState)
        {
            case State.Create:  //Start함수와 다른점?
                monsterAI.findPlayer.AddListener(StartTracking); //몬스터AI 스크립트의 findPlayer가 발생할경우 StartTracking 메서드를 호출
                monsterAI.lostPlayer.AddListener(StopTracking);  //플레이어를 놓쳣을경우 상태변경
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

    void monsterIdle() //대기 상태
    {
        MoveStartTime = Random.Range(2, 3); //대기시간을 랜덤으로 설정한후 이동하게 하기
        StartCoroutine(DelayChangeState(State.Move,MoveStartTime));
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
            dir *= Random.Range(10.0f, 50.0f); //랜덤한 거리
            return SpawnPos + dir;   //스폰한위치로부터 거리를 더한다
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

        
        bool isRunning = dist >= 40.0f; // 거리에 따라 달리기 여부 판단
        monsterAnim.SetBool("Move", !isRunning);
        monsterAnim.SetBool("Run", isRunning);

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




    void monsterBattle( )  //전투상태
    {

    }
    void monsterDead() //사망상태
    {
        //플레이어에게 골드를 주거나 아이템을 드랍후 몬스터 삭제
        //gold += Random.Range(1, 10);
        //StopAllCoroutines();//모든 코룬틴 정지;
        Destroy(gameObject,10.0f*Time.deltaTime); //몬스터가 사망한경우 특정프레임이후 객체 삭제
    }


    // Start is called before the first frame update
    void Start()
    {
        SpawnPos= transform.position; //시작과 동시에 생성위치를 기억
        ChangeState(State.Idle); //몬스터를 대기상태로 변경
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
