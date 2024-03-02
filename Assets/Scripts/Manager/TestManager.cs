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
                SceneManager.Instance.LoadSceneCor("MainScene_BJW", true);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_CSH", true);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_IJH", true);
                Debug.Log(SceneManager.Instance);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_JUJ", true);
                Debug.Log(SceneManager.Instance);
            }
        }
    }
}