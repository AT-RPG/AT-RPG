using System;
using UnityEngine;
using DG.Tweening;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 매니저들을 게임 시작전에 초기화합니다.
    /// </summary>
    public partial class GameManager : Singleton<GameManager>
    {
        private static GameManagerSettings setting;

        // LoadAllResourcesFromResourcesFolder()에서 실행
        private static event Action beforeFirstSceneLoadAction;

        // OnAfterFirstSceneLoad()에서 실행
        private static event Action afterFirstSceneLoadAction;

        // 매니저
        private static ResourceManager resourceManager      = null;
        private static SceneManager sceneManager            = null;
        private static UIManager uiManager                  = null;
        private static SaveLoadManager dataManager          = null;
        private static InputManager inputManager            = null;
        private static MultiplayManager multiplayManager    = null;

        // 매니저 안에서 사용되는 eventManager 변수
        private EventManager eventManager                   = null;
        // 매니저 안에서 사용되는 playerData 변수
        private PlayerData player                           = null;
        // 매니저 안에서 사용되는 loadCSVDataManager 변수
        private LoadCSVDataManager loadCSVDataManager       = null;
        // 매니저 안에서 사용되는 Inventory 변수
        // private Inventory inventory                         = null;

        // 매니저를 통해 PlayerData로 접근할 프로퍼티 변수
        public static PlayerData Player { get => Instance.player; }
        // 매니저를 통해 Inventoroy로 접근할 프로퍼티 변수
        // public static Inventory Inventory { get => Instance.inventory; }
        // Action변수들을 관리하는 EventManager로 접근할 수 있는 변수
        public static EventManager Event { get => Instance.eventManager; }
        // CSV 초기 데이터를 불러와 저장해두고 사용할 수 있게 만든 클래스로 접근할 수 있는 변수
        public static LoadCSVDataManager LoadCSVData { get => Instance.loadCSVDataManager; }

        protected override void Awake()
        {
            base.Awake();
            setting = Resources.Load<GameManagerSettings>("GameManagerSettings");
            loadCSVDataManager = new();
            eventManager = new();
            player = new();
            // inventory = new();
        }



        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되기 전에 실행
        // </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoad()
        {
            Init();
            DOTween.Init();

            PreloadResources();

            beforeFirstSceneLoadAction?.Invoke();
        }

        /// <summary>
        /// 게임시작전에 로드할 어드레서블 라벨입니다.
        /// </summary>
        private static void PreloadResources()
        {
            foreach (var label in setting.PreloadAddressableLabelMap) { ResourceManager.LoadAssets(label.labelString); }
        }



        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되고 난 후 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterFirstSceneLoad()
        {
            afterFirstSceneLoadAction?.Invoke();
        }



        /// <summary>
        /// 모든 매니저 초기화
        /// </summary>
        /// <remarks>
        /// NOTE : 초기화 순서 중요
        /// </remarks>
        private static void Init()
        {
            GameManager gameManager = GetInstance();

            multiplayManager = MultiplayManager.GetInstance();

            inputManager = InputManager.GetInstance();

            resourceManager = ResourceManager.GetInstance();

            sceneManager = SceneManager.GetInstance();

            uiManager = UIManager.GetInstance();

            dataManager = SaveLoadManager.GetInstance();
        }

        public static T Bind<T>(string _targetObjectName, Transform _rootTransfrom = null) where T : Component
        {
            if (_rootTransfrom == null)
                _rootTransfrom = GameObject.FindWithTag("DynamicCanvas").transform;
            T[] results = _rootTransfrom.GetComponentsInChildren<T>(true);

            foreach (var item in results)
                if (item.gameObject.name.Equals(_targetObjectName))
                    return item;

            return null;
        }
    }

    public partial class GameManager
    {
        // LoadAllResourcesFromResourcesFolder()에서 실행
        public static Action BeforeFirstSceneLoadAction
        {
            get
            {
                return beforeFirstSceneLoadAction;
            }
            set
            {
                beforeFirstSceneLoadAction = value;
            }
        }

        // AfterFirstSceneLoadAction()에서 실행
        public static Action AfterFirstSceneLoadAction
        {
            get
            {
                return afterFirstSceneLoadAction;
            }
            set
            {
                afterFirstSceneLoadAction = value;
            }
        }
    }
}
