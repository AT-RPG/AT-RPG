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
        [Tooltip("월드 버튼이 생성될 위치")]
        [SerializeField] private GameObject worldButtonContents;

        [Header("하위 팝업")]
        [Tooltip("월드 설정 팝업 프리팹")]
        [SerializeField] private ResourceReference<GameObject> worldSettingPopupPrefab;

        [Tooltip("월드 버튼 프리팹")]
        [SerializeField] private ResourceReference<GameObject> worldButtonPrefab;

        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        [Header("팝업 버튼")]
        [SerializeField] private GameObject playWorldButtonInstance;
        [SerializeField] private GameObject deleteWorldButtonInstance;

        // 피킹된 맵
        private MapButton currPickedWorldButton;


        private void Start()
        {
            AnimateStartSequence();
            StartCoroutine(LoadAllSavedMapDatas());

            playWorldButtonInstance.SetActive(false);
            deleteWorldButtonInstance.SetActive(false);
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
                    = Instantiate(worldButtonPrefab.Resource, worldButtonContents.transform);
                MapButton mapButton = mapButtonInstance.GetComponent<MapButton>();
                mapButton.MapSettingData = mapSettingData;
                mapButton.OnPickAction += OnPickWorld;
            }
        }


        /// <summary>
        /// 버튼의 동작 대상에 대한 포커스를 선택한 월드에 둡니다.
        /// </summary>
        private void OnPickWorld(GameObject mapButtonInstance)
        {
            // 맵 버튼이 맞는지?
            MapButton mapButton = mapButtonInstance.GetComponent<MapButton>();
            if (!mapButton)
            {
                Debug.LogError($"{mapButtonInstance}는 맵 버튼이 아닙니다.");
                return;
            }

            currPickedWorldButton = mapButton;
            playWorldButtonInstance.SetActive(true);
            deleteWorldButtonInstance.SetActive(true);
        }


        /// <summary>
        /// 월드를 설정 팝업을 생성합니다.
        /// </summary>
        public void OnInstantiateMapSettingPopup()
        {
            UIManager.InstantiatePopup(worldSettingPopupPrefab.Resource, PopupRenderMode.Hide);
        }


        /// <summary>
        /// 선택한 월드를 플레이합니다. 
        /// </summary>
        public void OnPlayWorld()
        {
            if (!currPickedWorldButton)
            {
                Debug.LogError($"맵이 아직 선택되지 않았습니다.");
                return;
            }

            InternalOnPlayWorld();
        }


        /// <summary>
        /// 월드를 플레이하기 전에 필요한 백앤드 작업을 수행합니다.
        /// </summary>
        /// TODO : 리펙토링
        private void InternalOnPlayWorld()
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
                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading && !DataManager.IsLoading, () =>
                {
                    // 로드한 맵 설정에서 멀티플레이가 활성화 되어있다면 세션을 만듭니다.
                    DataManager.LoadMapSettingDataCoroutine(
                        DataManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName,
                        () => !DataManager.IsLoading && !ResourceManager.IsLoading,
                        loadedMapSettingData => DataManager.WorldSettingData = loadedMapSettingData);

                    // 세이브 파일에 저장된 게임 오브젝트들을 불러와서 인스턴싱합니다.
                    // 그 후 호스트를 만들건지 정합니다.
                    DataManager.LoadAllGameObjectsCoroutine(
                        DataManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName,
                        () => !DataManager.IsLoading && !ResourceManager.IsLoading,
                        loadedGameObjectDatas => 
                        {
                            DataManager.InstantiateGameObjects(loadedGameObjectDatas);
                            if (DataManager.WorldSettingData.isMultiplayEnabled) { MultiplayManager.ConnectToCloud(); }
                        });
                });
            });
        }


        /// <summary>
        /// 선택된 월드를 삭제합니다.
        /// </summary>
        public void OnDeleteWorld()
        {
            if (!currPickedWorldButton)
            {
                Debug.LogError($"{currPickedWorldButton}이 아직 선택되지 않았습니다.");
                return;
            }

            string mapSaveDataPath = Path.Combine(DataManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName);
            Directory.Delete(mapSaveDataPath, true);

            Destroy(currPickedWorldButton.gameObject);
            playWorldButtonInstance.SetActive(false);
            deleteWorldButtonInstance.SetActive(false);
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


        public void DestroyPopup()
        {
            AnimateEscapeSequence();
        }
    }
}
