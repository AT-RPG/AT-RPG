using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRootMotion : MonoBehaviour
{
    Animator myAnim;

    Vector3 deltaPos;
    Quaternion deltaRot;
    // Start is called before the first frame update
    void Start()
    {
        myAnim= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //이동중에 벽과 충돌할경우  충돌지점까지 거리만큼 이동벡터 조정
        if (Physics.Raycast(new Ray(transform.position, deltaPos.normalized), out RaycastHit hit,
            deltaPos.magnitude, LayerMask.GetMask("Wall")))
        {
            deltaPos = deltaPos.normalized * hit.distance;
        }

        //부모객체에 이동/회전 벡터를 제어
        transform.parent.position += deltaPos;
        transform.parent.rotation *= deltaRot;

        //이동 회전 초기화
        deltaPos = Vector3.zero;
        deltaRot = Quaternion.identity;
    }


    /// <summary>
    /// 애니메이터가 업데이트될떄 호출되서 이동/회전값을 누적하여 저장함
    /// </summary>
    private void OnAnimatorMove()
    {
        deltaPos += myAnim.deltaPosition;
        deltaRot *= myAnim.deltaRotation;
    }
}
