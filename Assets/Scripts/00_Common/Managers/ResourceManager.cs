using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityObject = UnityEngine.Object;


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
            if (isLoading)
            {
                return;
            }

            isLoading = true;
            StartCoroutine(InternalLoadSceneAssetsCor(sceneName));
        }

        /// <summary>
        /// 씬에 모든 리소스를 비동기적으로 언로드
        /// </summary>
        public void UnloadAllAssetsAtSceneCor(string sceneName)
        {
            if (isUnloading)
            {
                return;
            }

            isUnloading = true;
            StartCoroutine(InternalUnloadSceneAssetsCor(sceneName));
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
                resource = resources[setting.GlobalAssetBundleName][typeof(T).Name][resourceName] as T;
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][typeof(T).Name][resourceName] as T;
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
                resource = resources[setting.GlobalAssetBundleName][typeof(T).Name][resourceRefer.name] as T;
            }
            else
            {
                resource = resources[SceneManager.Instance.CurrentSceneName][typeof(T).Name][resourceRefer.name] as T;
            }

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 모든 리소스를 획득
        /// </summary>
        public UnityObject[] GetAll()
        {
            List<UnityObject> sceneResources = new List<UnityObject>();

            var sceneResourceMap = resources[SceneManager.Instance.CurrentSceneName].Values;
            foreach (var resourceType in sceneResourceMap)
            {
                foreach (var resource in resourceType)
                {
                    sceneResources.Add(resource.Value);
                }
            }

            return sceneResources.ToArray();
        }

        /// <summary>
        /// 씬의 리소스 동기 로딩
        /// </summary>
        private void InternalLoadSceneAsset(string sceneName)
        {
            // 번들 위치
            string assetBundlePath = Path.Combine(setting.AssetBundleSavePath, sceneName);

//#if UNITY_EDITOR
//            // 에셋 번들이 존재하지 않는 경우 코루틴 종료
//            // NOTE : 게임이 완성되면 사실상 각 씬에 대해 에셋 번들이 존재할 수 밖에 없음
//            string searchPath = setting.AssetBundleSavePath;
//            string searchPattern = sceneName.ToLower();
//            string[] files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories);
//            if (files.Length <= 0)
//            {
//                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
//                isLoading = false;
//                return;
//            }
//#endif
            // 에셋 번들 로드
            AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (loadedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                return;
            }

            // 현재 씬에 에셋 번들 매핑
            MapAssetBundleAtScene(sceneName, loadedAssetBundle);

            // 에셋 번들의 리소스 로드
            Object[] loadedResources = loadedAssetBundle.LoadAllAssets<UnityObject>();

            // 현재 씬에 리소스 매핑
            MapResourcesAtScene(sceneName, loadedResources);

            isLoading = false;
        }

        /// <summary>
        /// 씬의 리소스 동기 언로딩
        /// </summary>
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

        /// <summary>
        /// 씬의 리소스 비동기 로딩
        /// </summary>
        private IEnumerator InternalLoadSceneAssetsCor(string sceneName)
        {
            // 번들 위치
            string assetBundlePath = Path.Combine(setting.AssetBundleSavePath, sceneName);

//#if UNITY_EDITOR
//            // 에셋 번들이 존재하지 않는 경우 코루틴 종료
//            // NOTE : 게임이 완성되면 사실상 각 씬에 대해 에셋 번들이 존재할 수 밖에 없음
//            string searchPath = setting.AssetBundleSavePath;
//            string searchPattern = sceneName.ToLower();
//            string[] files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories);
//            if (files.Length <= 0)
//            {
//                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
//                isLoading = false;
//                yield break;
//            }
//#endif 
            // 에셋 번들 로드
            AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            yield return new WaitUntil(() => assetBundleRequest.isDone);

            
            // 현재 씬에 에셋 번들을 매핑
            AssetBundle loadedAssetBundle = assetBundleRequest.assetBundle;
            if (loadedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                yield break;
            }

            // 현재 씬에 에셋 번들 매핑
            MapAssetBundleAtScene(sceneName, loadedAssetBundle);

            // 에셋 번들의 리소스 로드
            AssetBundleRequest assetRequest = loadedAssetBundle.LoadAllAssetsAsync<UnityObject>();
            yield return new WaitUntil(() => assetRequest.isDone);

            // 현재 씬에 리소스 매핑
            MapResourcesAtScene(sceneName, assetRequest.allAssets);

            // 페이크 로딩
            yield return LoadFakeDuration();

            isLoading = false;
        }

        /// <summary>
        /// 씬에 에셋 번들을 매핑
        /// </summary>
        private void MapAssetBundleAtScene(string sceneName, AssetBundle loadedAssetBundle)
        {
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                bundles[sceneName] = new List<AssetBundle>();
            }
            bundles[sceneName].Add(loadedAssetBundle);
        }

        /// <summary>
        /// 씬에 에셋 번들에서 나온 리소스들을 매핑
        /// </summary>
        private void MapResourcesAtScene(string sceneName, UnityObject[] loadedResources)
        {
            if (!resources.ContainsKey(sceneName) || resources[sceneName] == null)
            {
                resources[sceneName] = new ResourceMap();
            }
            foreach (var resource in loadedResources)
            {
                string resourceTypeName = resource.GetType().Name;
                if (!resources[sceneName].ContainsKey(resourceTypeName))
                {
                    resources[sceneName][resourceTypeName] = new Dictionary<string, UnityObject>();
                }

                resources[sceneName][resourceTypeName][resource.name] = resource;
            }
        }

        /// <summary>
        /// 페이크 로딩
        /// </summary>
        private IEnumerator LoadFakeDuration()
        {
            float fakeLoadingElapsedTime = 0f;
            float fakeLoadingDuration = setting.FakeLoadingDuration;
            while (fakeLoadingElapsedTime <= fakeLoadingDuration)
            {
                fakeLoadingElapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 씬의 리소스 비동기 언로딩
        /// </summary>
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

        /// <summary>
        /// 게임시작 시 첫 씬의 Awake()전에 호출
        /// </summary>
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