using UnityEngine;

namespace AT_RPG.Manager
{
    public class TestManager : Singleton<TestManager>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
            if (Input.GetKeyDown(UnityEngine.KeyCode.F1))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_BJW", LoadMode.LoadingResources);
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.F2))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_CSH", LoadMode.LoadingResources);
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.F3))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_IJH", LoadMode.LoadingResources);
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.F4))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_JUJ", LoadMode.LoadingResources);
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.F5))
            {
                DataManager.Instance.SaveAsCor(DataManager.Instance.Setting.DefaultSaveFolderPath, "MainScene_BJW", null);
                Debug.Log("세이브 성공");
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.F6))
            {
                DataManager.Instance.LoadCor(DataManager.Instance.Setting.DefaultSaveFolderPath, "MainScene_BJW", null);
                Debug.Log("로드 성공");
            }
        }
    }
}