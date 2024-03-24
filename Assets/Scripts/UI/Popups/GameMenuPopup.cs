using AT_RPG.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 인게임 플레이중에 ESC를 통해서 인스턴싱되는 게임 메뉴 팝업 클래스
    /// </summary>
    public class GameMenuPopup : Popup, IPopupDestroy
    {
        [Header("종속된 팝업")]
        [SerializeField] private ResourceReference<GameObject>  optionPopupPrefab;
        [SerializeField] private ResourceReference<GameObject>  worldSettingPopupPrefab;

        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation            fadeAnimation;
        [SerializeField] private PopupCanvasAnimation           popupAnimation;
        [SerializeField] private BlurCanvasAnimation            blurAnimation;

        [Header("게임 메뉴 버튼")]
        [SerializeField] private GameObject                     continueButtonInstance;
        [SerializeField] private GameObject                     saveButtonInstance;
        [SerializeField] private GameObject                     inviteButtonInstance;
        [SerializeField] private GameObject                     optionButtonInstance;
        [SerializeField] private GameObject                     titleButtonInstance;
        [SerializeField] private GameObject                     quitButtonInstance;

        private bool isEscaped = false;

        

        private void Awake()
        {
            if (SceneManager.CurrentSceneName == SceneManager.Setting.TitleScene) 
            { 
                titleButtonInstance.SetActive(false);
                saveButtonInstance.SetActive(false);
                inviteButtonInstance.SetActive(false);
            }         

            if (MultiplayManager.PlayMode == PlayMode.Client)
            {
                saveButtonInstance.SetActive(false);
                inviteButtonInstance.SetActive(false);
            }
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



        private void Update()
        {
            if (!isEscaped && Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyPopup();
            }
        }



        /// <summary>
        /// 게임 메뉴 팝업을 종료 합니다.
        /// </summary>
        public void OnDestroyPopup()
        {
            DestroyPopup();
        }



        /// <summary>
        /// 현재 게임의 맵 설정과 저장 대상인 게임 오브젝트를 저장합니다.
        /// </summary>
        public void OnSaveMap()
        {
            PlayMode currentPlayMode = MultiplayManager.PlayMode;
            if (currentPlayMode == PlayMode.Single || currentPlayMode == PlayMode.Host) { SaveWorld(); }

            GameObject logPopupInstance = UIManager.InstantiatePopup(UIManager.Setting.logPopupPrefab.Resource, PopupRenderMode.Default, false);
            LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

            logPopup.Log = $"Save Completed!";
            logPopup.Duration = 3.0f;
        }


        
        /// <summary>
        /// 현재 세션에 대한 초대코드를 생성합니다.
        /// </summary>
        public void OnCreateInviteCode()
        {
            IsMultiplayEnabled();

            GameObject logPopupInstance = UIManager.InstantiatePopup(UIManager.Setting.logPopupPrefab.Resource, PopupRenderMode.Default, false);
            LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

            logPopup.Log = IsMultiplayEnabled() ?
                           $"Invite code : {MultiplayManager.InviteCode} was generated! \n" +
                           $"Share this code to other player!" :
                           $"Multiplay is disabled. \n" +
                           $"Check multiplay enabled at map setting option";

            logPopup.Duration = IsMultiplayEnabled() ? 8.0f : 4.0f;
        }

        /// <summary>
        /// 플레이어 초대 코드를 생성하기전에 맵 설정에 멀티플레이가 켜져있는지 확인합니다.
        /// </summary>
        private bool IsMultiplayEnabled()
        {
            return DataManager.WorldSettingData.isMultiplayEnabled;
        }



        /// <summary>
        /// 게임 설정 팝업을 인스턴싱 합니다.
        /// </summary>
        public void OnInstantiateOptionPopup()
        {
            UIManager.InstantiatePopup(optionPopupPrefab.Resource, PopupRenderMode.Hide);
            DestroyPopup();
        }



        /// <summary>
        /// 게임 타이틀화면으로 돌아갑니다.
        /// </summary>
        public void OnLoadTitleScene()
        {
            PlayMode currentPlayMode = MultiplayManager.PlayMode;
            if (currentPlayMode == PlayMode.Single || currentPlayMode == PlayMode.Host) { SaveWorld(); }

            DataManager.WorldSettingData = null;

            // 타이틀 씬으로 이동
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.TitleScene;
            string loadingScene = SceneManager.Setting.LoadingScene;
            SceneManager.LoadSceneCoroutine(loadingScene, () => !DataManager.IsSaving, () =>
            {
                ResourceManager.LoadAllResourcesCoroutine(toScene);

                ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                SceneManager.LoadSceneCoroutine(
                    toScene, 
                    () => !ResourceManager.IsLoading && !DataManager.IsSaving, 
                    () => MultiplayManager.Disconnect());
            });
        }



        /// <summary>
        /// 컴퓨터 바탕화면으로 돌아갑니다. (게임을 종료합니다.)
        /// </summary>
        public void OnQuitGame()
        {
            PlayMode currentPlayMode = MultiplayManager.PlayMode;
            if (currentPlayMode == PlayMode.Single || currentPlayMode == PlayMode.Host) { SaveWorld(); }

            MultiplayManager.Disconnect();
            Application.Quit();
        }



        public void DestroyPopup()
        {
            if (isEscaped) { return; }

            isEscaped = true;
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

        private void SaveWorld()
        {
            DataManager.SaveWorldSettingData(
                DataManager.Setting.defaultSaveFolderPath, DataManager.WorldSettingData, () => !DataManager.IsSaving);

            DataManager.SaveWorldGameObjectDatas(
                DataManager.Setting.defaultSaveFolderPath, DataManager.WorldSettingData.worldName, () => !DataManager.IsSaving);
        }
    }
}
