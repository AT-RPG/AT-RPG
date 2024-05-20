#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace AT_RPG
{
    public class BehaviourTreeEditorInspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeEditorInspectorView, VisualElement.UxmlTraits> { }

        public BehaviourTreeEditorInspectorView()
        {

        }
    }
}

#endif
