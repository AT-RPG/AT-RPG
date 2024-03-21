using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMoveManager : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private MonsterStateManager monsterStateManager;

    float range=0.0f;
    bool longAttack = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterStateManager = GetComponent<MonsterStateManager>();
    }

    public void Move(Vector3 targetPos, bool isInBattle,float mRange,bool rangeType)
    {
        range= mRange;
        longAttack = rangeType;

        ///이동 부분
        if (isInBattle == true)
        {
            Vector3 targetPositionWithRange = TargetPositionWithRange(targetPos); //사거리에 도달할때까지 이동
            navMeshAgent.SetDestination(targetPositionWithRange);

            if (Vector3.Distance(transform.position, targetPositionWithRange) <= range)//사거리에 도달한다면
            {
                if (longAttack)       // 원거리에 경우 사거리만큼 뒤로 물러나기
                {
                    Vector3 retreatPosition = TargetPositionWithRange(targetPos);
                    navMeshAgent.SetDestination(retreatPosition);
                }
                else
                {
                    navMeshAgent.isStopped = true; //아니면 멈춘다
                }
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
        else
        {
            SetRandomDestination(); //비전투시 랜덤위치로 이동
        }
    }

    /// <summary>
    /// 랜덤으로 위치 지정후 이동
    /// </summary>
    private void SetRandomDestination()
    {
        // 네비게이션 영역 내에서 랜덤한 위치 생성
        Vector3 randomPosition = GetRandomNavMeshPosition();

        // 생성된 랜덤 위치를 목적지로 설정
        navMeshAgent.SetDestination(randomPosition);
    }
    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // 최대 이동 반경 지정
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas); // 랜덤 위치 

        return hit.position;
    }

    /// <summary>
    /// 원거리 몬스터의 경우 사거리보다 가까울 경우 뒤로 간다
    /// </summary>
    private Vector3 TargetPositionWithRange(Vector3 targetPos)
    {
        // 목표 지점과 몬스터의 현재 위치 사이의 방향 벡터 계산
        Vector3 directionToTarget = targetPos - transform.position;
        // 방향 벡터를 반대로 설정하고 사거리만큼의 거리를 곱하여 뒤로 물러날 위치 계산
        Vector3 retreatPosition = transform.position - directionToTarget.normalized * range;
        return retreatPosition;
    }


// Update is called once per frame
void Update()
    {
        if (!navMeshAgent.pathPending && !navMeshAgent.hasPath && !navMeshAgent.isPathStale)
        {
            monsterStateManager.MoveComplete();
        }
    }
}
