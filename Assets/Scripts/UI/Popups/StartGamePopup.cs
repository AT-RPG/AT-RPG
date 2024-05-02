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
        [SerializeField] private AssetReferenceResource<GameObject> worldSettingPopupPrefab;

        [Tooltip("월드 버튼 프리팹")]
        [SerializeField] private AssetReferenceResource<GameObject> worldButtonPrefab;

        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        [Header("팝업 버튼")]
        [SerializeField] private GameObject playWorldButtonInstance;
        [SerializeField] private GameObject deleteWorldButtonInstance;

        // 피킹된 맵
        private WorldButton currPickedWorldButton;


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
            string defaultSaveFolderPath = SaveLoadManager.Setting.defaultSaveFolderPath;
            string[] filePaths = Directory.GetDirectories(defaultSaveFolderPath);
            List<string> savedMapDataNames = filePaths.Select(path => Path.GetFileName(path)).ToList();

            // 맵 설정 데이터 얻기
            List<WorldSettingData> worldSettingDatas = new List<WorldSettingData>();
            foreach (var name in savedMapDataNames)
            {
                SaveLoadManager.LoadWorldSettingDataCoroutine(defaultSaveFolderPath, name, () => !SaveLoadManager.IsLoading, worldSettingData => worldSettingDatas.Add(worldSettingData));
            }

            // 맵 설정 데이터를 모두 불러올 때까지 대기 
            while (worldSettingDatas.Count != savedMapDataNames.Count)
            {
                yield return null;
            }

            // 맵 버튼 생성 및 초기화
            foreach (var worldSettingData in worldSettingDatas)
            {
                GameObject worldButtonInstance = Instantiate(worldButtonPrefab.Resource, worldButtonContents.transform);
                WorldButton worldButton = worldButtonInstance.GetComponent<WorldButton>();
                worldButton.WorldSettingData = worldSettingData;
                worldButton.OnPickAction += OnPickWorld;
            }
        }

        /// <summary>
        /// 버튼의 동작 대상에 대한 포커스를 선택한 월드에 둡니다.
        /// </summary>
        private void OnPickWorld(GameObject worldButtonInstance)
        {
            // 맵 버튼이 맞는지?
            WorldButton worldButton = worldButtonInstance.GetComponent<WorldButton>();
            if (!worldButton)
            {
                Debug.LogError($"{worldButtonInstance}는 맵 버튼이 아닙니다.");
                return;
            }

            currPickedWorldButton = worldButton;
            playWorldButtonInstance.SetActive(true);
            deleteWorldButtonInstance.SetActive(true);
        }

        /// <summary>
        /// 월드를 설정 팝업을 생성합니다.
        /// </summary>
        public void OnInstantiateWorldSettingPopup()
        {
            UIManager.InstantiatePopup(worldSettingPopupPrefab.Resource, PopupRenderMode.Default);
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
        private void InternalOnPlayWorld()
        {
            SceneManager.LoadSceneCoroutine(SceneManager.Setting.LoadingScene, null, () =>
            {
                // 리소스 로딩
                foreach (var label in SceneManager.Setting.MainSceneAddressableLabelMap)
                {
                    ResourceManager.LoadAssetsAsync(label.labelString, null, true);
                }

                // 리소스 언로딩
                foreach (var label in SceneManager.Setting.TitleSceneAddressableLabelMap)
                {
                    ResourceManager.UnloadAssetsAsync(label.labelString);
                }

                // 로딩 순서 
                // 1. 맵 설정 불러오기
                // 2. 호스트 모드 (활성화가 되어있는 경우)
                // 3. 맵 오브젝트 불러오기
                // 4. 씬 이동
                InternalLoadWorldSetting();
            });
        }

        /// <summary>
        /// 로드한 맵 설정에서 멀티플레이가 활성화 되어있다면 세션을 만듭니다.
        /// </summary>
        private void InternalLoadWorldSetting()
        {
            SaveLoadManager.LoadWorldSettingDataCoroutine(
                SaveLoadManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName,
                () => !SaveLoadManager.IsLoading && !ResourceManager.IsLoading,
                loadedWorldSettingData =>
                {
                    SaveLoadManager.WorldSettingData = loadedWorldSettingData;
                    if (loadedWorldSettingData.isMultiplayEnabled)
                    {
                        InternalConnectMultiplay();
                    }
                    else
                    {
                        InternalLoadWorldGameObject();
                    }
                });
        }

        /// <summary>
        /// 멀티플레이가 활성화된 경우, Host모드로 연결합니다.
        /// </summary>
        private void InternalConnectMultiplay()
        {
            MultiplayManager.Connect(() => !ResourceManager.IsLoading && !SaveLoadManager.IsLoading, InternalLoadWorldGameObject);
        }

        /// <summary>
        /// 세이브 파일에 저장된 게임 오브젝트들을 불러와서 인스턴싱합니다.
        /// </summary>
        private void InternalLoadWorldGameObject()
        {
            SaveLoadManager.LoadGameObjectDatasCoroutine(
                SaveLoadManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName,
                () => !SaveLoadManager.IsLoading && !ResourceManager.IsLoading,
                loadedGameObjectDatas => 
                {
                    SaveLoadManager.InstantiateGameObjectFromData(loadedGameObjectDatas);
                    SceneManager.LoadSceneCoroutine(SceneManager.Setting.MainScene, () => !ResourceManager.IsLoading && !SaveLoadManager.IsLoading && !MultiplayManager.IsConnecting);
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

            string mapSaveDataPath = Path.Combine(SaveLoadManager.Setting.defaultSaveFolderPath, currPickedWorldButton.MapName);
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
