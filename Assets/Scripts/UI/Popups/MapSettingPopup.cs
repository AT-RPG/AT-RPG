using AT_RPG.Manager;
using System;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                 <br/>
    /// + 맵 설정 팝업에서 사용되는 클래스       <br/>
    /// </summary>
    public class MapSettingPopup : Popup, IPopupDestroy
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
        /// 시작 애니메이션을 실행합니다.
        /// </summary>
        private void AnimateStartSequence()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
            blurAnimation.StartFade();
        }



        /// <summary>
        /// 현재 팝업을 닫고, 맵 설정에 따라 맵 데이터를 생성합니다.
        /// </summary>
        public void OnCreateMap()
        {
            if (!IsMapSettingEnable())
            {
                return;
            }

            // 맵 설정 데이터 초기화
            MapSettingData mapSettingData = new MapSettingData();
            mapSettingData.mapName = mapName.text;
            mapSettingData.lastModifiedTime = DateTime.Now.ToString();

            // 맵 설정 데이터 저장
            // 저장이 완료되면 맵 설정 팝업창을 닫고, 맵 선택 팝업창을 인스턴싱
            DataManager.SaveMapSettingDataCoroutine(DataManager.Setting.defaultSaveFolderPath, mapSettingData, null, () =>
            {
                popupCanvas?.RemoveAllPopupInstance();
                UIManager.InstantiatePopup(startGamePopupPrefab.Resource, PopupRenderMode.Hide);
            });
        }

        /// <summary>
        /// 맵 설정에 잘못된 부분이 없는가?
        /// </summary>
        /// <returns></returns>
        private bool IsMapSettingEnable()
        {
            // 맵 이름 비어있음?
            if (mapName.text.Length <= 0)
            {
                GameObject logPopupInstance 
                    = UIManager.InstantiatePopup(UIManager.Setting.gameMenuPopupPrefab.Resource, PopupRenderMode.Default);
                LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

                logPopup.Log = "Can't be map name empty!";

                return false;
            }

            // 이미 세이브 파일에 동일한 맵 이름이 있음?
            if (DataManager.IsSaveDataDirectroyExist(DataManager.Setting.defaultSaveFolderPath, mapName.text))
            {
                GameObject logPopupInstance 
                    = UIManager.InstantiatePopup(UIManager.Setting.gameMenuPopupPrefab.Resource, PopupRenderMode.Default);
                LogPopup logPopup = logPopupInstance.GetComponent<LogPopup>();

                logPopup.Log = $"{mapName.text} already exist!";

                return false;
            }

            return true;
        }



        /// <summary>
        /// 현재 팝업을 닫고, 맵 선택 팝업을 인스턴싱합니다.       
        /// </summary>
        public void OnInstantiateStartGamePopup()
        {
            popupCanvas?.RemovePopupInstance(new InputValue());
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