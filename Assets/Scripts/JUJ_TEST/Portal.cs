using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform portal;

    // 원하는 거리
    public float distanceOffset = 2f;

    void OnCollisionEnter(Collision other)
    {
        // 출구와 플레이어 사이의 방향 벡터 계산
        Vector3 exitDirection = portal.position - transform.position;
        exitDirection.Normalize(); // 방향 벡터 정규화

        // 플레이어를 원하는 거리만큼 더해 새로운 위치 설정
        Vector3 newPosition = portal.position + exitDirection * distanceOffset;

        // 플레이어 위치를 새로운 위치로 설정
        other.transform.position = newPosition;
    }
}