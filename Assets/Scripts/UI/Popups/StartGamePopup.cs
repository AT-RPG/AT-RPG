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
    public class StartGamePopup : Popup, IPopupDestroy
    {
        [Tooltip("맵 버튼이 생성될 위치")]
        [SerializeField] private GameObject mapButtonContents;

        [Header("하위 팝업")]
        [Tooltip("맵 설정 팝업 프리팹")]
        [SerializeField] private ResourceReference<GameObject> mapSettingPopupPrefab;

        [Tooltip("맵 버튼 프리팹")]
        [SerializeField] private ResourceReference<GameObject> mapButtonPrefab;

        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        // 피킹된 맵
        private MapButton currPickedMapButton;



        private void Start()
        {
            AnimateStartSequence();
            StartCoroutine(LoadAllSavedMapDatas());
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

        private void OnPickMap(GameObject mapButtonInstance)
        {
            // 맵 버튼이 맞는지?
            MapButton mapButton = mapButtonInstance.GetComponent<MapButton>();
            if (!mapButton)
            {
                Debug.LogError($"{mapButtonInstance}는 맵 버튼이 아닙니다.");
                return;
            }

            currPickedMapButton = mapButton;
        }



        /// <summary>
        /// 맵 설정 팝업을 생성합니다.
        /// </summary>
        public void OnInstantiateMapSettingPopup()
        {
            UIManager.InstantiatePopup(mapSettingPopupPrefab.Resource, PopupRenderMode.Hide);
        }



        /// <summary>
        /// 선택한 맵을 플레이합니다. 
        /// </summary>
        public void OnPlayMap()
        {
            if (!currPickedMapButton)
            {
                Debug.LogError($"{currPickedMapButton}이 아직 선택되지 않았습니다.");
                return;
            }

            InternalOnPlayMap();
        }

        /// <summary>
        /// 맵을 플레이하기 전에 필요한 백앤드 작업을 수행합니다.
        /// TODO : 리소스 로딩 바로 직후 세이브 파일 로딩하면 에러가 생김...
        /// </summary>
        private void InternalOnPlayMap()
        {
            SerializedGameObjectsList gameObjectDatas = new SerializedGameObjectsList();

            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.MainScene;
            SceneManager.LoadScene(SceneManager.Setting.LoadingScene, () =>
            {
                // 리소스 로딩/언로딩 + 세이브 파일 로딩
                ResourceManager.LoadAllResourcesCoroutine(toScene);
                ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                // 로딩이 끝나면 씬을 변경합니다.
                SceneManager.LoadSceneCoroutine(toScene,
                () =>
                {
                    return !ResourceManager.IsLoading && !DataManager.IsLoading;
                },
                () =>
                {
                    DataManager.LoadMapSettingDataCoroutine(
                        DataManager.Setting.defaultSaveFolderPath, currPickedMapButton.MapName,
                        () => !DataManager.IsLoading && !ResourceManager.IsLoading,
                        loadedMapSettingData => DataManager.MapSettingData = loadedMapSettingData);

                    DataManager.LoadAllGameObjectsCoroutine(
                        DataManager.Setting.defaultSaveFolderPath, currPickedMapButton.MapName,
                        () => !DataManager.IsLoading && !ResourceManager.IsLoading,
                        loadedGameObjectDatas => DataManager.InstantiateGameObjects(loadedGameObjectDatas));
                });
            });
        }


        /// <summary>
        /// 선택된 맵을 삭제합니다.
        /// </summary>
        public void OnDeleteMap()
        {
            if (!currPickedMapButton)
            {
                Debug.LogError($"{currPickedMapButton}이 아직 선택되지 않았습니다.");
                return;
            }

            string mapSaveDataPath 
                = Path.Combine(DataManager.Setting.defaultSaveFolderPath, currPickedMapButton.MapName);
            Directory.Delete(mapSaveDataPath, true);

            Destroy(currPickedMapButton.gameObject);
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
