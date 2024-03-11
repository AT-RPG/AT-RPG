using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 클래스단위로 리펙토링->배틀구현->스폰
/// </summary>
public class MonsterMain : MonsterPorperty //애니메이터를 쓰기위해 애니메이터 제어 스크립트상속
{
    [SerializeField] Transform monResPos;
    MonsterInfo mon1 = new MonsterInfo("몬1", 10.0f, 3.0f, 1, 30.0f, 0.5f, 10.0f); //몬스터 인포 참조 --방식 수정해야함
                                                                                  //인포방식 가독성 매우안좋으므로 몬스터마다 개별 인포를 만들후 스탯을 사용하게끔 변경

    public GameObject Imp; //몬스터 리스폰을 위한 게임오브젝트 지정

    Coroutine move = null; //몬스터의 움직임을 관리
    Coroutine rotate = null; //몬스터의 회전을 관리
    Coroutine chasing = null; //몬스터의 추적을 관리


    public MonsterAI monsterAI; //몬스터 ai기능을 참조
    private bool isTracking = false;

    public enum State //유한상태머신 :: 스폰 -대기 -이동 -전투 -사망
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State monsterState = State.Create;//몬스터 초기 상태를 스폰으로 설정

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
                monsterCreate();
                break;
            case State.Idle: //몬스터가 대기상태
                monsterIdle();
                break;
            case State.Move: //몬스터가 움직이는 상태
                monsterMove();
                break;
            case State.Battle: //몬스터가 전투상태
                StartCoroutine(monsterBattle(monsterTarget.position));
                break;
            case State.Dead: //몬스터가 사망
                monsterDead();
                break;
        }
    }
    void monsterCreate()//몬스터 생성-몬스터가 사망한후 일정시간이후 다시 재스폰시킴
    {
        Instantiate(Imp); //임프의 사본을 생성한다
        transform.position = monResPos.position; //몬스터를 스폰위치로 이동시킨다
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
        Debug.Log($"{moveStartTime}이동목표 설정");
        StartCoroutine(DelayChangeState(State.Move, moveStartTime));
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

    void monsterDirection(Vector3 dir)//몬스터의 로밍 목표 설정
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
    IEnumerator monsterMoving(Vector3 target)//몬스터의 이동
    {
        //목표 위치까지의 방향과 거리계산후 정규화
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        //거리에 따른 달리기/전투 판단
        //if (dist <= mon1.mRange)

        //회전코루틴의 정지/재시작
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
    /// -------전투 관련 스크립트 ----------------------------------------------
    /// 타겟이 감지가되면 타겟의 좌표를 얻은후에 배틀함수 호출
    /// 이후 타겟과 거리를 재면서 사거리보다 멀리떨어져있으면 이동함수 호출
    /// 이후 타겟이 사거리안에 들어오면 공격
    /// </summary>


    IEnumerator monsterBattle(Vector3 battletarget)  //전투상태
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
    

    /// ------사망 관련 스크립트---------------------------------------------------
    ///리스폰 관리자로 별도의 스크립트로 분리할것
    /// </summary>
    void monsterDead() //사망상태
    {
        StopAllCoroutines();//모든 코룬틴 정지;
        monsterAnim.SetBool("Move", false);
        monsterAnim.SetBool("Run", false);
        monsterAnim.SetBool("Battle", false);
        monsterAnim.SetTrigger("Dead"); //사망 애니메이션 실행
        StartCoroutine(DelaySpwanState(State.Create, monsterSpwanTimer)); //일정시간 딜레이후 생성상태로 전환시킨다
        Destroy(gameObject, 200.0f * Time.deltaTime); //몬스터가 사망한경우 특정프레임이후 객체 삭제
    }

    IEnumerator DelaySpwanState(State s, float m_delaytime) //정해진값만큼 딜레이후 상태를 생성으로 변경
    {
        yield return new WaitForSeconds(m_delaytime);
        // gameObject.SetActive(false);
        ChangeState(s);
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
