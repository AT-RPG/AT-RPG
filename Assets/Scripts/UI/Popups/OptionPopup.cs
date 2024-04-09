using UnityEngine;
using AT_RPG.Manager;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                <br/>
    /// + 게임 설정 팝업에서 사용되는 클래스     <br/>
    /// </summary>
    public class OptionPopup : Popup, IPopupDestroy
    {
        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        [Header("종속 팝업")]
        [SerializeField] private AssetReferenceResource<GameObject> mapSettingPopup;

        [Header("옵션 메뉴 버튼")]
        [SerializeField] private GameObject continueButtonInstance;
        [SerializeField] private GameObject graphicButtonInstance;
        [SerializeField] private GameObject controlButtonInstance;
        [SerializeField] private GameObject gamesButtonInstance;

        private void Awake()
        {
            if (SceneManager.CurrentSceneName != SceneManager.Setting.MainScene) { gamesButtonInstance.SetActive(false); }
        }

        private void Start()
        {
            AnimateStartSequence();
        }

        /// <summary>
        /// 시작 애니메이션을 실행합니다.
        /// </summary>
        private void AnimateStartSequence()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
            blurAnimation.StartFade();
        }



        public void DestroyPopup()
        {
            AnimateEscapeSequence();
        }

        /// <summary>
        /// 종료 애니메이션과 함께, 현재 팝업을 삭제합니다.
        /// </summary>
        private void AnimateEscapeSequence()
        {
            popupAnimation.EndPopup();
            fadeAnimation.EndFade(() =>
            {
                Destroy(gameObject);
            });
            blurAnimation.EndFade();
        }
    }
}