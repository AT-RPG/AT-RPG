using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace TestResource
{
    /// <summary>
    /// Key1 = 씬 이름, Key2 = 리소스 이름, Value1 = 리소스 풀
    /// </summary>
    public class ResourceMap : Dictionary<string, Dictionary<string, TestObjectPool<Object>>> { }

    public class TestResourceManager : MonoBehaviour
    {
        public static string                AssetBundleBuildDir = Application.streamingAssetsPath;
        private static TestResourceManager  instance = null;
        private ResourceMap                 loadedResources = new ResourceMap();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private AsyncOperation LoadResourcesUsedFromScene(string sceneName)
        {
            // 에셋 번들 로드
            AssetBundleCreateRequest assetBundleRequest =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, sceneName));
            yield return assetBundleRequest;

            if (!assetBundleRequest.isDone)
            {
                Debug.Log("에셋 번들 로드 실패");
                return;
            }

            // 에셋 번들의 리소스들 로드
            AssetBundleRequest assetRequest = assetBundleRequest.assetBundle.LoadAllAssetsAsync();
            yield return assetRequest;

            if (!assetRequest.isDone)
            {
                Debug.Log("에셋 불러오기 실패");
                return;
            }

            // 현재 Scene의 리소스들을 매핑
            foreach (var resource in assetRequest.allAssets)
            {
                //loadedResources[sceneName][resource.name] = new TestObjectPool<Object>(
                //        resource,
                //        CreatePooledItem,
                //        OnReturnedToPool,
                //        OnTakeFromPool,
                //        OnDestroyPoolObject
                //    );
            }
            

        }

        //private AsyncOperation UnloadResourcesUnusedFromScene(string sceneName)
        //{

        //}

        //public T Load<T>(string resourceName) where T : Object
        //{
        //    return loadedResources[SceneManager.GetActiveScene().name][resourceName].Get() as T;
        //}

        //public Object Load(string resourceName)
        //{
        //    return loadedResources[SceneManager.GetActiveScene().name][resourceName].Get();
        //}


        public TestResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject testResourceManagerInstance = new GameObject();
                    testResourceManagerInstance.AddComponent<TestResourceManager>();

                    return instance;
                }

                return instance;
            }
        }
    }

}