using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObjectStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("test");
    }
}
public class BuildingObjectStateMachine1 : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("test1");
    }
}