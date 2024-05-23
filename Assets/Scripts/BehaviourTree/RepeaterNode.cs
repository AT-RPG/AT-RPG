namespace AT_RPG
{
    /// <summary>
    /// <see cref="DecoratorNode.Child"/>를 반복하는 클래스
    /// </summary>
    public class RepeaterNode : DecoratorNode
    {
        protected override void OnStart()
        {
            
        }

        protected override NodeState OnUpdate()
        {
            child.Update();

            return NodeState.Running;
        }

        protected override void OnEnd()
        {

        }
    }
}