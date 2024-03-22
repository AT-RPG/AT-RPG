using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 모든 씬의 캔버스 설정을 동일하게 만들어주는 클래스
    /// </summary>
    public partial class UIManager : Singleton<UIManager>
    {
        // 매니저 기본 설정
        [SerializeField] private static UIManagerSetting    setting;

        // 현재 씬의 모든 Canvas를 저장
        private static List<Canvas>                         sceneCanvases = new List<Canvas>();

        // 현재 씬의 팝업UI를 관리
        private static PopupCanvas                          popupCanvas;

        private static GameObject                           gameMenuPopupInstance;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<UIManagerSetting>("UIManagerSettings");

            InputManager.AddKeyAction("Setting/Undo", OnInstantiateGameMenuPopup);

            GameManager.AfterFirstSceneLoadAction += OnUnLoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += OnLoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += OnCreatePopupCanvas;
            GameManager.AfterFirstSceneLoadAction += OnSortCanvases;
            GameManager.AfterFirstSceneLoadAction += OnSetupAllCanvasScalarsAsSetting;

            SceneManager.AfterSceneLoadAction += OnUnLoadAllCanvases;
            SceneManager.AfterSceneLoadAction += OnLoadAllCanvases;
            SceneManager.AfterSceneLoadAction += OnCreatePopupCanvas;
            SceneManager.AfterSceneLoadAction += OnSortCanvases;
            SceneManager.AfterSceneLoadAction += OnSetupAllCanvasScalarsAsSetting;
        }

        private static void OnInstantiateGameMenuPopup(InputValue inputValue)
        {
            // 아직 팝업이 남아있는가?
            if (popupCanvas.GetPopupCount() >= 1) { return; }

            if (!gameMenuPopupInstance)
            {
                gameMenuPopupInstance = InstantiatePopup(setting.gameMenuPopupPrefab.Resource, PopupRenderMode.Hide, false);
            }
        }

        /// <summary>
        /// 모든 캔버스 래퍼런스를 버립니다.
        /// </summary>
        private static void OnUnLoadAllCanvases()
        {
            sceneCanvases = null;
            popupCanvas = null;
        }

        /// <summary>
        /// 현재 씬의 모든 캔버스 래퍼런스를 불러옵니다.
        /// </summary>
        private static void OnLoadAllCanvases()
        {
            sceneCanvases = FindObjectsOfType<Canvas>().ToList();
            if (sceneCanvases.Count >= 1)
            {
                sceneCanvases = sceneCanvases.OrderBy(canvas => canvas.sortingOrder).ToList();
            }
        }

        /// <summary>
        /// 불러온 캔버스 컨테이너를 sortingOrder순으로 정렬합니다.    <br/>
        /// + 팝업 캔버스를 sortingOrder 가장 마지막으로 설정합니다.   <br/>
        /// </summary>
        private static void OnSortCanvases()
        {
            sceneCanvases = sceneCanvases.OrderBy(canvas => canvas.sortingOrder).ToList();
        }

        /// <summary>
        /// 현재 씬에 팝업 캔버스를 생성합니다.
        /// </summary>
        private static void OnCreatePopupCanvas()
        {
            GameObject popupCanvasInstance = Instantiate(setting.popupCanvasPrefab.Resource);
            PopupCanvas = popupCanvasInstance.GetComponent<PopupCanvas>();
            sceneCanvases.Add(popupCanvasInstance.GetComponent<Canvas>());
        }

        /// <summary>
        /// UI매니저 설정에 있는 CanvasScalar 래퍼런스로 현재 씬의 모든 Canvas 래퍼런스 일괄 설정
        /// </summary>
        private static void OnSetupAllCanvasScalarsAsSetting()
        {
            // canvasScalar에 복사할 setting의 canvasScalarReference 리플렉션 프로퍼티
            PropertyInfo[] canvasScalarSettingProperties
                = setting.canvasScalerSetting.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);


            // 리플렉션으로 모든 프로퍼티를 복사
            foreach (var canvas in sceneCanvases)
            {
                var canvasScalar = canvas.gameObject.GetComponent<CanvasScaler>();
                if (!canvasScalar)
                {
                    continue;
                }

                // canvasScalar의 프로퍼티를 setting의 canvasScalarReference의 프로퍼티로 교체
                PropertyInfo[] canvasScalarProperties = canvasScalar.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (int i = 0; i < canvasScalarSettingProperties.Length; i++)
                {
                    canvasScalarProperties[i].SetValue(
                        canvasScalar, canvasScalarSettingProperties[i].GetValue(setting.canvasScalerSetting));
                }
            }
        }



        /// <summary>
        /// 다른 캔버스에 가리지 않도록 전용 UIManager 팝업 캔버스에 팝업 UI를 인스턴싱
        /// </summary>
        public static GameObject InstantiatePopup(GameObject popupPrefab, PopupRenderMode popupRenderMode, bool registerPopupStack = true)
        {
            // 팝업 인스턴싱 및 초기화
            GameObject popupInstance = Instantiate(popupPrefab, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupRenderMode = popupRenderMode;
            popup.PopupCanvas = popupCanvas;

            // 팝업 등록
            if (registerPopupStack) { popupCanvas.Push(popup);}

            return popupInstance;
        }
    }

    public partial class UIManager
    {
        // UI매니저 기본 설정
        public static UIManagerSetting Setting => setting;

        // 현재 씬의 팝업UI를 관리
        public static PopupCanvas PopupCanvas
        {
            get => popupCanvas;
            set => popupCanvas = value;
        }
    }
}