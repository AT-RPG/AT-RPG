using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

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

        public override BehaviourNode Clone()
        {
            DecoratorNode clone = Instantiate(this);
            clone.child = child.Clone();

            return clone;
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