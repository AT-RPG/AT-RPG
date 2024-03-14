using AT_RPG.Manager;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                 <br/>
    /// + 맵 설정 팝업에서 사용되는 클래스       <br/>
    /// </summary>
    public class MapSettingPopup : Popup
    {
        [Tooltip("맵 선택 팝업 프리팹")]
        [SerializeField] protected ResourceReference<GameObject> startGamePopupPrefab;

        [Tooltip("에러 로그용 프리팹")]
        [SerializeField] private ResourceReference<GameObject> logPopupPrefab;

        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        [Header("맵 설정")]
        [SerializeField] private TMP_InputField mapName;


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

            // 맵 선택 팝업을 인스턴싱합니다.
            GameObject popupInstance
                = Instantiate(startGamePopupPrefab.Resource, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupCanvas = popupCanvas;
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
        /// 맵 생성 버튼에서 사용됩니다. <br/>
        /// 팝업을 닫고, 맵 설정에 따라 맵 데이터를 생성합니다.  <br/>
        /// </summary>
        public void OnCreateMap()
        {
            if (!IsMapSettingEnable())
            {
                return;
            }
        }

        /// <summary>
        /// 맵 생성 취소 버튼에서 사용됩니다. <br/>
        /// + 맵 선택 팝업을 인스턴싱합니다. <br/>
        /// + 팝업을 닫습니다.        
        /// </summary>
        public void OnCancelCreateMap()
        {
            InvokeDestroy();
        }

        /// <summary>
        /// 맵 설정에 잘못된 부분이 없는가?
        /// </summary>
        /// <returns></returns>
        private bool IsMapSettingEnable()
        {
            // 맵 이름 있음?
            if (mapName.text.Length <= 0)
            {
                LogPopup logPopup = InstantiateLogPopup();
                logPopup.Log.text = "Can't be map name empty!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 맵 설정에서 잘못된 부분에 대한 로그를 보여주기 위해 사용됩니다.
        /// </summary>
        private LogPopup InstantiateLogPopup()
        {
            GameObject popupInstance
                = Instantiate(logPopupPrefab.Resource, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupCanvas = popupCanvas;

            return popupInstance.GetComponent<LogPopup>();
        }
    }
}