using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 여러개의 <see cref="ActionNode"/>를 가지는 클래스
    /// </summary>
    public abstract class CompositeNode : BehaviourNode
    {
        public List<BehaviourNode> Children
        {
            get => children;
            set => children = value;
        }
        [SerializeField] protected List<BehaviourNode> children = new();
    }
}