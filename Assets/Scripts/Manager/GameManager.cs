using System;
using UnityEngine;
using DG.Tweening;

namespace AT_RPG.Manager
{
    /// <summary>
    /// Manager 최상위 클래스
    /// </summary>
    public partial class GameManager : Singleton<GameManager>
    {

        // OnBeforeFirstSceneLoad()에서 실행되는 이벤트
        private static event Action onBeforeFirstSceneLoadEvent;

        // OnAfterFirstSceneLoadEvent()에서 실행되는 이벤트
        private static event Action onAfterFirstSceneLoadEvent;

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
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoad()
        {
            Init();
            DOTween.Init();

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
            GameManager gameManager = GetInstance();

            resourceManager = ResourceManager.GetInstance();
            resourceManager.transform.SetParent(gameManager.transform);

            sceneManager = SceneManager.GetInstance();
            sceneManager.transform.SetParent(gameManager.transform);

            uiManager = UIManager.GetInstance();
            uiManager.transform.SetParent(gameManager.transform);

            saveLoadManager = SaveLoadManager.GetInstance();
            saveLoadManager.transform.SetParent(gameManager.transform);

            testManager = TestManager.GetInstance();
            testManager.transform.SetParent(gameManager.transform);

            inputManager = InputManager.GetInstance();
            inputManager.transform.SetParent(gameManager.transform);
        }
    }

    public partial class GameManager
    {
        // OnBeforeFirstSceneLoad()에서 실행되는 이벤트
        public static Action OnBeforeFirstSceneLoadEvent
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
        public static Action OnAfterFirstSceneLoadEvent
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
