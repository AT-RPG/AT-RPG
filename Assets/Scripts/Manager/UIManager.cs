using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AT_RPG.Manager
{
    /// TODO - 캔버스가 씬에 여러개가 있는 경우 따로 처리 필요
    public partial class UIManager : Singleton<UIManager>
    {
        // 현재 씬의 이벤트 시스템
        private GameObject eventSystemInstance = null;

        // 현재 씬의 캔버스
        private Canvas canvas = null;



        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            SceneManager.Instance.BeforeSceneChangedEvent.AddListener(OnBeforeSceneChanged);
            SceneManager.Instance.AfterSceneChangedEvent.AddListener(OnAfterSceneChanged);
        }



        /// <summary>
        /// 씬이 변경되었을 경우, 현재 씬에서 캔버스 획득<br/>
        /// 없다면 캔버스 새로 생성
        /// </summary>
        public void OnBeforeSceneChanged()
        {
            // 씬에 캔버스가 없는 경우
            if (!canvas)
            {
                canvas = FindObjectOfType<Canvas>();
                if (!canvas)
                {
                    // 캔버스 GameObject 생성
                    GameObject canvasInstance = new GameObject("Canvas");
                    canvas = canvasInstance.AddComponent<Canvas>();
                    canvasInstance.AddComponent<CanvasScaler>();
                    canvasInstance.AddComponent<GraphicRaycaster>();
                    canvasInstance.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }

            // 씬에 이벤트 시스템이 없는 경우
            if (!eventSystemInstance)
            {
                var eventSystemComp = FindObjectOfType<EventSystem>();
                if (!eventSystemComp)
                {
                    // 이벤트 시스템 GameObject 생성
                    eventSystemInstance = new GameObject("EventSystem");
                    eventSystemInstance.AddComponent<EventSystem>();
                    eventSystemInstance.AddComponent<StandaloneInputModule>();
                }
                else
                {
                    eventSystemInstance = eventSystemComp.gameObject;
                }
            }
        }

        public void OnAfterSceneChanged()
        {
            
        }
    }

    public partial class UIManager
    {
        // 현재 씬의 이벤트 시스템
        public GameObject EventSystemInstance => eventSystemInstance;

        // 현재 씬의 캔버스
        public Canvas Canvas => canvas;
    }
}