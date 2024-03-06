using Unity.Collections;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "DataManagerSettings", menuName = "ScriptableObject/DataManager Setting")]
    public class DataManagerSetting : ScriptableObject
    {
        [ReadOnly] public static readonly string DefaultSaveFolderPath 
            = System.IO.Path.Combine(Application.persistentDataPath, "Saves");

    }
}
