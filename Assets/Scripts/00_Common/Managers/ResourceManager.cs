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
        [SerializeField] private static ResourceManagerSetting setting;

        // 리소스 해쉬맵
        private static SceneResourceMap resources = new SceneResourceMap();

        // 에셋 번들 해쉬맵
        private static AssetBundleMap bundles = new AssetBundleMap();

        // 리소스 언로드 대기열
        private static Queue<UnloadRequest> unloadQueue = new Queue<UnloadRequest>();

        // 리소스 로딩중
        private static bool isLoading = false;

        // 리소스 성공 시 콜백
        public delegate void CompletedCallback();

        // 시작 조건 콜백 true->로딩 시작, false->로딩 대기
        public delegate bool StartConditionCallback();



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            GameManager.BeforeFirstSceneLoadAction += LoadAllResourcesFromResourcesFolder;
            GameManager.BeforeFirstSceneLoadAction += LoadAllResourcesFromAssetBundleGlobal;
            GameManager.BeforeFirstSceneLoadAction += LoadAllResourcesFromAssetBundleFirstScene;
        }

        private void Update()
        {
            ProcessUnloadQueue();
        }



        /// <summary>
        /// 씬에서 사용되는 모든 리소스를 비동기적으로 로드
        /// </summary>
        /// <param name="sceneName">리소스가 사용되는 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        public static void LoadAllResourcesCoroutine(
            string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            if (isLoading)
            {
                Debug.LogError("리소스가 로딩중입니다.");
                return;
            }

            Instance.StartCoroutine(InternalLoadAllResourcesCoroutine(sceneName, started, completed));
        }

        /// <summary>
        /// 씬에서 사용되는 모든 리소스를 동기적으로 로드
        /// </summary>
        /// <param name="sceneName">리소스가 사용되는 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        public static void LoadAllResources(string sceneName, CompletedCallback completed = null)
        {
            if (isLoading)
            {
                Debug.LogError("리소스가 로딩중입니다.");
                return;    
            }

            InternalLoadAllResources(sceneName, completed);
        }

        /// <summary>
        /// 씬에 모든 리소스를 비동기적으로 언로드하는 요청을 보냅니다.
        /// </summary>
        public static void UnloadAllResourcesCoroutine(
            string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            unloadQueue.Enqueue(new UnloadRequest(sceneName, started, completed));
        }

        /// <summary>
        /// 씬에 모든 리소스를 동기적으로 즉시 언로드합니다.
        /// </summary>
        public static void UnloadAllResources(string sceneName, CompletedCallback completed = null)
        {
            InternalUnloadAllResources(sceneName, completed);
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스를 획득                       <br/>
        /// NOTE : SceneManager의 CurrentSceneName을 사용      <br/>
        /// </summary>
        /// <param name="resourceName">찾을 리소스 이름</param>
        public static T Get<T>(string resourceName) where T : UnityObject
        {
            T resource = null;
            string typeName = typeof(T).Name;
            string currSceneName = SceneManager.CurrentSceneName;
            string globalBundleName = setting.GlobalAssetBundleName;


            // 현재 씬에 리소스가 등록되었는지?
            if (resources.ContainsKey(currSceneName) &&
                resources[currSceneName].ContainsKey(typeName) &&
                resources[currSceneName][typeName].ContainsKey(resourceName))
            {
                // 타입 형변환이 가능한지?
                resource = resources[currSceneName][typeName][resourceName] as T;
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

            // 리소스 폴더에 등록되었는지?
            if (resources.ContainsKey("Resources") &&
                resources["Resources"].ContainsKey(typeName) &&
                resources["Resources"][typeName].ContainsKey(resourceName))
            {
                // 타입 형변환이 가능한지?
                resource = resources["Resources"][typeName][resourceName] as T;
                if (resource)
                {
                    return resource;
                }
            }

            Debug.LogError($"{nameof(ResourceManager)}.cs에서 {resourceName}을 로드할 수 없습니다. 하단의 로그를 확인해주세요. \n" +
                           $"1. {resourceName}를 {typeName}로 형변환 할 수 없습니다. \n" +
                           $"2. {resourceName}가 리소스 폴더나 에셋번들의 리소스가 아닙니다. 에셋 번들 재빌드나 리소스 GUID 재빌드를 하셨나요? \n");
            
            return resource;
        }

        /// <summary>
        /// 씬에 로드된 모든 리소스를 획득
        /// </summary>
        public static UnityObject[] GetAll(string sceneName)
        {
            List<UnityObject> sceneResources = new List<UnityObject>();

            var sceneResourceMap = resources[sceneName].Values;
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
        /// 언로드 요청을 수락합니다.
        /// </summary>
        private static void ProcessUnloadQueue()
        {
            // 언로드 요청이 있음?
            if (unloadQueue.Count <= 0)
            {
                return;
            }

            // 언로드 요청 수락
            var unloadRequest = unloadQueue.Dequeue();
            Instance.StartCoroutine(InternalUnloadAllResourcesCoroutine(
                unloadRequest.SceneName, unloadRequest.StartCondition, unloadRequest.Completed));
        }

        /// <summary>
        /// 씬의 리소스 비동기 로딩
        /// </summary>
        private static IEnumerator InternalLoadAllResourcesCoroutine(
            string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            isLoading = true;

            // 시작 조건
            if (started != null)
            {
                while (!started.Invoke())
                {
                    yield return null;
                }
            }

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
            MapAssetBundlesAtScene(sceneName, loadedAssetBundle);
            MapResourcesAtScene(sceneName, assetRequest.allAssets);

            // 페이크 로딩
            yield return LoadFakeDuration();

            isLoading = false;
            yield return null;
            completed?.Invoke();
        }

        /// <summary>
        /// 씬의 리소스 동기 로딩
        /// </summary>
        private static void InternalLoadAllResources(string sceneName, CompletedCallback completed)
        {
            isLoading = true;

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
            MapAssetBundlesAtScene(sceneName, loadedAssetBundle);
            MapResourcesAtScene(sceneName, loadedResources);

            isLoading = false;
            completed?.Invoke();
        }

        /// <summary>
        /// 씬의 리소스 비동기 언로딩
        /// </summary>
        private static IEnumerator InternalUnloadAllResourcesCoroutine(
            string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            // 시작 조건
            if (started != null)
            {
                while (!started.Invoke())
                {
                    yield return null;
                }
            }

            // 현재 씬에 에셋 번들이 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
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

            completed?.Invoke();
        }

        /// <summary>
        /// 씬의 리소스 동기 언로딩
        /// </summary>
        private static void InternalUnloadAllResources(string sceneName, CompletedCallback completed = null)
        {
            // 현재 씬에 매핑된 에셋 번들이 있음?
            if (!bundles.ContainsKey(sceneName) || bundles[sceneName] == null)
            {
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

            completed?.Invoke();
        }

        /// <summary>
        /// 씬에 에셋 번들을 매핑
        /// </summary>
        private static void MapAssetBundlesAtScene(string sceneName, AssetBundle loadedAssetBundle)
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
        private static void MapResourcesAtScene(string sceneName, UnityObject[] loadedResources)
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
        private static IEnumerator LoadFakeDuration()
        {
            float fakeLoadingElapsedTime = 0f;
            float fakeLoadingDuration = setting.FakeLoadingDuration;
            while (fakeLoadingElapsedTime <= fakeLoadingDuration)
            {
                fakeLoadingElapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private static void LoadAllResourcesFromAssetBundleFirstScene()
        {
            LoadAllResources(SceneManager.CurrentSceneName);
        }

        /// <summary>
        /// 게임시작 시, 리소스 폴더의 리소스 로드
        /// </summary>
        private static void LoadAllResourcesFromResourcesFolder()
        {
            UnityObject[] loadedResources = Resources.LoadAll("");
            MapResourcesAtScene("Resources", loadedResources);
        }

        /// <summary>
        /// 게임시작 시, 글로벌 에셋 번들의 리소스 로드
        /// </summary>
        private static void LoadAllResourcesFromAssetBundleGlobal()
        {
            LoadAllResources(setting.GlobalAssetBundleName);
        }
    }

    public partial class ResourceManager
    {
        // 매니저 기본 설정
        public static ResourceManagerSetting Setting => setting;

        // 리소스 로딩중
        public static bool IsLoading => isLoading;
    }
}