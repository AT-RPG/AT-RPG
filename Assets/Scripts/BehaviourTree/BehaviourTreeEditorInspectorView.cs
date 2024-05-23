#if UNITY_EDITOR

using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace AT_RPG
{
    public class BehaviourTreeEditorInspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeEditorInspectorView, VisualElement.UxmlTraits> { }

        private Editor editor;

        public BehaviourTreeEditorInspectorView()
        {

        }

        /// <summary>
        /// 노드의 기존 인스펙터 정보를 툴의 인스펙터에서 출력
        /// </summary>
        public void Update(BehaviourTreeEditorNodeView nodeView = null)
        {
            Clear();

            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
            }

            if (nodeView != null)
            {
                editor = Editor.CreateEditor(nodeView.Node);
                IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
                Add(container);
            }
        }
    }
}

#endif
