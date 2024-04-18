using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // x,y,z축을 inspector창에 출력 => 미니맵에 사용하기 위함
    [SerializeField]
    private Transform target; // 타겟 설정 (플레이어)
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (!target) return;
        transform.position = new Vector3(
            (x ? target.position.x : transform.position.x), //타겟의 x,y,z 축을 따라서 움직임
            (y ? target.position.y + 10f : transform.position.y),
            (z ? target.position.z : transform.position.z));
    }
}
