using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// <see cref="ActionNode"/>를 테스트하는 클래스
    /// </summary>
    public class TestNode : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override NodeState OnUpdate()
        {
            Debug.Log("This is TestNode");
            return NodeState.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}