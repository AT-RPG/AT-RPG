using UnityEngine;

namespace AT_RPG.Manager
{
    public partial class GameManager : Singleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 첫 Scene이 로드되고, Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            GameManager gameManager = Instance;
            InputManager inputManager = InputManager;
            ResourceManager resourceManager = ResourceManager;
            SceneManager sceneManager = SceneManager;
            DataManager dataManager = DataManager;
        }
    }

    public partial class GameManager
    {
        public static ResourceManager ResourceManager => ResourceManager.Instance;
        public static SceneManager SceneManager => SceneManager.Instance;
        public static DataManager DataManager => DataManager.Instance;
        public static InputManager InputManager => InputManager.Instance;
    }
}
