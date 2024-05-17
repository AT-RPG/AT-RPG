using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 여러개의 <see cref="ActionNode"/>만 가지는 클래스
    /// </summary>
    public class SequencerNode : BehaviourNode
    {
        public List<BehaviourNode> Children
        {
            get => children;
            set => children = value;
        }
        [SerializeField] protected List<BehaviourNode> children = new();



        private int current = 0;



        protected override void OnStart()
        {
            current = 0;
        }

        protected override NodeState OnUpdate()
        {
            switch (children[current].Update())
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Success:
                    current++;
                    if (current > children.Count - 1)
                    {
                        return NodeState.Success;
                    }
                    return NodeState.Running;

                case NodeState.Failure:
                    return NodeState.Failure;
            }

            return children[current].State;
        }

        protected override void OnEnd()
        {
            current = 0;
        }
    }
}
