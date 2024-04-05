using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityObject = UnityEngine.Object;


namespace AT_RPG.Manager
{
    /// <summary>
    /// 어드레서블 에셋을 미리 로드하는 클래스
    /// </summary>
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        // 기본 설정
        private static ResourceManagerSettings setting;

        // 매핑을 통해 캐싱된 리소스에 접근
        private static AssetGuidMap assetGuidMap = new();

        // 현재 캐시된 어드레서블 리소스
        private static ResourceMap resources = new();

        // 어드레서블에서 로드한 리소스를 래퍼런싱하는 핸들
        private static ResourceHandleMap resourceHandles = new();

        // 동작중인 리소스 로딩
        private static List<ResourceRequest> loadOperations = new();

        // 동작중인 리소스 언로딩
        private static List<ResourceRequest> unloadOperations = new();



        protected override void Awake()
        {
            base.Awake();
            setting = Resources.Load<ResourceManagerSettings>("ResourceManagerSettings");
            assetGuidMap = AssetGuidMap.Load();
        }



        private void Update()
        {
            
        }



        private void OnDestroy()
        {
            Resources.UnloadAsset(setting);
        }



        /// <summary>
        /// 어드레서블 리소스를 매니저에 캐싱합니다.<br/>
        /// </summary>
        /// <param name="key">로드할 어드레서블 에셋의 <see cref="AssetReference.AssetGUID"/></param>
        /// <param name="completed">로드 성공시 콜백</param>
        public static void LoadAssetAsync<TObject>(string key, Action<TObject> completed = null) where TObject : UnityObject
        {
            Instance.StartCoroutine(LoadAssetAsyncImpl(new ResourceRequest(key), completed));
        }

        private static IEnumerator LoadAssetAsyncImpl<TObject>(ResourceRequest request, Action<TObject> completed) where TObject : UnityObject
        {
            loadOperations.Add(request);

            // 어드레서블 에셋을 로드하기 위해, 에셋의 위치를 로드
            var locationHandle = Addressables.LoadResourceLocationsAsync(request.Key);
            yield return locationHandle;
            if (locationHandle.Status != AsyncOperationStatus.Succeeded) { Debug.LogError($"{request.Key}를 사용하는 어드레서블 에셋의 위치 로드 실패."); yield break; }

            // 에셋의 위치(IResourceLocation)로 어드레서블 에셋을 로드
            // 로드된 에셋을 리소스 매니저에 캐싱
            var resourceHandle = Addressables.LoadAssetAsync<TObject>(locationHandle.Result[0]);
            resourceHandle.Completed += handle => CacheResource(request.Key, resourceHandle);
            yield return resourceHandle;
            if (resourceHandle.Status != AsyncOperationStatus.Succeeded) { Debug.LogError($"{request.Key}를 사용하는 어드레서블 리소스 로드 실패."); yield break; }

            Addressables.Release(locationHandle);
            loadOperations.Remove(request);
            completed?.Invoke(resourceHandle.Result);
        }

        private static void CacheResource<TObject>(string key, AsyncOperationHandle<TObject> resourceHandle) where TObject : UnityObject
        {
            resources.Add(key, key, resourceHandle.Result);
            resourceHandles.Add(key, resourceHandle);
        }



        /// <summary>
        /// 어드레서블 리소스를 매니저에 캐싱합니다.<br/>
        /// </summary>
        /// <param name="key">로드할 어드레서블 에셋의 <see cref="AssetLabelReference.labelString"/></param>
        /// <param name="completed">로드 성공시 콜백</param>

        public static void LoadAssetsAsync(string key, Action<List<UnityObject>> completed = null)
        {
            Instance.StartCoroutine(LoadAssetsAsyncImpl(new ResourceRequest(key), completed));
        }

        private static IEnumerator LoadAssetsAsyncImpl(ResourceRequest request, Action<List<UnityObject>> completed)
        {
            loadOperations.Add(request);

            // 어드레서블 에셋을 로드하기 위해, 에셋의 위치를 로드
            var locationHandle = Addressables.LoadResourceLocationsAsync(request.Key);
            yield return locationHandle;
            if (locationHandle.Status != AsyncOperationStatus.Succeeded) { Debug.LogError($"{request.Key}를 사용하는 어드레서블 에셋의 위치 로드 실패."); yield break; }


            // 에셋의 위치(IResourceLocation)로 어드레서블 에셋을 로드
            // 로드된 에셋을 리소스 매니저에 캐싱
            var resourceHandle = Addressables.LoadAssetsAsync<UnityObject>(locationHandle.Result, null);
            resourceHandle.Completed += handle => CacheResources(request.Key, locationHandle, resourceHandle);
            yield return resourceHandle;
            if (resourceHandle.Status != AsyncOperationStatus.Succeeded) { Debug.LogError($"{request.Key}를 사용하는 어드레서블 리소스 로드 실패."); yield break; }

            Addressables.Release(locationHandle);
            loadOperations.Remove(request);
            completed?.Invoke(resourceHandle.Result.ToList());
        }

        private static void CacheResources(string key, AsyncOperationHandle<IList<IResourceLocation>> locationHandle, AsyncOperationHandle<IList<UnityObject>> resourceHandle)
        {
            var resources = resourceHandle.Result.ToList();
            var locations = locationHandle.Result.ToList();

            for (int i = 0; i < resources.Count; i++) { ResourceManager.resources.Add(assetGuidMap[locations[i].InternalId], new(key, resources[i])); }
            resourceHandles.Add(key, resourceHandle);
        }



        /// <summary>
        /// 매니저에 캐싱된 어드레서블 리소스를 언로드합니다. <br/>
        /// </summary>
        /// <param name="key">로드 시 사용했던 key</param>
        public static void UnloadAssetsAsync(string key, Action completed = null)
        {
            Instance.StartCoroutine(UnloadAssetsAsyncImpl(new ResourceRequest(key), completed));
        }

        private static IEnumerator UnloadAssetsAsyncImpl(ResourceRequest request, Action completed)
        {
            unloadOperations.Add(request);

            // 핸들 언로드를 통해 래퍼런스를 제거
            Addressables.Release(resourceHandles[request.Key]);
            resourceHandles.Remove(request.Key);

            // 리소스가 key에 의해 생성된 리소스면 언로드
            List<string> keysToRemove = new();
            foreach (var resource in resources) { if (resource.Value.Key == request.Key) { keysToRemove.Add(resource.Key); } }
            foreach (var keyToRemove in keysToRemove) { resources.Remove(keyToRemove); }
            yield return Resources.UnloadUnusedAssets();

            loadOperations.Remove(request);
            completed?.Invoke();
        }



        /// <summary>
        /// 사전 로드된 어드레서블 리소스를 가져옵니다.
        /// </summary>
        /// <param name="key">'<see cref="AssetReference.AssetGUID"/>'</param>
        public static T Get<T>(string key) where T : UnityObject => resources[key].Value as T;
    }

    public partial class ResourceManager
    {
        // 기본 설정
        public static ResourceManagerSettings Setting => setting;

        // 리소스 로딩중
        public static bool IsLoading => loadOperations.Count > 0;
    }
}