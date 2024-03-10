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
        private static List<Canvas> SceneCanvases = new List<Canvas>();



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<UIManagerSetting>("UIManagerSettings");

            GameManager.AfterFirstSceneLoadAction += UnLoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += LoadAllCanvases;
            GameManager.AfterFirstSceneLoadAction += SetupAllCanvasScalarsAsSetting;
        }



        /// <summary>
        /// 현재 씬의 모든 Canvas 래퍼런스를 버립니다.
        /// </summary>
        public static void UnLoadAllCanvases()
        {
            SceneCanvases = null;
        }

        /// <summary>
        /// 현재 씬의 모든 Canvas 래퍼런스를 Canvas.sortingOrder순으로 SceneCanvases에 저장
        /// </summary>
        public static void LoadAllCanvases()
        {
            SceneCanvases = FindObjectsOfType<Canvas>().ToList();
            if (SceneCanvases.Count >= 1)
            {
                SceneCanvases = SceneCanvases.OrderBy(canvas => canvas.sortingOrder).ToList();
            }
        }

        /// <summary>
        /// UI매니저 설정에 있는 CanvasScalar 래퍼런스로 현재 씬의 모든 Canvas 래퍼런스 일괄 설정
        /// </summary>
        public static void SetupAllCanvasScalarsAsSetting()
        {
            // canvasScalar에 복사할 setting의 canvasScalarReference 리플렉션 프로퍼티
            PropertyInfo[] canvasScalarSettingProperties
                = setting.CanvasScalerSettingReference.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);


            // 리플렉션으로 모든 프로퍼티를 복사
            foreach (var canvas in SceneCanvases)
            {
                var canvasScalar = canvas.gameObject.GetComponent<CanvasScaler>();
                if (!canvasScalar)
                {
                    continue;
                }

                // canvasScalar의 프로퍼티를 setting의 canvasScalarReference의 프로퍼티로 교체
                PropertyInfo[] canvasScalarProperties = canvasScalar.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (int i =0; i < canvasScalarSettingProperties.Length; i++)
                {
                    canvasScalarProperties[i].SetValue(
                        canvasScalar, canvasScalarSettingProperties[i].GetValue(setting.CanvasScalerSettingReference));
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