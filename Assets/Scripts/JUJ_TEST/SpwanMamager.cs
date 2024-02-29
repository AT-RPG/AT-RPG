using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanMamager : MonoBehaviour
{
    public Transform newSpawnPoint;  // Inspector에서 다른 스폰 지점을 할당할 변수

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 스폰 지점에 들어왔을 때
        if (other.CompareTag("Player"))
        {
            // 플레이어가 다른 스폰 지점으로 이동
            if (newSpawnPoint != null)
            {
                other.GetComponent<Player>().MoveToNewSpawnPoint(newSpawnPoint);
            }
        }
    }
}
