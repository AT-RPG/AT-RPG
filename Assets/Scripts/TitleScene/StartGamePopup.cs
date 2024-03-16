using AT_RPG.Manager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        [SerializeField] private ResourceReference<GameObject> mapSettingPopupPrefab;

        [Tooltip("맵 버튼 프리팹")]
        [SerializeField] private ResourceReference<GameObject> mapButtonPrefab;

        [Tooltip("맵 버튼이 생성될 위치")]
        [SerializeField] private GameObject mapButtonContents;

        [Tooltip("플레이 시 이동할 메인 씬")]
        [SerializeField] private SceneReference mainScene;

        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        // 피킹된 맵
        private MapButton pickedMapButton;

        private void Start()
        {
            StartCoroutine(LoadAllSavedMapDatas());
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
        /// 저장된 모든 월드 정보를 불러옵니다.
        /// </summary>
        private IEnumerator LoadAllSavedMapDatas()
        {
            // 모든 맵 세이브 폴더 가져오기
            string defaultSaveFolderPath = DataManager.Setting.defaultSaveFolderPath;
            string[] filePaths = Directory.GetDirectories(defaultSaveFolderPath);
            List<string> savedMapDataNames = filePaths.Select(path => Path.GetFileName(path)).ToList();

            // 맵 설정 데이터 얻기
            List<MapSettingData> mapSettingDatas = new List<MapSettingData>();
            foreach (var name in savedMapDataNames)
            {
                DataManager.LoadMapSettingDataCoroutine(defaultSaveFolderPath, name, 
                () =>
                {
                    return !DataManager.IsLoading;
                }    
                , 
                mapSettingData =>
                {
                    mapSettingDatas.Add(mapSettingData);
                });
            }

            // 맵 설정 데이터를 모두 불러올 때까지 대기 
            while (mapSettingDatas.Count != savedMapDataNames.Count)
            {
                yield return null;
            }

            // 맵 버튼 생성 및 초기화
            foreach (var mapSettingData in mapSettingDatas)
            {
                GameObject mapButtonInstance
                    = Instantiate(mapButtonPrefab.Resource, mapButtonContents.transform);
                MapButton mapButton = mapButtonInstance.GetComponent<MapButton>();
                mapButton.MapSettingData = mapSettingData;
                mapButton.OnPickAction += OnPickMap;
            }
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

        /// <summary>
        /// 맵을 선택합니다.
        /// </summary>
        public void OnPickMap(GameObject mapButtonInstance)
        {
            // 맵 버튼이 맞는지?
            MapButton mapButton = mapButtonInstance.GetComponent<MapButton>();
            if (!mapButton)
            {
                Debug.LogError($"{mapButtonInstance}는 맵 버튼이 아닙니다.");
                return;
            }

            pickedMapButton = mapButton;
        }

        /// <summary>
        /// 선택한 맵을 플레이합니다. 
        /// </summary>
        public void OnPlayMap()
        {
            if (!pickedMapButton)
            {
                Debug.LogError($"{pickedMapButton}이 아직 선택되지 않았습니다.");
                return;
            }

            InternalOnPlayMap();
        }

        /// <summary>
        /// 맵을 플레이하기 전에 필요한 백앤드 작업을 수행합니다.
        /// </summary>
        private void InternalOnPlayMap()
        {
            string currentScene = SceneManager.CurrentSceneName;
            SerializedGameObjectsList gameObjectDatas = new SerializedGameObjectsList();
            SceneManager.LoadScene(SceneManager.Setting.LoadingScene, () =>
            {
                ResourceManager.LoadAllResourcesCoroutine(mainScene);
                ResourceManager.UnloadAllResourcesCoroutine(currentScene);
                DataManager.LoadAllGameObjectsCoroutine(
                    DataManager.Setting.defaultSaveFolderPath, pickedMapButton.MapName, null,
                    loadedGameObjectDatas => loadedGameObjectDatas.ForEach(loadedGameObjectData => gameObjectDatas.Add(loadedGameObjectData)));
                SceneManager.LoadSceneCoroutine(mainScene,
                () =>
                {
                    return !ResourceManager.IsLoading;
                },
                () =>
                {
                    DataManager.InstantiateGameObjects(gameObjectDatas);
                });
            });
        }

        /// <summary>
        /// 선택된 맵을 삭제합니다.
        /// </summary>
        public void OnDeleteMap()
        {
            if (!pickedMapButton)
            {
                Debug.LogError($"{pickedMapButton}이 아직 선택되지 않았습니다.");
                return;
            }

            string mapSaveDataPath 
                = Path.Combine(DataManager.Setting.defaultSaveFolderPath, pickedMapButton.MapName);
            Directory.Delete(mapSaveDataPath, true);

            Destroy(pickedMapButton.gameObject);
        }
    }
}
