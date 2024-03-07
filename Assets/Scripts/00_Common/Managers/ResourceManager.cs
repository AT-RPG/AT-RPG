using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityObject = UnityEngine.Object;


namespace AT_RPG.Manager
{
    /// <summary>
    /// 설명 : 에셋 번들에 등록된 리소스들을 씬의 이름을 통해서 메모리에 로드하는 클래스 <br/> <br/>
    /// 
    /// 주의 사항 : <br/>
    /// 1. 에셋 번들의 라벨이 씬 이름과 동일해야함 <br/>
    /// </summary>
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        // 매니저 기본 설정
        [SerializeField] private ResourceManagerSetting setting;

        // 씬 리소스 해쉬맵
        private SceneResourceMap resources = new SceneResourceMap();

        // 씬 에셋 번들 해쉬맵
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
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceName">찾을 리소스 이름</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public T Get<T>(string resourceName) where T : UnityObject
        {
            T resource = null;
            string typeName = typeof(T).Name;
            string sceneName = SceneManager.Instance.CurrentSceneName;
            string globalBundleName = setting.GlobalAssetBundleName;


            // 현재 씬에 리소스가 등록되었는지?
            if (resources.ContainsKey(sceneName) &&
                resources[sceneName].ContainsKey(typeName) &&
                resources[sceneName][typeName].ContainsKey(resourceName))
            {
                // 타입 형변환이 가능한지?
                resource = resources[sceneName][typeName][resourceName] as T;
                if (resource)
                {
                    return resource;
                }
            }

            // 글로벌 리소스에 등록되었는지?
            if (resources.ContainsKey(globalBundleName) &&
                resources[globalBundleName].ContainsKey(typeName) &&
                resources[globalBundleName][typeName].ContainsKey(resourceName))
            {
                // 타입 형변환이 가능한지?
                resource = resources[globalBundleName][typeName][resourceName] as T;
                if (resource)
                {
                    return resource;
                }
            }

            Debug.LogError($"{resourceName}를 {typeName}로 형변환 할 수 없거나" +
                           $", 해당 씬에 리소스가 등록되지 않았습니다. {resourceName} = null을 반환합니다.");
            
            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득
        /// </summary>
        /// <param name="resourceRefer">찾을 리소스 래퍼런스</param>
        /// <param name="isGlobal">리소스가 글로벌 번들인지</param>
        public T Get<T>(UnityObject resourceRefer)
            where T : UnityObject
        {
            T resource = null;
            string typeName = typeof(T).Name;
            string sceneName = SceneManager.Instance.CurrentSceneName;
            string globalBundleName = setting.GlobalAssetBundleName;


            // 현재 씬에 리소스가 등록되었는지?
            if (resources.ContainsKey(sceneName) &&
                resources[sceneName].ContainsKey(typeName) &&
                resources[sceneName][typeName].ContainsKey(resourceRefer.name))
            {
                // 타입 형변환이 가능한지?
                resource = resources[sceneName][typeName][resourceRefer.name] as T;
                if (resource)
                {
                    return resource;
                }
            }

            // 글로벌 리소스에 등록되었는지?
            if (resources.ContainsKey(globalBundleName) &&
                resources[globalBundleName].ContainsKey(typeName) &&
                resources[globalBundleName][typeName].ContainsKey(resourceRefer.name))
            {
                // 타입 형변환이 가능한지?
                resource = resources[globalBundleName][typeName][resourceRefer.name] as T;
                if (resource)
                {
                    return resource;
                }
            }

            Debug.LogError($"{resourceRefer.name}를 {typeName}로 형변환 할 수 없거나" +
                           $", 해당 씬에 리소스가 등록되지 않았습니다. {resourceRefer.name} = null을 반환합니다.");

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
        /// 씬의 리소스 비동기 로딩
        /// </summary>
        private IEnumerator InternalLoadSceneAssetsCor(string sceneName)
        {
            // 에셋 번들 로드
            string assetBundlePath = Path.Combine(setting.AssetBundlesSavePath, sceneName);
            AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            yield return new WaitUntil(() => assetBundleRequest.isDone);

            // 로드한 에셋 번들 유효한가?
            AssetBundle loadedAssetBundle = assetBundleRequest.assetBundle;
            if (loadedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                yield break;
            }

            // 에셋 번들의 리소스 로드
            AssetBundleRequest assetRequest = loadedAssetBundle.LoadAllAssetsAsync<UnityObject>();
            yield return new WaitUntil(() => assetRequest.isDone);

            // 현재 씬에 리소스, 에셋 번들을 매핑
            MapAssetBundleAtScene(sceneName, loadedAssetBundle);
            MapResourcesAtScene(sceneName, assetRequest.allAssets);

            // 페이크 로딩
            yield return LoadFakeDuration();

            isLoading = false;
        }

        /// <summary>
        /// 씬의 리소스 동기 로딩
        /// </summary>
        private void InternalLoadSceneAsset(string sceneName)
        {
            // 에셋 번들 로드
            string assetBundlePath = Path.Combine(setting.AssetBundlesSavePath, sceneName);
            AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (loadedAssetBundle == null)
            {
                Debug.LogWarning($"씬에 사용되는 에셋 번들이 없습니다. 필요 에셋 번들 : {sceneName}");
                isLoading = false;
                return;
            }

            // 에셋 번들의 리소스 로드
            Object[] loadedResources = loadedAssetBundle.LoadAllAssets<UnityObject>();

            // 현재 씬에 리소스, 에셋 번들을 매핑
            MapAssetBundleAtScene(sceneName, loadedAssetBundle);
            MapResourcesAtScene(sceneName, loadedResources);

            isLoading = false;
        }

        /// <summary>
        /// 씬의 리소스 비동기 언로딩
        /// </summary>
        private IEnumerator InternalUnloadSceneAssetsCor(string sceneName)
        {
            // 현재 씬에 에셋 번들이 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                isUnloading = false;
                yield break;
            }

            // 에셋 번들 + 에셋 번들의 리소스 언로드
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

            // 매핑 해제
            bundles[sceneName] = null;
            resources[sceneName] = null;

            isUnloading = false;
        }

        /// <summary>
        /// 씬의 리소스 동기 언로딩
        /// </summary>
        private void InternalUnloadSceneAsset(string sceneName)
        {
            // 현재 씬에 매핑된 에셋 번들이 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                isUnloading = false;
                return;
            }

            // 에셋 번들 + 에셋 번들의 리소스 언로드
            foreach (var bundle in bundles[sceneName])
            {
                if (bundle == null)
                {
                    continue;
                }

                bundle.Unload(true);
            }

            // 매핑 해제
            bundles[sceneName] = null;
            resources[sceneName] = null;

            isUnloading = false;
        }

        /// <summary>
        /// 씬에 에셋 번들을 매핑
        /// </summary>
        private void MapAssetBundleAtScene(string sceneName, AssetBundle loadedAssetBundle)
        {
            // 씬에 에셋 번들 매핑이 처음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
                bundles[sceneName] = new List<AssetBundle>();
            }

            // 로드한 에셋 번들 씬에 매핑
            bundles[sceneName].Add(loadedAssetBundle);
        }

        /// <summary>
        /// 씬에 에셋 번들에서 나온 리소스들을 매핑
        /// </summary>
        private void MapResourcesAtScene(string sceneName, UnityObject[] loadedResources)
        {
            // 씬에 리소스 매핑이 처음?
            if (!resources.ContainsKey(sceneName) || resources[sceneName] == null)
            {
                resources[sceneName] = new ResourceMap();
            }

            // 로드한 리소스들 씬에 전부 매핑
            foreach (var resource in loadedResources)
            {
                // 이 리소스의 자료형 매핑이 처음?
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
        /// 게임시작 시, 글로벌 에셋 번들 + 첫 씬의 에셋 번들 로드
        /// </summary>
        private void OnBeforeFirstSceneLoad()
        {
            LoadAllAssetsAtScene(setting.GlobalAssetBundleName);
            LoadAllAssetsAtScene(SceneManager.Instance.CurrentSceneName);
        }
    }

    public partial class ResourceManager
    {
        // 씬 리소스 로딩중
        public bool IsLoading => isLoading;

        // 씬 리소스 언로딩중
        public bool IsUnloading => isUnloading;

        // 매니저 기본 설정
        public ResourceManagerSetting Setting => setting;
    }
}