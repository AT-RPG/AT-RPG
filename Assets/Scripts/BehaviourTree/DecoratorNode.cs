using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 하나의 <see cref="ActionNode"/>만 가지는 클래스
    /// </summary>
    public abstract class DecoratorNode : BehaviourNode
    {
        public BehaviourNode Child
        {
            get => child;
            set => child = value;
        }
        [SerializeField] protected BehaviourNode child;
    }
}