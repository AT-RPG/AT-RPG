using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void Add(BehaviourNode node)
        {
            if (nodes.Count == 0)
            {
                root = node;
                current = node;
            }

            nodes.Add(node);
        }

        public void Remove(BehaviourNode node)
        {
            nodes.Remove(node);

            if (nodes.Count == 0)
            {
                root = null;
                current = null;
            }
        }
    }
}