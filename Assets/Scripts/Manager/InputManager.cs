using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace AT_RPG.Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        private event Action keyEvent;

        protected override void Awake()
        {
            base.Awake();
        }

        public void OnUpdate()
        {
            keyEvent?.Invoke();
        }
    }

    public partial class InputManager
    {
        /// <summary>
        /// 매 프레임마다 키보드 이벤트를 Invoke합니다.
        /// </summary>
        public Action KeyEvent => keyEvent;
    }
}