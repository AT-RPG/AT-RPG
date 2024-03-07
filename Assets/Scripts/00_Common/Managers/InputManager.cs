namespace AT_RPG.Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        // 매 프레임마다 호출되는 키보드 이벤트
        private KeyMap keyMap
            = new KeyMap()
            {

            };

        protected override void Awake()
        {
            base.Awake();
        }

        public void OnUpdate()
        {
            
        }
    }

    public partial class InputManager
    {
        // 매 프레임마다 호출되는 키보드 이벤트
        public KeyMap KeyMap
        {
            get
            {
                return keyMap;
            }
            set
            {
                keyMap = value;
            }
        }
    }
}