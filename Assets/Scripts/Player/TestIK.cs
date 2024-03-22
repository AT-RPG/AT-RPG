using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIK : MonoBehaviour
{
    private Animator myAnim;
    [SerializeField] Transform leftHandGrip;
    [SerializeField] float ikWeight = 0.0f;

    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    void Update()
    {
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGrip.position);
        myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
    }
}
