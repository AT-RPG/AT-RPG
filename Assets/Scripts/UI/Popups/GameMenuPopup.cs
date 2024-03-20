using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 인게임 플레이중에 ESC를 통해서 인스턴싱되는 게임 메뉴 팝업 클래스
    /// </summary>
    public class GameMenuPopup : Popup, IPopupDestroy
    {
        [Tooltip("게임 설정 팝업 프리팹")]
        [SerializeField] private ResourceReference<GameObject>  optionPopupPrefab;

        [Tooltip("타이틀 화면 씬")]
        [SerializeField] private SceneReference                 titleScene;

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
            if (SceneManager.CurrentSceneName != SceneManager.Setting.MainScene) 
            { 
                saveButtonInstance.SetActive(false);
                inviteButtonInstance.SetActive(false);
            }
            if (SceneManager.CurrentSceneName == SceneManager.Setting.TitleScene) { titleButtonInstance.SetActive(false); }
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
                isEscaped = true;
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
            DataManager.SaveMapSettingDataCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData, () => !DataManager.IsSaving);

            DataManager.SaveAllGameObjectsCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName, () => !DataManager.IsSaving);
        }


        
        /// <summary>
        /// 현재 세션에 대한 초대코드를 생성합니다.
        /// </summary>
        public void OnCreateInviteCode()
        {
            GameObject logPopupInstance = UIManager.InstantiatePopup(UIManager.Setting.logPopupPrefab.Resource, PopupRenderMode.Default, false);
            LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

            logPopup.Log = $"Invite code : {MultiplayManager.CreateInviteCode()} was generated! \n" +
                           $"Share this code to other player!";

            logPopup.Duration = 8.0f;
        }



        /// <summary>
        /// 게임 설정 팝업을 인스턴싱 합니다.
        /// </summary>
        public void OnInstantiateOptionPopup()
        {
            
        }



        /// <summary>
        /// 게임 타이틀화면으로 돌아갑니다.
        /// </summary>
        public void OnLoadTitleScene()
        {
            DataManager.SaveMapSettingDataCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData, () => !DataManager.IsSaving);

            DataManager.SaveAllGameObjectsCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName, () => !DataManager.IsSaving);

            // 타이틀 씬으로 이동
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.TitleScene;
            string loadingScene = SceneManager.Setting.LoadingScene;
            SceneManager.LoadSceneCoroutine(loadingScene, () => !DataManager.IsSaving, () =>
            {
                ResourceManager.LoadAllResourcesCoroutine(toScene);

                ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading && !DataManager.IsSaving);
            });
        }



        /// <summary>
        /// 컴퓨터 바탕화면으로 돌아갑니다. (게임을 종료합니다.)
        /// </summary>
        public void OnQuitGame()
        {
            Application.Quit();
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
