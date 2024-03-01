using UnityEngine;

namespace AT_RPG.Manager
{
#if UNITY_EDITOR

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
                SceneManager.Instance.LoadSceneCor("MainScene_BJW");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_CSH");
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_IJH");
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.Instance.LoadSceneCor("MainScene_JUJ");
            }
        }
    }

#endif
}