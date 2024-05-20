#if UNITY_EDITOR

using System;
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

        public BehaviourTree Tree
        {
            get => tree;
        }
        private BehaviourTree tree;

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

        /// <summary>
        /// '<see cref="BehaviourTreeEditorGraphView"/>' 우클릭 시, 나타나는 메뉴.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);

            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", action =>
                    {
                        var node = tree.CreateNode(type);
                        CreateNodeView(node);
                    });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", action =>
                    {
                        var node = tree.CreateNode(type);
                        CreateNodeView(node);
                    });
                }
            }
        }

        /// <summary>
        /// <see cref="BehaviourTree"/>를 UI로 생성.
        /// </summary>
        public void CreateView(BehaviourTree tree)
        {
            /// 이전 정보 삭제.
            graphViewChanged -= OnGraphViewChange;
            DeleteElements(graphElements);

            /// 새로운 tree UI 불러오기.
            this.tree = tree;
            graphViewChanged += OnGraphViewChange;
            tree.Nodes.ForEach(n => CreateNodeView(n));
        }

        /// <summary>
        /// NodeView에 대한 'Delete' 기능을 구현.
        /// </summary>
        private GraphViewChange OnGraphViewChange(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    BehaviourTreeEditorNodeView nodeView = element as BehaviourTreeEditorNodeView;
                    if (nodeView != null)
                    {
                        tree.DeleteNode(nodeView.Node);
                    }
                });
            }

            return graphViewChange;
        }

        /// <summary>
        /// <see cref="BehaviourNode"/>가 UI로 보일 수 있도록 생성.
        /// </summary>
        private void CreateNodeView(BehaviourNode node)
        {
            BehaviourTreeEditorNodeView nodeView = new BehaviourTreeEditorNodeView(node);
            AddElement(nodeView);
        }
    }
}

#endif