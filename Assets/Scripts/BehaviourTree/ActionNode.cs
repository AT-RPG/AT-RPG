using System.Collections.Generic;

namespace AT_RPG
{
    /// <summary>
    /// 실제 행동을 정의하는 클래스
    /// </summary>
    public abstract class ActionNode : BehaviourNode 
    {
        public override void AddChild(BehaviourNode child)
        {
            
        }

        public override void RemoveChild(BehaviourNode child)
        {
            
        }

        public override List<BehaviourNode> GetChildren()
        {
            return new();
        }
    }
}