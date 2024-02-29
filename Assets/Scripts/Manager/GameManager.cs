using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// Manager 최상위 클래스
    /// </summary>
    public partial class GameManager : Singleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            InputManager.OnUpdate();
        }


        /// <summary>
        /// 첫 Scene이 로드되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashScreen()
        {

        }


        /// <summary>
        /// 첫 Scene이 로드되고, Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // 매니저 초기화
            GameManager gameManager = Instance;

            InputManager inputManager = InputManager;
            inputManager.transform.SetParent(gameManager.transform);

            ResourceManager resourceManager = ResourceManager;
            resourceManager.transform.SetParent(gameManager.transform);

            SceneManager sceneManager = SceneManager;
            sceneManager.transform.SetParent(gameManager.transform);

            UIManager uiManager = UIManager;
            uiManager.transform.SetParent(gameManager.transform);

            SaveLoadManager saveloadManager = SaveLoadManager;
            saveloadManager.transform.SetParent(gameManager.transform);

#if UNITY_EDITOR
            TestManager testManager = TestManager;
            testManager.transform.SetParent(gameManager.transform);
#endif
        }

        /// <summary>
        /// 첫 Scene이 로드되고, Awake()가 호출되고 난 후 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            // 현재 씬의 캔버스를 가져오거나 새로 생성합니다.
            UIManager.OnBeforeSceneChanged();
        }
    }

    public partial class GameManager
    {
        public static InputManager InputManager => InputManager.Instance;
        public static ResourceManager ResourceManager => ResourceManager.Instance;
        public static SceneManager SceneManager => SceneManager.Instance;
        public static UIManager UIManager => UIManager.Instance;
        public static SaveLoadManager SaveLoadManager => SaveLoadManager.Instance;
#if UNITY_EDITOR
        public static TestManager TestManager => TestManager.Instance;
#endif
    }
}
