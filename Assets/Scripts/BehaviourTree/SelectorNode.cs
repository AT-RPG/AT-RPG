using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 여러개의 <see cref="ActionNode"/>들을 차례대로 실행하며, 각 자식 노드가 실패할 때 다음 자식 노드로 넘어갑니다.
    /// </summary>
    public class SelectorNode : BehaviourNode
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
                    return NodeState.Success;

                case NodeState.Failure:
                    current++;
                    if (current > children.Count - 1)
                    {
                        return NodeState.Failure;
                    }
                    return NodeState.Running;
            }

            return children[current].State;
        }

        protected override void OnEnd()
        {
            current = 0;
        }
    }
}
