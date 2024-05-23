#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace AT_RPG
{
    public class BehaviourTreeEditorGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeEditorGraphView, GraphView.UxmlTraits> { }

        public Action<BehaviourTreeEditorNodeView> OnNodeSelected;

        public Action<BehaviourTreeEditorNodeView> OnNodeUnselected;

        public Action<BehaviourNode> OnSetAsRoot;

        public BehaviourTree Tree
        {
            get => tree;
        }
        private BehaviourTree tree;



        public BehaviourTreeEditorGraphView()
        {
            /// 행동 트리 툴에 그리드 추가.
            var grid = new GridBackground();
            grid.name = nameof(GridBackground);
            Insert(0, grid);

            /// 행동 트리 툴에 유틸리티 기능(ex. 확대, 스크롤) 추가.
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            /// 행동 트리 툴에 USS적용.
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviourTreeEditor.UssPath);
            styleSheets.Add(uss);
        }

        /// <summary>
        /// 현재 행동 트리 UI를 삭제.
        /// </summary>
        public void DeleteTreeView()
        {
            graphViewChanged -= OnGraphViewChange;
            DeleteElements(graphElements);
        }

        /// <summary>
        /// 행동 트리 UI를 생성.
        /// </summary>
        public void CreateTreeView(BehaviourTree tree)
        {
            this.tree = tree;
            graphViewChanged += OnGraphViewChange;

            CreateNodeViews();
            CreateEdgeViews();
        }



        /// <summary>
        /// 마우스 우클릭 시, 나타나는 메뉴.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (tree != null && tree.Root == null)
            {
                AppendRootNodeCreationAction(evt);
            }
            else
            {
                AppendDerivedNodeCreationActions<ActionNode>(evt);
                AppendDerivedNodeCreationActions<DecoratorNode>(evt);
                AppendDerivedNodeCreationActions<CompositeNode>(evt);
            }
        }

        /// <summary>
        /// 노드끼리 연결 시, 연결이 불가능한 조건을 처리.
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
                        parentView.Node.RemoveChild(childView.Node);
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
                    parentView.Node.AddChild(childView.Node);
                });
            }

            return graphViewChange;
        }



        /// <summary>
        /// 행동 트리의 모든 노드에 대한 UI를 생성.
        /// </summary>
        private void CreateNodeViews()
        {
            foreach (BehaviourNode node in tree.Nodes)
            {
                BehaviourTreeEditorNodeView nodeView = new BehaviourTreeEditorNodeView(node);
                nodeView.OnNodeSelected = OnNodeSelected;
                nodeView.OnNodeUnselected = OnNodeUnselected;
                nodeView.OnSetAsRoot = OnSetAsRoot;
                AddElement(nodeView);
            }
        }

        /// <summary>
        /// "<paramref name="node"/>"에 대한 UI를 생성.
        /// </summary>
        private void CreateNodeView(BehaviourNode node)
        {
            BehaviourTreeEditorNodeView nodeView = new BehaviourTreeEditorNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.OnNodeUnselected = OnNodeUnselected;
            nodeView.OnSetAsRoot = OnSetAsRoot;
            AddElement(nodeView);
        }

        /// <summary>
        /// 노드끼리 부모-자식 관계에 따라 연결선을 생성.
        /// </summary>
        private void CreateEdgeViews()
        {
            foreach (BehaviourNode node in tree.Nodes)
            {
                CreateEdgeViewsInternal(node);
            }
        }

        /// <summary>
        /// 노드의 자식 in 자식, 재귀적으로 연결선을 생성.
        /// </summary>
        private void CreateEdgeViewsInternal(BehaviourNode parent)
        {
            foreach (BehaviourNode child in parent.GetChildren())
            {
                BehaviourTreeEditorNodeView parentView = FindNodeView(parent);
                BehaviourTreeEditorNodeView childView = FindNodeView(child);

                Edge edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);

                CreateEdgeViewsInternal(child);
            }
        }

        /// <summary>
        /// GUID를 통해 "<paramref name="node"/>"에 UI를 검색.
        /// </summary>
        private BehaviourTreeEditorNodeView FindNodeView(BehaviourNode node)
        {
            return GetNodeByGuid(node.Guid) as BehaviourTreeEditorNodeView;
        }

        /// <summary>
        /// 마우스 우클릭 시 <typeparamref name="BehaviourNode"/>에 파생된 노드를 생성하는 목록 추가.
        /// </summary>
        private void AppendDerivedNodeCreationActions<BehaviourNode>(ContextualMenuPopulateEvent evt) where BehaviourNode : AT_RPG.BehaviourNode
        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{typeof(BehaviourNode).Name}] {type.Name}", action =>
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

        /// <summary>
        /// 마우스 우클릭 시 루트 노드를 생성하는 목록 추가.
        /// </summary>
        private void AppendRootNodeCreationAction(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"[{typeof(RootNode).Name}] {typeof(RootNode).Name}", action =>
            {
                if (tree == null)
                {
                    throw new NullReferenceException($"{nameof(BehaviourTree)} must be binded.");
                }

                var node = tree.CreateNode(typeof(RootNode));
                tree.SetRoot(node);
                CreateNodeView(node);
            });
        }
    }
}

#endif