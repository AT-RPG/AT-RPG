using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// '<see cref="Duration"/>'동안 '<see cref="NodeState.Running"/>'을 반환.           <br/>
    /// 이후 '<see cref="NodeState.Success"/>'를 반환.
    /// </summary>
    public class WaitNode : ActionNode
    {
        public float Duration
        {
            get => duration;
            set => duration = value;
        }
        [SerializeField] private float duration = 1.0f;



        private float until;


        protected override void OnStart()
        {
            until = Time.time + duration;
        }

        protected override NodeState OnUpdate()
        {
            if (Time.time > until)
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        protected override void OnEnd()
        {
            until = 0f;
        }
    }
}