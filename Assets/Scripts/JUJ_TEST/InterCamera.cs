using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterCamera : MonoBehaviour
{
    public LayerMask playerLayer;

    public void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("플레이어 들어옴");
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("버튼 눌림");
            }
            // 상호작용 중일 때 상호작용 중지
        }
        else
        {
            Debug.Log("나감");
        }
    }
}
