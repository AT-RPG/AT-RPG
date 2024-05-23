using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public override BehaviourNode Clone()
        {
            CompositeNode clone = Instantiate(this);
            clone.children.ConvertAll(child => child.Clone());
            return clone;
        }

        public override void AddChild(BehaviourNode child)
        {
            children.Add(child);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(child);
            AssetDatabase.SaveAssets();
#endif 
        }

        public override void RemoveChild(BehaviourNode child)
        {
            children.Remove(child);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(child);
            AssetDatabase.SaveAssets();
#endif 
        }

        public override List<BehaviourNode> GetChildren()
        {
            return children;
        }
    }
}