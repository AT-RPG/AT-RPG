using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 바인딩된 <see cref="BehaviourTree"/>를 실행하는 클래스
    /// </summary>
    public class BehaviourTreeController : MonoBehaviour
    {
        [SerializeField] private BehaviourTree tree;



        private void Start()
        {
            tree = tree.Clone();
            tree.Start();
        }

        private void Update()
        {
            tree.Update();
        }
    }
}