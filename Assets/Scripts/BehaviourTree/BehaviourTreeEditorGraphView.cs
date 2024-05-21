#if UNITY_EDITOR

using System;
using System.Linq;
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


        /// <summary>
        /// BT의 Graph부분을 만들고 uss에 저장.
        /// </summary>
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
        /// 현재 Graph View를 초기화.
        /// </summary>
        public void DeleteView()
        {
            graphViewChanged -= OnGraphViewChange;
            DeleteElements(graphElements);
        }

        /// <summary>
        /// <see cref="BehaviourTree"/>를 UI로 생성.
        /// </summary>
        public void CreateView(BehaviourTree tree)
        {
            this.tree = tree;
            graphViewChanged += OnGraphViewChange;

            CreateNodeViews();
            CreateEdgeViews();
        }



        /// <summary>
        /// '<see cref="BehaviourTreeEditorGraphView"/>' 우클릭 시, 나타나는 메뉴.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            AppendNodeCreationActions<ActionNode>(evt);
            AppendNodeCreationActions<DecoratorNode>(evt);
            AppendNodeCreationActions<CompositeNode>(evt);
        }

        /// <summary>
        /// 노드끼리 연결 시, 예외를 처리.
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }



        /// <summary>
        /// BehaviourTree에 변화가 생기는 경우 트리거.
        /// </summary>
        private GraphViewChange OnGraphViewChange(GraphViewChange graphViewChange)
        {
            /// 노드를 'Delete'시 삭제.
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    BehaviourTreeEditorNodeView nodeView = element as BehaviourTreeEditorNodeView;
                    if (nodeView != null)
                    {
                        tree.DeleteNode(nodeView.Node);
                    }

                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        var parentView = edge.output.node as BehaviourTreeEditorNodeView;
                        var childView = edge.input.node as BehaviourTreeEditorNodeView;
                        tree.RemoveChild(parentView.Node, childView.Node);
                    }
                });
            }

            /// 노드끼리 연결을 구현.
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    var parentView = edge.output.node as BehaviourTreeEditorNodeView;
                    var childView = edge.input.node as BehaviourTreeEditorNodeView;
                    tree.AddChild(parentView.Node, childView.Node);
                });
            }

            return graphViewChange;
        }



        /// <summary>
        /// <see cref="BehaviourNode"/>가 UI로 보일 수 있도록 생성.
        /// </summary>
        private void CreateNodeViews()
        {
            foreach (BehaviourNode node in tree.Nodes)
            {
                BehaviourTreeEditorNodeView nodeView = new BehaviourTreeEditorNodeView(node);
                AddElement(nodeView);
            }
        }

        /// <summary>
        /// <see cref="BehaviourNode"/>가 UI로 보일 수 있도록 생성.
        /// </summary>
        private void CreateNodeView(BehaviourNode node)
        {
            BehaviourTreeEditorNodeView nodeView = new BehaviourTreeEditorNodeView(node);
            AddElement(nodeView);
        }

        /// <summary>
        /// <see cref="BehaviourNode"/>끼리 연결된 라인이 보일 수 있도록 생성.
        /// </summary>
        private void CreateEdgeViews()
        {
            foreach (BehaviourNode node in tree.Nodes)
            {
                CreateEdgeViewsInternal(node);
            }
        }

        /// <summary>
        /// <see cref="tree"/>가 가지고 있는 노드의 자식까지 재귀적으로 연결선을 생성.
        /// </summary>
        private void CreateEdgeViewsInternal(BehaviourNode parent)
        {
            foreach (BehaviourNode child in tree.GetChildren(parent))
            {
                BehaviourTreeEditorNodeView parentView = FindNodeView(parent);
                BehaviourTreeEditorNodeView childView = FindNodeView(child);

                Edge edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);

                CreateEdgeViewsInternal(child);
            }
        }

        /// <summary>
        /// GUID를 통해 <paramref name="node"/>에 바인딩된 UI를 검색.
        /// </summary>
        private BehaviourTreeEditorNodeView FindNodeView(BehaviourNode node)
        {
            return GetNodeByGuid(node.Guid.ToString()) as BehaviourTreeEditorNodeView;
        }

        /// <summary>
        /// 마우스 우클릭 시 나타나는 Node 생성 목록을 추가.
        /// </summary>
        private void AppendNodeCreationActions<T>(ContextualMenuPopulateEvent evt) where T : BehaviourNode
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{typeof(T).Name}] {type.Name}", action =>
                {
                    if (tree == null)
                    {
                        throw new NullReferenceException($"{nameof(BehaviourTree)} must be binded.");
                    }

                    var node = tree.CreateNode(type);
                    CreateNodeView(node);
                });
            }
        }
    }
}

#endif