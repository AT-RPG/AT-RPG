using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using System;
using UnityObject = UnityEngine.Object;
using System.Linq;

namespace AT_RPG.Manager
{   
    public class SceneResourceMap : Dictionary<string, ResourceMap> { }
    public class ResourceMap : Dictionary<string, UnityObject> { }

    public class ResourceManager : Singleton<ResourceManager>
    {
        private static string ResourceFolderPath = Path.Combine(
                Application.dataPath, "Resources"
            );

        private SceneResourceMap resources = new SceneResourceMap();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// 씬에 모든 리소스를 동기적으로 로드
        /// </summary>
        public void LoadAll(string sceneName)
        {
            string[] dirPaths = Directory.GetDirectories(Path.Combine(ResourceFolderPath, sceneName));
            InternalLoadAll(dirPaths, sceneName);
        }

        /// <summary>
        /// 씬에 모든 리소스를 비동기적으로 로드
        /// </summary>
        public Task LoadAllAsync(string sceneName)
        {
            Task task = Task.Run(() =>
            {
                string[] dirPaths = Directory.GetDirectories(Path.Combine(ResourceFolderPath, sceneName));
                InternalLoadAll(dirPaths, sceneName);
            });

            return task;
        }

        private void InternalLoadAll(string[] dirPaths, string sceneName)
        {
            resources[sceneName] = new ResourceMap();

            // Resources의 씬(sceneName) 폴더에 있는 모든 리소스를 로드
            foreach (var dirPath in dirPaths)
            {
                UnityObject[] items = Resources.LoadAll(Path.Combine(sceneName, String.GetFolderOrFileName(dirPath)));
                foreach (var item in items)
                {
                    resources[sceneName][item.name] = item;
                }
            }
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스(resourceName)를 획득
        /// </summary>
        public UnityObject Get(string resourceName)
        {
            UnityObject resource =
                resources[SceneManager.Instance.CurrentSceneName][resourceName];

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스(resourceRefer.name)를 획득
        /// </summary>
        public UnityObject Get(UnityObject resourceRefer)
        {
            UnityObject resource =
                resources[SceneManager.Instance.CurrentSceneName][resourceRefer.name];

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스(resourceName)를 획득
        /// </summary>
        public T Get<T>(string resourceName) where T : UnityObject
        {
            T resource =
                resources[SceneManager.Instance.CurrentSceneName][resourceName] as T;

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 리소스(resourceRefer.name)를 획득
        /// </summary>
        public T Get<T>(UnityObject resourceRefer)
            where T : UnityObject
        {
            T resource =
                resources[SceneManager.Instance.CurrentSceneName][resourceRefer.name] as T;

            return resource;
        }

        /// <summary>
        /// 현재 씬에 로드된 모든 리소스(resourceName)를 획득
        /// </summary>
        public UnityObject[] GetAll(string sceneName)
        {
            UnityObject[] sceneResources =
                resources[sceneName].Values.ToArray();

            return sceneResources;
        }

        /// <summary>
        /// 씬에서 사용되지 않는 리소스를 언로드
        /// </summary>
        public Task UnLoadAllAsync(string sceneName)
        {
            Task task = Task.Run(() =>
            {
                AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
                while(!unloadOperation.isDone) 
                {

                }

                resources[sceneName] = null;
            });

            return task;
        }
    }
}