using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections;
using AT_RPG;

using UnityObject = UnityEngine.Object;
using System.Resources;


namespace AT_RPG.Manager
{   
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        // 매니저 기본 설정
        [SerializeField] private ResourceManagerSetting setting;

        // 로드된 씬 리소스 저장 해쉬맵
        private SceneResourceMap resources = new SceneResourceMap();

        // 로드된 에셋 번들
        private AssetBundleMap bundles = new AssetBundleMap();

        // 씬 리소스 로딩중
        private bool isLoading = false;

        // 씬 리소스 언로딩중 
        private bool isUnloading = false;


        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            GameManager.OnBeforeFirstSceneLoadEvent += OnBeforeFirstSceneLoad;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
        }



        /// <summary>
        /// 씬에 모든 리소스를 동기적으로 로드
        /// </summary>
        public void LoadAllAssetsAtScene(string sceneName)
        {
            if (isLoading)
            {
                return;    
            }

            isLoading = true;
            InternalLoadSceneAsset(sceneName);
        }

        /// <summary>
        /// 씬에 모든 리소스를 동기적으로 언로드
        /// </summary>
        public void UnloadAllAssetsAtScene(string sceneName)
        {
            if (isUnloading)
            {
                return;
            }

            isUnloading = true;
            InternalUnloadSceneAsset(sceneName);
        }

        /// <summary>
        /// 씬에 모든 리소스를 비동기적으로 로드
        /// </summary>
        public void LoadAllAssetsAtSceneCor(string sceneName)
        {
            if (!isLoading)
            {
                isLoading = true;
                StartCoroutine(InternalLoadSceneAssetsCor(sceneName));
            }
        }

        /// <summary>
        /// 씬에 모든 리소스를 비동기적으로 언로드
        /// </summary>
        public void UnloadAllAssetsAtSceneCor(string sceneName)
        {
            if (!isUnloading)
            {
                isUnloading = true;
                StartCoroutine(InternalUnloadSceneAssetsCor(sceneName));
            }
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceName">찾을 리소스 이름</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public UnityObject Get(string resourceName, bool isGlobal)
        {
            UnityObject resource;
            if (isGlobal)
            {
                resource = resources[setting.GlobalAssetBundleName][resourceName];
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][resourceName];
            }

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceRefer">찾을 리소스 래퍼런스</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public UnityObject Get(UnityObject resourceRefer, bool isGlobal)
        {
            UnityObject resource;
            if (isGlobal)
            {
                resource = resources[setting.GlobalAssetBundleName][resourceRefer.name];
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][resourceRefer.name];
            }

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceName">찾을 리소스 이름</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public T Get<T>(string resourceName, bool isGlobal) where T : UnityObject
        {
            T resource;
            if (isGlobal)
            {
                resource = resources[setting.GlobalAssetBundleName][resourceName] as T;
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][resourceName] as T;
            }

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceRefer">찾을 리소스 래퍼런스</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public T Get<T>(UnityObject resourceRefer, bool isGlobal)
            where T : UnityObject
        {
            T resource;
            if (isGlobal)
            {
                resource = resources[setting.GlobalAssetBundleName][resourceRefer.name] as T;
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][resourceRefer.name] as T;
            }

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 모든 리소스를 획득
        /// </summary>
        public UnityObject[] GetAll()
        {
            UnityObject[] sceneResources =
                resources[SceneManager.Instance.CurrentSceneName].Values.ToArray();

            return sceneResources;
        }



        private void InternalLoadSceneAsset(string sceneName)
        {
            // 번들 위치
            string assetBundlePath = Path.Combine(setting.AssetBundleSavePath, sceneName);

#if UNITY_EDITOR
            // 에셋 번들이 존재하지 않는 경우 코루틴 종료
            // NOTE : 게임이 완성되면 사실상 각 씬에 대해 에셋 번들이 존재할 수 밖에 없음
            string searchPath = setting.AssetBundleSavePath;
            string searchPattern = sceneName.ToLower();
            string[] files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories);
            if (files.Length <= 0)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                return;
            }
#endif

            // 에셋 번들 로드
            AssetBundle addedAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (addedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                return;
            }

            // 현재 씬에 번들 매핑
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                bundles[sceneName] = new List<AssetBundle>();
            }
            bundles[sceneName].Add(addedAssetBundle);

            // 에셋 번들의 리소스 로드
            Object[] loadedResources = addedAssetBundle.LoadAllAssets<UnityObject>();

            // 현재 씬에 리소스 매핑
            if (!resources.ContainsKey(sceneName) || resources[sceneName] == null)
            {
                resources[sceneName] = new ResourceMap();
            }
            foreach (var resource in loadedResources)
            {
                resources[sceneName][resource.name] = resource;
            }

            isLoading = false;
        }

        private void InternalUnloadSceneAsset(string sceneName)
        {
            // 현재 씬에 로드된 에셋 번들 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                isUnloading = false;
                return;
            }

            // 에셋 번들(리소스) 언로드
            foreach (var bundle in bundles[sceneName])
            {
                if (bundle == null)
                {
                    continue;
                }

                bundle.Unload(true);
            }

            // 언로드된 에셋 번들 매핑 해제
            bundles[sceneName] = null;

            // 언로드된 리소스 매핑 해제
            resources[sceneName] = null;

            isUnloading = false;
        }

        private IEnumerator InternalLoadSceneAssetsCor(string sceneName)
        {
            // 번들 위치
            string assetBundlePath = Path.Combine(setting.AssetBundleSavePath, sceneName);

#if UNITY_EDITOR
            // 에셋 번들이 존재하지 않는 경우 코루틴 종료
            // NOTE : 게임이 완성되면 사실상 각 씬에 대해 에셋 번들이 존재할 수 밖에 없음
            string searchPath = setting.AssetBundleSavePath;
            string searchPattern = sceneName.ToLower();
            string[] files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories);
            if (files.Length <= 0)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                yield break;
            }
#endif 
            // 에셋 번들 로드
            AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            yield return new WaitUntil(() => assetBundleRequest.isDone);

            
            // 현재 씬에 에셋 번들을 매핑
            AssetBundle addedAssetBundle = assetBundleRequest.assetBundle;
            if (addedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                yield break;
            }

            // 현재 씬에 번들 매핑
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                bundles[sceneName] = new List<AssetBundle>();
            }
            bundles[sceneName].Add(addedAssetBundle);

            // 에셋 번들의 리소스 로드
            AssetBundleRequest assetRequest = addedAssetBundle.LoadAllAssetsAsync<UnityObject>();
            yield return new WaitUntil(() => assetRequest.isDone);

            // 현재 씬에 리소스 매핑
            if (!resources.ContainsKey(sceneName) || resources[sceneName] == null)
            {
                resources[sceneName] = new ResourceMap();
            }
            foreach (var resource in assetRequest.allAssets)
            {
                resources[sceneName][resource.name] = resource;
            }

            // 페이크 로딩
            float fakeLoadingElapsedTime = 0f;
            float fakeLoadingDuration = setting.FakeLoadingDuration;
            while (fakeLoadingElapsedTime <= fakeLoadingDuration)
            {
                fakeLoadingElapsedTime += Time.deltaTime;
                yield return null;
            }

            isLoading = false;
        }

        private IEnumerator InternalUnloadSceneAssetsCor(string sceneName)
        {
            // 현재 씬에 로드된 에셋 번들 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                isUnloading = false;
                yield break;
            }

            // 에셋 번들(리소스) 언로드
            foreach (var bundle in bundles[sceneName])
            {
                if (bundle == null)
                {
                    continue;
                }

                AsyncOperation unloadOperation = bundle.UnloadAsync(true);
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }

            // 언로드된 에셋 번들 매핑 해제
            bundles[sceneName] = null;

            // 언로드된 리소스 매핑 해제
            resources[sceneName] = null;

            isUnloading = false;

            yield break;
        }

        private void OnBeforeFirstSceneLoad()
        {
            LoadAllAssetsAtScene(setting.GlobalAssetBundleName);
            LoadAllAssetsAtScene(SceneManager.Instance.CurrentSceneName);
        }
    }

    public partial class ResourceManager
    {
        // 리소스 로딩중
        public bool IsLoading => isLoading;

        // 리소스 언로딩중
        public bool IsUnloading => isUnloading;

        // 매니저 기본 설정
        public ResourceManagerSetting Setting => setting;
    }
}