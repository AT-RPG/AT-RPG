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


        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<UIManagerSetting>("UIManagerSettings");
        }
    }

    public partial class UIManager
    {
        // 화면을 점점 불투명하게 하는 효과
        public GameObject ScreenFadeOut => ScreenFadeOut;

        // 매니저 기본 설정
        public UIManagerSetting Setting => setting;
    }
}