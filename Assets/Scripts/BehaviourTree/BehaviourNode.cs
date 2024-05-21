using System;
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

        public Guid Guid
        {
            get => guid;
            set => guid = value;
        }
        private Guid guid = Guid.Empty;

#if UNITY_EDITOR
        /// <summary>
        /// 에디터에서 노드UI의 위치
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
        private Vector2 position = Vector2.zero;
#endif

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