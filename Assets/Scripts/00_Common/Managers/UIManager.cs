using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 모든 씬의 캔버스 설정을 동일하게 만들어주는 클래스
    /// </summary>
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