using UnityEngine;

namespace AT_RPG
{
    public abstract class BehaviourNode : ScriptableObject
    {
        public NodeState State
        {
            get => state;
            set => state = value;
        }
        protected NodeState state = NodeState.Running;

        public bool IsStarted
        {
            get => isStarted;
        }
        private bool isStarted = false;



        public NodeState Update()
        {
            if (!isStarted)
            {
                OnStart();
                isStarted = true;
            }

            state = OnUpdate();

            if (state == NodeState.Success || state == NodeState.Failure)
            {
                OnEnd();
                isStarted = false;
            }

            return state;
        }

        protected abstract void OnStart();

        protected abstract NodeState OnUpdate();

        protected abstract void OnEnd();
    }
}