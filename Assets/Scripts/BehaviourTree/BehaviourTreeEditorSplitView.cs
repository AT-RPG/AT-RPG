#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace AT_RPG
{
    public class BehaviourTreeEditorSplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeEditorSplitView, TwoPaneSplitView.UxmlTraits> { }

        public BehaviourTreeEditorSplitView()
        {

        }
    }
}

#endif