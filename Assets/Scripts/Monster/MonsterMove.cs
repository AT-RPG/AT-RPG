using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private Coroutine moveCoroutine;
    private Coroutine rotateCoroutine;

    public void MoveToPosition(Transform transform, Vector3 targetPosition, float moveSpeed)
    {
        if (moveCoroutine != null)
        {
            // 이동 코루틴 중지
            StopCoroutine(moveCoroutine);
        }
        if (rotateCoroutine != null)
        {
            // 회전 코루틴 중지
            StopCoroutine(rotateCoroutine);
        }
        // 이동 및 회전 코루틴 시작
        moveCoroutine = StartCoroutine(MovingCoroutine(transform, targetPosition, moveSpeed));
    }

    private IEnumerator MovingCoroutine(Transform transform, Vector3 targetPosition, float moveSpeed)
    {
        // 목표 방향 계산
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 목표 지점까지 이동 및 회전
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360);
            yield return null;
        }
        moveCoroutine = null;
    }
}
