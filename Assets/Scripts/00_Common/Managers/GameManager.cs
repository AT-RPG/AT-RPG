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


        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<GameManagerSettings>("GameManagerSettings");
        }



        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되기 전에 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoad()
        {
            Init();
            DOTween.Init();

            PreloadResource();

            beforeFirstSceneLoadAction?.Invoke();
        }

        /// <summary>
        /// 모든 매니저 초기화 <br/>
        /// NOTE : 초기화 순서 중요
        /// </summary>
        private static void Init()
        {
            GameManager gameManager = GetInstance();

            multiplayManager = MultiplayManager.GetInstance();

            inputManager = InputManager.GetInstance();

            sceneManager = SceneManager.GetInstance();

            uiManager = UIManager.GetInstance();

            dataManager = SaveLoadManager.GetInstance();

            resourceManager = ResourceManager.GetInstance();
        }

        /// <summary>
        /// <see cref="GameManager.setting"/>에 등록된 어드레서블 라벨을 로드
        /// </summary>
        private static void PreloadResource()
        {
            foreach (var label in setting.PreloadAddressableLabelMap)
            {
                ResourceManager.LoadAssetsAsync(label.labelString, objects => Debug.Log($"{label.labelString} 로드 성공"), false);
            }
        }



        /// <summary>
        /// 첫 Scene이 로드되고, Hierarchy에 있는 GameObject들 Awake()가 호출되고 난 후 실행
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterFirstSceneLoad()
        {
            afterFirstSceneLoadAction?.Invoke();
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
