using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AT_RPG.Manager
{
    // TODO - 
    // 1. UI Scale Mode
    // 2. Reference Pixel Size
    // ...
    public static class UISetting
    {

    }

    /// <summary>
    /// 현재 씬의 UI를 컨트롤
    /// </summary>
    /// TODO - 캔버스가 씬에 여러개가 있는 경우 따로 처리 필요
    public partial class UIManager : Singleton<UIManager>
    {
        // 현재 씬의 이벤트 시스템 GameObject
        [SerializeField] private GameObject eventSystemInstance = null;

        // 현재 씬의 캔버스 GameObject
        [SerializeField] private GameObject canvasInstance = null;

        // 현재 씬의 캔버스
        [SerializeField] private Canvas canvas = null;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Start()
        {
            SceneManager.Instance.SceneChangedEvent += OnSceneChanged;
        }

        /// <summary>
        /// Scene이 변경되었을 경우, 현재 씬에서 Canvas를 가져오고,
        /// 없다면 새로 생성합니다.
        /// </summary>
        public void OnSceneChanged()
        {
            // 씬에 캔버스가 없는 경우
            if (!canvas)
            {
                canvas = FindObjectOfType<Canvas>();
                if (!canvas)
                {
                    // 캔버스 GameObject 생성
                    canvasInstance = new GameObject("Canvas");
                    canvas = canvasInstance.AddComponent<Canvas>();
                    canvasInstance.AddComponent<CanvasScaler>();
                    canvasInstance.AddComponent<GraphicRaycaster>();
                    canvasInstance.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                }
                else
                {
                    canvasInstance = canvas.gameObject;
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
    }

    public partial class UIManager
    {
        // 현재 씬의 이벤트 시스템 GameObject
        public GameObject EventSystemInstance => eventSystemInstance;

        // 현재 씬의 캔버스 GameObject
        public GameObject CanvasInstance => canvasInstance;

        // 현재 씬의 캔버스
        public Canvas Canvas => canvas;
    }
}