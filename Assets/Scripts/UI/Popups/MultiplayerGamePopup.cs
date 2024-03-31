using AT_RPG.Manager;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                        <br/>
    /// + 멀티플레이 설정 팝업에서 사용되는 클래스       <br/>
    /// </summary>
    public class MultiPlayerGamePopup : Popup, IPopupDestroy
    {
        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        [Header("초대코드 입력 필드")]
        [SerializeField] private TMP_InputField inviteCodeField;


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


        /// <summary>
        /// 초대코드를 이용해서 다른 HOST의 세션에 입장합니다.
        /// </summary>
        public void OnConnectSession()
        {
            MultiplayManager.ConnectToPlayer(inviteCodeField.text, OnConnectSuccess, OnConnectFail);
        }

        /// <summary>
        /// 다른 HOST의 세션 입장에 성공합니다.
        /// </summary>
        private void OnConnectSuccess()
        {
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.MainSceneAsset.SceneName;
            string loadingScene = SceneManager.Setting.LoadingSceneAsset.SceneName;
            SceneManager.LoadScene(loadingScene, () =>
            {
                // 리소스 로딩/언로딩 + 세이브 파일 로딩
                // ResourceManager.LoadAllResourcesCoroutine(toScene);
                // ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                // 로딩이 끝나면 씬을 변경합니다.
                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
            });
        }

        /// <summary>
        /// 다른 HOST의 세션 입장에 실패했습니다.
        /// </summary>
        private void OnConnectFail()
        {
            GameObject logPopupInstance = UIManager.InstantiatePopup(UIManager.Setting.logPopupPrefab.Resource, PopupRenderMode.Default);
            LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

            logPopup.Log = $"Join failed. Invite code mismatch.";
            logPopup.Duration = 4.5f;
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