#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

namespace AT_RPG
{
    public partial class BehaviourTreeEditorNodeView
    {
        public static readonly string UxmlPath = "Assets/Scripts/BehaviourTree/BehaviourTreeEditorNodeView.uxml";
    }


    public partial class BehaviourTreeEditorNodeView : Node
    {
        /// <summary>
        /// NodeView가 선택될 때 호출.
        /// </summary>
        public Action<BehaviourTreeEditorNodeView> OnNodeSelected;

        /// <summary>
        /// NodeView가 선택해제될 때 호출.
        /// </summary>
        public Action<BehaviourTreeEditorNodeView> OnNodeUnselected;

        public Action<BehaviourNode> OnSetAsRoot;

        /// <summary>
        /// NodeView의 실제 인스턴스.
        /// </summary>
        public BehaviourNode Node;

        /// <summary>
        /// 부모관계 노드 연결부.
        /// </summary>
        public Port Input;

        /// <summary>
        /// 자식관계 노드 연결부.
        /// </summary>
        public Port Output;



        public BehaviourTreeEditorNodeView(BehaviourNode node) : base(UxmlPath)
        {
            Node = node;
            title = node.name;
            viewDataKey = node.Guid.ToString();

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPorts();
            CreateOutputPorts();
        }



        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction($"Set as root", action =>
            {
                OnSetAsRoot?.Invoke(Node);
            });
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Vector2 nextPosition = new Vector2(newPos.x, newPos.y);
            Node.Position = nextPosition;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            OnNodeUnselected?.Invoke(this);
        }



        private void CreateInputPorts()
        {
            switch (Node)
            {
                case RootNode:
                    break;

                case ActionNode:
                    Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(BehaviourNode));
                    break;

                case CompositeNode:
                    Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(BehaviourNode));
                    break;

                case DecoratorNode:
                    Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(BehaviourNode));
                    break;
            }

            if (Input != null)
            {
                Input.portName = "";
                Input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(Input);
            }
        }

        private void CreateOutputPorts()
        {
            switch (Node)
            {
                case RootNode:
                    Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(BehaviourNode));
                    break;

                case ActionNode:
                    break;

                case CompositeNode:
                    Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(BehaviourNode));
                    break;

                case DecoratorNode:
                    Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(BehaviourNode));
                    break;
            }

            if (Output != null)
            {
                Output.portName = "";
                Output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(Output);
            }
        }
    }
}

#endif
