#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace AT_RPG
{
    public class BehaviourTreeEditorGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeEditorGraphView, GraphView.UxmlTraits> { }

        public BehaviourTreeEditorGraphView()
        {
            var grid = new GridBackground();
            grid.name = nameof(GridBackground);
            Insert(0, grid);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviourTreeEditor.UssPath);
            styleSheets.Add(uss);
        }
    }
}

#endif