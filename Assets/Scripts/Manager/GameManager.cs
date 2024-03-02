using UnityEngine;
using UnityEngine.Events;

namespace AT_RPG.Manager
{
    /// <summary>
    /// Manager 최상위 클래스
    /// </summary>
    public partial class GameManager : Singleton<GameManager>
    {
        // OnBeforeSplashScreen()에서 실행되는 이벤트
        private static UnityEvent onBeforeSplashScreenEvent = new UnityEvent();

        // OnBeforeFirstSceneLoad()에서 실행되는 이벤트
        private static UnityEvent onBeforeFirstSceneLoadEvent = new UnityEvent();

        // OnAfterFirstSceneLoadEvent()에서 실행되는 이벤트
        private static UnityEvent onAfterFirstSceneLoadEvent = new UnityEvent();

        // 매니저
        private static ResourceManager resourceManager = null;
        private static SceneManager sceneManager = null;
        private static UIManager uiManager = null;
        private static SaveLoadManager saveLoadManager = null;
        private static TestManager testManager = null;
        private static InputManager inputManager = null;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            inputManager.OnUpdate();
        }

        /// <summary>
        /// 첫 Scene이 로드되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashScreen()
        {
            Init();

            onBeforeSplashScreenEvent?.Invoke();
        }

        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoad()
        {
            onBeforeFirstSceneLoadEvent?.Invoke();
        }

        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되고 난 후 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterFirstSceneLoad()
        {
            onAfterFirstSceneLoadEvent?.Invoke();
        }

        /// <summary>
        /// 모든 매니저 초기화 <br/>
        /// NOTE : 초기화 순서 중요
        /// </summary>
        private static void Init()
        {
            GameManager gameManager = Instance;

            resourceManager = ResourceManager.Instance;
            resourceManager.transform.SetParent(gameManager.transform);

            sceneManager = SceneManager.Instance;
            sceneManager.transform.SetParent(gameManager.transform);

            uiManager = UIManager.Instance;
            uiManager.transform.SetParent(gameManager.transform);

            saveLoadManager = SaveLoadManager.Instance;
            saveLoadManager.transform.SetParent(gameManager.transform);

            testManager = TestManager.Instance;
            testManager.transform.SetParent(gameManager.transform);

            inputManager = InputManager.Instance;
            inputManager.transform.SetParent(gameManager.transform);
        }
    }

    public partial class GameManager
    {
        // OnBeforeSplashScreen()에서 실행되는 이벤트
        public static UnityEvent OnBeforeSplashScreenEvent
        {
            get
            {
                return onBeforeSplashScreenEvent;
            }
            set
            {
                onBeforeSplashScreenEvent = value;
            }
        }

        // OnBeforeFirstSceneLoad()에서 실행되는 이벤트
        public static UnityEvent OnBeforeFirstSceneLoadEvent
        {
            get
            {
                return onBeforeFirstSceneLoadEvent;
            }
            set
            {
                onBeforeFirstSceneLoadEvent = value;
            }
        }

        // OnAfterFirstSceneLoadEvent()에서 실행되는 이벤트
        public static UnityEvent OnAfterFirstSceneLoadEvent
        {
            get
            {
                return onAfterFirstSceneLoadEvent;
            }
            set
            {
                onAfterFirstSceneLoadEvent = value;
            }
        }
    }
}
