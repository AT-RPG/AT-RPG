using System.Collections;
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

        private UIManagerSettings setting = null;

        private float test;

        protected override void Awake()
        {
            base.Awake();

            GameManager.OnBeforeFirstSceneLoadEvent += OnBeforeFirstSceneLoad;
            GameManager.OnAfterFirstSceneLoadEvent += OnAfterSceneChangedEvent;

            SceneManager.Instance.BeforeSceneChangedCoroutine += OnBeforeSceneChangedCoroutine;
            SceneManager.Instance.AfterSceneChangedEvent += OnAfterSceneChangedEvent;
        }

        /// <summary>
        /// 필요한 리소스를 로드
        /// </summary>
        private void OnBeforeFirstSceneLoad()
        {
            setting = ResourceManager.Instance.Get<UIManagerSettings>("UIManagerSettings", true);
            if (setting == null)
            {
                Debug.LogError($"{nameof(UIManagerSettings)} 스크립터블 오브젝트가 존재X" +
                               $"bundle에 {nameof(UIManagerSettings)}이 빠져있거나, 이름이 틀립니다.");
            }
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
        /// 화면에 페이드 아웃 효과를 대기
        /// </summary>
        private IEnumerator OnBeforeSceneChangedCoroutine()
        {
            GameObject screenFadeOutInstance = Instantiate(setting.ScreenFadeOut, canvas.transform);
            FadeIn fadeOutComp = screenFadeOutInstance.GetComponent<FadeIn>();

            yield return StartCoroutine(fadeOutComp.StartFade());

            Destroy(screenFadeOutInstance);
        }

        /// <summary>
        /// 화면에 페이드 인 효과를 대기
        /// </summary>
        private IEnumerator OnAfterSceneChangedCoroutine()
        {
            GameObject screenFadeInInstance = Instantiate(setting.ScreenFadeIn, canvas.transform);
            FadeIn fadeInComp = screenFadeInInstance.GetComponent<FadeIn>();

            yield return StartCoroutine(fadeInComp.StartFade());

            Destroy(screenFadeInInstance);
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