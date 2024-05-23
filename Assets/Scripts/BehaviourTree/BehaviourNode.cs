using System;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
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

#if UNITY_EDITOR
        public string Guid
        {
            get => guid;
            set => guid = value;
        }
        [SerializeField, HideInInspector] private string guid;

        /// <summary>
        /// 에디터에서 노드UI의 위치
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
        [SerializeField, HideInInspector] private Vector2 position = Vector2.zero;
#endif

        public virtual BehaviourNode Clone()
        {
            return Instantiate(this);
        }

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

        public abstract void AddChild(BehaviourNode child);

        public abstract void RemoveChild(BehaviourNode child);

        public abstract List<BehaviourNode> GetChildren();

        protected abstract void OnStart();

        protected abstract NodeState OnUpdate();

        protected abstract void OnEnd();
    }
}