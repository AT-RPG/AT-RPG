using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;  // 캐릭터의 Transform 참조
    public float distance = 2.0f;  // 카메라와 캐릭터 사이의 거리
    public float height = 1.5f;  // 카메라의 높이
    public float rotationSpeed = 3.0f; // 카메라 회전 속도

    void Update()
    {
        // 플레이어의 위치를 기준으로 카메라의 위치 조정
        Vector3 newPosition = player.position - player.forward * distance;
        newPosition.y = player.position.y + height;  // 높이 조절
        transform.position = newPosition;
        float mouseX = Input.GetAxis("Mouse X");
        // 수평 회전
        transform.Rotate(Vector3.up, mouseX * rotationSpeed);

        // 플레이어를 바라보도록 카메라 회전
        transform.LookAt(player);
    }
}
