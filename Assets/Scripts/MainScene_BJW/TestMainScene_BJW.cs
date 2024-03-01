using AT_RPG.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMainScene_BJW : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var resource in ResourceManager.Instance.GetAll(SceneManager.Instance.CurrentSceneName))
        {
            Instantiate(resource);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
