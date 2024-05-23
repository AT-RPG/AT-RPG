using System;
using System.Collections.Generic;
using UnityEngine;
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
            set => root = value;
        }
        [SerializeField] private BehaviourNode root = null;   

        /// <summary>
        /// BT가 가지고 있는 모든 행동
        /// </summary>
        public List<BehaviourNode> Nodes
        {
            get => nodes;
        }
        [SerializeField] private List<BehaviourNode> nodes = new();

        /// <summary>
        /// 현재 BT에서 동작중인 행동
        /// </summary>
        public BehaviourNode Current
        {
            get => current;
        }
        private BehaviourNode current = null;


        public void Start()
        {
            current = root;
        }

        public void Update()
        {
            if (current.State == NodeState.Running)
            {
                current.Update();
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree clone = Instantiate(this);
            clone.nodes.ConvertAll(node => node.Clone());
            return clone;
        }

        public void SetRoot(BehaviourNode node)
        {
            root = node;
        }

        public void AddNode(BehaviourNode node)
        {
            nodes.Add(node);
        }

        public void RemoveNode(BehaviourNode node)
        {
            if (node == root)
            {
                root = null;
            }

            nodes.Remove(node);
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
            node.Guid = GUID.Generate().ToString();

            AddNode(node);

            AssetDatabase.AddObjectToAsset(node, this);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(BehaviourNode node)
        {
            RemoveNode(node);

            AssetDatabase.RemoveObjectFromAsset(node);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}