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
        // LoadAllResourcesFromResourcesFolder()에서 실행
        private static event Action beforeFirstSceneLoadAction;

        // OnAfterFirstSceneLoad()에서 실행
        private static event Action afterFirstSceneLoadAction;

        // 매니저
        private static ResourceManager resourceManager  = null;
        private static SceneManager sceneManager        = null;
        private static UIManager uiManager              = null;
        private static DataManager dataManager      = null;
        private static TestManager testManager          = null;
        private static InputManager inputManager        = null;



        protected override void Awake()
        {
            base.Awake();
        }



        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoad()
        {
            Init();
            DOTween.Init();

            beforeFirstSceneLoadAction?.Invoke();
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
        /// 모든 매니저 초기화 <br/>
        /// NOTE : 초기화 순서 중요
        /// </summary>
        private static void Init()
        {
            GameManager gameManager = GetInstance();

            resourceManager = ResourceManager.GetInstance();
            resourceManager.transform.SetParent(gameManager.transform);

            sceneManager = SceneManager.GetInstance();
            sceneManager.transform.SetParent(gameManager.transform);

            uiManager = UIManager.GetInstance();
            uiManager.transform.SetParent(gameManager.transform);

            dataManager = DataManager.GetInstance();
            dataManager.transform.SetParent(gameManager.transform);

            testManager = TestManager.GetInstance();
            testManager.transform.SetParent(gameManager.transform);

            inputManager = InputManager.GetInstance();
            inputManager.transform.SetParent(gameManager.transform);
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
