using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// <see cref="ActionNode"/>를 테스트하는 클래스
    /// </summary>
    public class DebugNode : ActionNode
    {
        public string Msg;

        protected override void OnStart()
        {
        }

        protected override NodeState OnUpdate()
        {
            Debug.Log(Msg);
            return NodeState.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}