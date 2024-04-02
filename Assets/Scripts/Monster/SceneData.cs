using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Instance;
    public  Transform hpBarsTransform;
    // Update is called once per frame
    private void Awake()
    {
       Instance = this;
    }
}
