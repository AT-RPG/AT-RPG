using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AT_RPG
{
    /// <summary>
    /// BT의 시작 노드
    /// </summary>
    public class RootNode : BehaviourNode
    {
        public BehaviourNode Child
        {
            get => child;
            set => child = value;
        }
        [SerializeField] private BehaviourNode child;

        protected override void OnStart()
        {

        }

        protected override NodeState OnUpdate()
        {
            return child.Update();
        }

        protected override void OnEnd()
        {

        }

        public override void AddChild(BehaviourNode child)
        {
            this.child = child;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(child);
            AssetDatabase.SaveAssets();
#endif 
        }

        public override void RemoveChild(BehaviourNode child)
        {
            this.child = null;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(child);
            AssetDatabase.SaveAssets();
#endif 
        }

        public override List<BehaviourNode> GetChildren()
        {
            List<BehaviourNode> children = new() { child };
            return children;
        }
    }
}
