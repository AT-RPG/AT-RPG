using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AT_RPG
{
    /// <summary>
    /// 몬스터 AI를 정의하는 <see cref="BehaviourNode"/> 컨테이너 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "ScriptableObject/BehaviourTree", order = int.MaxValue)]
    public class BehaviourTree : ScriptableObject
    {
        /// <summary>
        /// BT의 처음 행동
        /// </summary>
        public BehaviourNode Root
        {
            get => root;
        }
        [SerializeField] private BehaviourNode root = null;
        
        /// <summary>
        /// 현재 BT에서 동작중인 행동
        /// </summary>
        public BehaviourNode Current
        {
            get => current;
        }
        [SerializeField] private BehaviourNode current = null;

        /// <summary>
        /// BT가 가지고 있는 모든 행동
        /// </summary>
        public List<BehaviourNode> Nodes
        {
            get => nodes;
        }
        [SerializeField] private List<BehaviourNode> nodes = new();

        

        public void Update()
        {
            if (current.State == NodeState.Running)
            {
                current.Update();
            }
        }

        public void AddNode(BehaviourNode node)
        {
            if (nodes.Count == 0)
            {
                root = node;
                current = node;
            }

            nodes.Add(node);
        }

        public void AddChild(BehaviourNode parent, BehaviourNode child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null)
            {
                decorator.Child = child;
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                composite.Children.Add(child);
            }
        }

        public void RemoveNode(BehaviourNode node)
        {
            nodes.Remove(node);

            if (nodes.Count == 0)
            {
                root = null;
                current = null;
            }
        }

        public void RemoveChild(BehaviourNode parent, BehaviourNode child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null)
            {
                decorator.Child = null;
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                composite.Children.Remove(child);
            }
        }

        public List<BehaviourNode> GetChildren(BehaviourNode parent)
        {
            List<BehaviourNode> children = new();

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null && decorator.Child != null)
            {
                children.Add(decorator.Child);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null && composite.Children != null)
            {
                children.AddRange(composite.Children);
            }

            return children;
        }

#if UNITY_EDITOR
        public BehaviourNode CreateNode(Type type)
        {
            if (type.IsAssignableFrom(typeof(BehaviourNode)))
            {
                throw new ArgumentException($"The type must be drived from {typeof(BehaviourNode)}");
            }

            BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
            node.name = type.Name;
            node.Guid = Guid.NewGuid();

            AddNode(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(BehaviourNode node)
        {
            RemoveNode(node);

            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}