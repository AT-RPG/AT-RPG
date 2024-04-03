using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        // 현재 캐시된 어드레서블 리소스
        private static ResourceMap resources = new();

        // 어드레서블에서 로드한 리소스를 래퍼런싱하는 핸들
        private static ResourceHandleMap resourceHandles = new();

        // 동작중인 리소스 로딩
        private static List<ResourceRequest> loadOperations = new();



        protected override void Awake()
        {
            base.Awake();
            setting = Resources.Load<ResourceManagerSettings>("ResourceManagerSettings");

        }



        private void OnDestroy()
        {
            Resources.UnloadAsset(setting);
        }



        /// <summary>
        /// 어드레서블 리소스를 매니저에 캐싱합니다. <br/>
        /// </summary>
        /// <param name="key">로드할 어드레서블 에셋의 <see cref="AssetLabelReference.labelString"/> 입니다.</param>
        /// <param name="started">로드 시작조건, 이 조건이 True가 되면 로드를 시작합니다.</param>
        /// <param name="completed">로드 종료 시, 콜백</param>
        public static AsyncOperationHandle<IList<UnityObject>> LoadAssetsAsync(string key)
        {
            ResourceRequest newRequest = new()
            {
                RequestId = Guid.NewGuid(),
                Key = key,
            };

            return LoadAssetsAsyncImpl(newRequest).Result;
        }

        /// <summary>
        /// 어드레서블 리소스를 매니저에 캐싱하는 동작을 구현합니다. <see cref="LoadAssetsAsync"/>에서 생성된 <see cref="ResourceRequest"/>에 맞춰 동작합니다.
        /// </summary>
        private static async Task<AsyncOperationHandle<IList<UnityObject>>> LoadAssetsAsyncImpl(ResourceRequest request)
        {
            loadOperations.Add(request);

            // 라벨이 붙은 모든 어드레서블 에셋의 위치 로드
            var locationHandle = Addressables.LoadResourceLocationsAsync(request.Key, Addressables.MergeMode.Union);
            await locationHandle.Task;
            if (locationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{request.Key}를 사용하는 어드레서블 에셋의 위치 로드 실패.");
                return default;
            }

            // 로드한 어드레서블 리소스들을 매핑
            int locationIndex = 0;
            resources[request.Key] = new();
            var resourceHandle = Addressables.LoadAssetsAsync<UnityObject>(locationHandle.Result, resource => 
            {
                resources[request.Key][AssetGuidMap.Map[locationHandle.Result[locationIndex].PrimaryKey]]= resource;
                locationIndex++;
            });
            await resourceHandle.Task;
            if (resourceHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{request.Key}에서 어드레서블 리소스 로드 실패.");
                return default;
            }

            // 리소스 매니저는 'AssetReference.AssetGUID'를 사용하기 때문에 필요없어진 'locationHandle'은 릴리즈
            // 후에 언로드할 수 있도록 'resourceHandles'를 캐싱
            Addressables.Release(locationHandle);
            resourceHandles.Add(request.Key, resourceHandle);
            loadOperations.Remove(request);

            return resourceHandle;
        }



        /// <summary>
        /// 매니저에 캐싱된 어드레서블 리소스를 언로드합니다. <br/>
        /// </summary>
        /// <param name="key">로드 시 사용했던 key</param>
        /// <param name="started">언로드 시작조건, 이 조건이 True가 되면 언로드를 시작합니다.</param>
        /// <param name="completed">로드 종료 시, 콜백</param>
        public static void Unload(string key)
        {
            ResourceRequest newRequest = new()
            {
                RequestId = Guid.NewGuid(),
                Key = key,
            };

            UnloadImpl(newRequest);
        }

        private static void UnloadImpl(ResourceRequest request)
        {
            // 핸들 언로드를 통해 래퍼런스를 제거
            Addressables.Release(resourceHandles[request.Key]);
            resourceHandles.Remove(request.Key);

            // 리소스를 언로드합니다
            Resources.UnloadUnusedAssets();
            resources.Remove(request.Key);
        }



        /// <summary>
        /// 사전 로드된 어드레서블 리소스를 가져옵니다.
        /// </summary>
        /// <param name="guid"> '<see cref="AssetReference.AssetGUID"/>'를 여기에 넣어주세요. </param>
        public static T Get<T>(string guid) where T : UnityObject
        {
            T resource = null;
            return resource;
        }
    }

    public partial class ResourceManager
    {
        // 기본 설정
        public static ResourceManagerSettings Setting => setting;

        // 리소스 로딩중
        public static bool IsLoading => loadOperations.Count > 0;
    }
}