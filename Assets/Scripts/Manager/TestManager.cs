using System.Collections;
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
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_BJW", LoadMode.LoadingResources);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_CSH", LoadMode.LoadingResources);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_IJH", LoadMode.LoadingResources);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_JUJ", LoadMode.LoadingResources);
                Debug.Log(SceneManager.Instance);
            }
        }
    }
}