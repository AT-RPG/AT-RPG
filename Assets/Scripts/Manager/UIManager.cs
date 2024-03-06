using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AT_RPG.Manager
{
    /// TODO - 캔버스가 씬에 여러개가 있는 경우 따로 처리 필요
    public partial class UIManager : Singleton<UIManager>
    {
        // 매니저 기본 설정
        [SerializeField] private UIManagerSetting setting;

        // 현재 씬의 이벤트 시스템
        private GameObject eventSystemInstance = null;

        // 현재 씬의 캔버스
        private Canvas canvas = null;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<UIManagerSetting>("UIManagerSettings");

            GameManager.OnAfterFirstSceneLoadEvent += OnAfterSceneChangedEvent;
            SceneManager.Instance.BeforeSceneChangedCoroutine += OnBeforeSceneChangedCoroutine;
            SceneManager.Instance.AfterSceneChangedEvent += OnAfterSceneChangedEvent;
        }



        /// <summary>
        /// 씬이 변경되었을 경우, 현재 씬에서 캔버스 획득<br/>
        /// 없다면 캔버스 새로 생성
        /// </summary>
        private void OnAfterSceneChangedEvent()
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

        /// <summary>
        /// 화면이 점점 검게 변하는 효과 씬 로딩 이벤트에 추가
        /// </summary>
        private IEnumerator OnBeforeSceneChangedCoroutine()
        {
            GameObject screenFadeOutInstance = Instantiate(setting.ScreenFadeOutInstance, canvas.transform);
            FadeInImage fadeOutComp = screenFadeOutInstance.GetComponent<FadeInImage>();

            yield return StartCoroutine(fadeOutComp.StartFade());
        }
    }

    public partial class UIManager
    {
        // 현재 씬의 이벤트 시스템
        public GameObject EventSystemInstance => eventSystemInstance;

        // 현재 씬의 캔버스
        public Canvas Canvas => canvas;

        // 화면을 점점 불투명하게 하는 효과
        public GameObject ScreenFadeOut => ScreenFadeOut;

        // 매니저 기본 설정
        public UIManagerSetting Setting => setting;
    }
}