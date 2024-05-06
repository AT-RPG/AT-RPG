using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    Animator myAnim;
    Vector3 deltaPos;
    Quaternion deltaRot;

    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(new Ray(transform.position, deltaPos.normalized), out RaycastHit hit,
            deltaPos.magnitude, LayerMask.GetMask("Wall")))
        {
            deltaPos = deltaPos.normalized * hit.distance;
        }

        transform.parent.position += deltaPos;
        transform.parent.rotation *= deltaRot;
        deltaPos = Vector3.zero;
        deltaRot = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        deltaPos += myAnim.deltaPosition;
        deltaRot *= myAnim.deltaRotation;
    }
}

