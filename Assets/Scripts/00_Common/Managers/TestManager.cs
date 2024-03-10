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
                SceneManager.LoadSceneCoroutine("MainScene_BJW");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.LoadSceneCoroutine("MainScene_CSH");
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.LoadSceneCoroutine("MainScene_IJH");
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.LoadSceneCoroutine("MainScene_JUJ");
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                DataManager.SaveCoroutine(DataManager.Setting.DefaultSaveFolderPath, "MainScene_BJW", null);
                Debug.Log("세이브 성공");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                DataManager.LoadCoroutine(DataManager.Setting.DefaultSaveFolderPath, "MainScene_BJW", null);
                Debug.Log("로드 성공");
            }
        }
    }
}