using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


namespace AT_RPG.Manager
{
    /// <summary>
    /// 어드레서블 에셋을 미리 로드하는 클래스
    /// </summary>
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        // 기본 설정
        [SerializeField] private ResourceManagerSetting defaultSetting;
        private static ResourceManagerSetting setting;

        // 어드레서블에서 로드한 리소스를 어드레서블 Guid와 매핑하는 클래스
        private static Dictionary<string, Object> resourceMap = new Dictionary<string, Object>();

        // 어드레서블에서 로드한 리소스 래퍼런스를 소유하는 핸들
        private static List<AsyncOperationHandle> resourceHandle = new List<AsyncOperationHandle>();

        // 리소스 로드 대기열
        private static Queue<LoadRequest> loadQueue = new Queue<LoadRequest>();

        // 리소스 언로드 대기열
        private static Queue<UnloadRequest> unloadQueue = new Queue<UnloadRequest>();

        // 리소스 로드를 성공하면, 이 바인딩이 호출됩니다.
        public delegate void LoadCompleted();

        // 리소스 로드를 시작할려면, 이 바인딩이 True가 되야합니다.
        public delegate bool LoadStartCondition();



        protected override void Awake()
        {
            base.Awake();
            setting = defaultSetting;
        }



        /// <summary>
        /// 사전 로드된 어드레서블 리소스를 가져옵니다.
        /// </summary>
        /// <param name="guid"> '<see cref="AssetReference.AssetGUID"/>'를 여기에 넣어주세요. </param>
        public static T Get<T>(string guid) where T : Object
        {
            T resource = null;
            return resource;
        }
    }

    public partial class ResourceManager
    {
        // 기본 설정
        public static ResourceManagerSetting Setting => setting;

        // 리소스 로딩중
        public static bool IsLoading => loadQueue.Count > 0;

        // 리소스 언로딩중
        public static bool IsUnloading => unloadQueue.Count > 0;
    }
}