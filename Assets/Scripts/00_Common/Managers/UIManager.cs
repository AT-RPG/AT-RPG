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
        [SerializeField] private static UIManagerSetting setting;

        // 현재 씬의 모든 Canvas를 저장
        private static List<Canvas> sceneCanvases = new List<Canvas>();



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<UIManagerSetting>("UIManagerSettings");

            GameManager.AfterFirstSceneLoadAction += OnUnLoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += OnLoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += OnSortCanvases;
            GameManager.AfterFirstSceneLoadAction += OnSetupAllCanvasScalarsAsSetting;

            SceneManager.AfterSceneLoadAction += OnUnLoadAllCanvases;
            SceneManager.AfterSceneLoadAction += OnLoadAllCanvases;
            SceneManager.AfterSceneLoadAction += OnSortCanvases;
            SceneManager.AfterSceneLoadAction += OnSetupAllCanvasScalarsAsSetting;
        }



        /// <summary>
        /// 모든 캔버스 래퍼런스를 버립니다.
        /// </summary>
        private static void OnUnLoadAllCanvases()
        {
            sceneCanvases = null;
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
        /// UI매니저 설정에 있는 CanvasScalar 래퍼런스로 현재 씬의 모든 Canvas 래퍼런스 일괄 설정
        /// </summary>
        private static void OnSetupAllCanvasScalarsAsSetting()
        {
            // canvasScalar에 복사할 setting의 canvasScalarReference 리플렉션 프로퍼티
            PropertyInfo[] canvasScalarSettingProperties
                = setting.canvasScalerSettings.GetType().GetProperties(
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
                        canvasScalar, canvasScalarSettingProperties[i].GetValue(setting.canvasScalerSettings));
                }
            }
        }
    }

    public partial class UIManager
    {
        // UI매니저 기본 설정
        public UIManagerSetting Setting => setting;
    }
}