using AT_RPG;
using AT_RPG.Manager;
using UnityEngine;

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
            SceneManager.Instance.Load("MainScene_BJW");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.Instance.Load("MainScene_CSH");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.Instance.Load("MainScene_IJH");
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.Instance.Load("MainScene_JUJ");
        }
    }
}

#endif