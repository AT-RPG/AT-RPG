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
            // �̵� �ڷ�ƾ ����
            StopCoroutine(moveCoroutine);
        }
        if (rotateCoroutine != null)
        {
            // ȸ�� �ڷ�ƾ ����
            StopCoroutine(rotateCoroutine);
        }
        // �̵� �� ȸ�� �ڷ�ƾ ����
        moveCoroutine = StartCoroutine(MovingCoroutine(transform, targetPosition, moveSpeed));
    }

    private IEnumerator MovingCoroutine(Transform transform, Vector3 targetPosition, float moveSpeed)
    {
        // ��ǥ ���� ���
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // ��ǥ �������� �̵� �� ȸ��
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360);
            yield return null;
        }
        moveCoroutine = null;
    }
}
