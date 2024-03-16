using AT_RPG.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 인게임 플레이중에 ESC를 통해서 인스턴싱되는 게임 메뉴 팝업 클래스
    /// </summary>
    public class GameMenuPopup : Popup
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
        [SerializeField] private GameObject                     optionButtonInstance;
        [SerializeField] private GameObject                     titleButtonInstance;
        [SerializeField] private GameObject                     quitButtonInstance;



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
        /// 팝업 종료를 팝업 UI캔버스에 요청합니다.
        /// </summary>
        public override void InvokeDestroy()
        {
            base.InvokeDestroy();

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



        /// <summary>
        /// 게임 메뉴 팝업을 종료 합니다.
        /// </summary>
        public void OnContinueButtonClick()
        {
            
        }



        /// <summary>
        /// 현재 게임의 맵 설정과 저장 대상인 게임 오브젝트를 저장합니다.
        /// </summary>
        public void OnSaveButtonClick()
        {
            DataManager.LoadMapSettingDataCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName);

            DataManager.LoadAllGameObjectsCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName);
        }



        /// <summary>
        /// 게임 설정 팝업을 인스턴싱 합니다.
        /// </summary>
        public void OnOptionButtonClick()
        {
            
        }



        /// <summary>
        /// 게임 타이틀화면으로 돌아갑니다.
        /// </summary>
        public void OnTitleButtonClick()
        {
            // 씬 이동 전에 현재 월드 세이브
            DataManager.LoadMapSettingDataCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName);

            DataManager.LoadAllGameObjectsCoroutine(
                DataManager.Setting.defaultSaveFolderPath, DataManager.MapSettingData.mapName);


            // 타이틀 씬으로 이동
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.TitleScene;
            string loadingScene = SceneManager.Setting.LoadingScene;
            SceneManager.LoadScene(loadingScene, () =>
            {
                ResourceManager.LoadAllResourcesCoroutine(toScene);

                ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
            });
        }



        /// <summary>
        /// 컴퓨터 바탕화면으로 돌아갑니다. (게임을 종료합니다.)
        /// </summary>
        public void OnQuitButtonClick()
        {
            Application.Quit();
        }
    }
}
