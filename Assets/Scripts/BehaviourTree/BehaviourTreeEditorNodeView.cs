#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Drawing;

namespace AT_RPG
{
    public class BehaviourTreeEditorNodeView : Node
    {
        public BehaviourNode Node;
        public Port Input;
        public Port Output;



        public BehaviourTreeEditorNodeView(BehaviourNode node)
        {
            Node = node;
            title = node.name;
            viewDataKey = node.Guid.ToString();

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPorts();
            CreateOutputPorts();
        }



        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Vector2 nextPosition = new Vector2(newPos.x, newPos.y);
            Node.Position = nextPosition;
        }



        private void CreateInputPorts()
        {
            if (Node is ActionNode)
            {
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else
            if (Node is CompositeNode)
            {
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else
            if (Node is DecoratorNode)
            {
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (Input != null)
            {
                Input.portName = "";
                inputContainer.Add(Input);
            }
        }

        private void CreateOutputPorts()
        {
            if (Node is ActionNode)
            {

            }
            else
            if (Node is CompositeNode)
            {
                Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else
            if (Node is DecoratorNode)
            {
                Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (Output != null)
            {
                Output.portName = "";
                outputContainer.Add(Output);
            }
        }
    }
}

#endif
