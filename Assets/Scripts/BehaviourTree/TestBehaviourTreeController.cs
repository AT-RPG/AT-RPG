using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 바인딩된 <see cref="BehaviourTree"/>를 실행하는 클래스
    /// </summary>
    public class TestBehaviourTreeController : MonoBehaviour
    {
        [SerializeField] private BehaviourTree tree;



        private void Start()
        {
            tree = ScriptableObject.CreateInstance<BehaviourTree>();

            RepeaterNode repeat = ScriptableObject.CreateInstance<RepeaterNode>();
            {
                SequencerNode seq1 = ScriptableObject.CreateInstance<SequencerNode>();
                {
                    TestNode test1 = ScriptableObject.CreateInstance<TestNode>();
                    seq1.Children.Add(test1);

                    WaitNode wait1 = ScriptableObject.CreateInstance<WaitNode>();
                    seq1.Children.Add(wait1);

                    TestNode test2 = ScriptableObject.CreateInstance<TestNode>();
                    seq1.Children.Add(test2);

                    WaitNode wait2 = ScriptableObject.CreateInstance<WaitNode>();
                    seq1.Children.Add(wait2);

                    TestNode test3 = ScriptableObject.CreateInstance<TestNode>();
                    seq1.Children.Add(test3);

                    WaitNode wait3 = ScriptableObject.CreateInstance<WaitNode>();
                    seq1.Children.Add(wait3);
                }

                repeat.Child = seq1;
            }
            tree.AddNode(repeat);

        }

        private void Update()
        {
            tree.Update();
        }
    }
}