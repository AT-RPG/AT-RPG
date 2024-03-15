using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                 <br/>
    /// + 맵 선택 팝업에서 사용되는 클래스       <br/>
    /// </summary>
    public class StartGamePopup : Popup
    {
        [Tooltip("맵 설정 팝업 프리팹")]
        [SerializeField] protected ResourceReference<GameObject> mapSettingPopupPrefab;

        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        private void Start()
        {
            AnimateStartSequence();
        }

        /// <summary>
        /// 팝업 종료를 요청합니다.
        /// </summary>
        public override void InvokeDestroy()
        {
            base.InvokeDestroy();

            AnimateEscapeSequence();
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

        /// <summary>
        /// 맵 새로 생성하기 버튼에서 사용됩니다. <br/>
        /// + 맵 설정 팝업을 인스턴싱합니다. <br/>
        /// + 팝업을 닫습니다.
        /// </summary>
        public void OnInstantiateMapSettingPopupButton()
        {
            GameObject popupInstance 
                = Instantiate(mapSettingPopupPrefab.Resource, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupCanvas = popupCanvas;

            InvokeDestroy();
        }
    }
}
