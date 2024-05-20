#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace AT_RPG
{
    public class BehaviourTreeEditorNodeView : Node
    {
        public BehaviourNode Node;

        public BehaviourTreeEditorNodeView(BehaviourNode node)
        {
            this.Node = node;
            this.title = node.name;
        }
    }
}

#endif
