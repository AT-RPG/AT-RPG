using System;


namespace AT_RPG.Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        // 매 프레임마다 호출되는 키보드 이벤트
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
        // 매 프레임마다 호출되는 키보드 이벤트
        public Action KeyEvent
        {
            get
            {
                return keyEvent;
            }
            set
            {
                keyEvent = value;
            }
        }
    }
}