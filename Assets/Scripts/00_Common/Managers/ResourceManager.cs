using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // 어드레서블에서 로드한 리소스를 어드레서블 Guid와 매핑하는 클래스
        private static Dictionary<string, UnityObject> resources = new();

        // 어드레서블에서 로드한 리소스 래퍼런스를 소유하는 핸들
        private static List<AsyncOperationHandle> resourceHandles = new();

        /// 동작중인 리소스 로드
        /// 로드 시, <see cref="LoadAsync(string, StartCondition, Completion)"/>를 Add
        private static List<ResourceRequest> loadOperations = new();

        /// 동작중인 리소스 언로드
        /// 언로드 시, <see cref="UnloadAsync(string, StartCondition, Completion)"/>를 Remove
        private static List<ResourceRequest> unloadOperations = new();

        // 리소스 로드를 성공하면, 이 바인딩이 호출됩니다.
        public delegate void Completion();

        // 리소스 로드를 시작할려면, 이 바인딩이 True가 되야합니다.
        public delegate bool StartCondition();



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<ResourceManagerSettings>($"{nameof(ResourceManagerSettings)}");
        }



        private void OnDestroy()
        {
            Resources.UnloadAsset(setting);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="label">로드할 <see cref="AssetLabelReference.labelString"/></param>
        /// <param name="started">로드 시작조건</param>
        /// <param name="completed">로드 종료 시, 콜백</param>
        public static void LoadAsync(List<string> labels, StartCondition started = null, Completion completed = null)
        {
            // 현재 로딩에 고유 식별자를 부여
            // 이 식별자로 현재 동작중인 로딩들을 관리
            ResourceRequest newRequest = new()
            {
                RequestId = Guid.NewGuid(),
                Labels = labels,
                Started = started,
                Completed = completed
            };

            Instance.StartCoroutine(LoadAsyncImpl(newRequest));
        }

        private static IEnumerator LoadAsyncImpl(ResourceRequest request)
        {
            loadOperations.Add(request);

            // 시작조건 콜백
            while (request.Started?.Invoke() ?? true) { yield return null; }

            // 라벨이 붙은 모든 어드레서블 에셋의 위치 로드
            var locationHandle = Addressables.LoadResourceLocationsAsync(request.Labels, Addressables.MergeMode.Union);
            yield return locationHandle;
            if (locationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{request.Labels}에서 어드레서블 에셋의 위치 로드 실패.");
                yield break;
            }

            // 로드한 어드레서블 리소스들을 매핑
            int locationIndex = 0;
            var resourceHandle = Addressables.LoadAssetsAsync<UnityObject>(locationHandle.Result, resource => 
            {
                resources.Add(AssetGuidMap.Map[locationHandle.Result[locationIndex].PrimaryKey], resource);
                locationIndex++;
            });
            yield return resourceHandle;
            if (resourceHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{request.Labels}에서 어드레서블 리소스 로드 실패.");
                yield break;
            }

            // 완료 콜백
            request.Completed?.Invoke();

            loadOperations.Remove(request);
        }



        //public static async Task UnloadAsync(string label, StartCondition started, Completion completed)
        //{
            
        //}

        //private static async Task UnloadAsyncImpl(string label, StartCondition started, Completion completed)
        //{

        //}



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

        // 리소스 언로딩중
        public static bool IsUnloading => unloadOperations.Count > 0;
    }
}