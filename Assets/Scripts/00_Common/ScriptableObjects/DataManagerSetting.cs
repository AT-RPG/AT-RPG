using Unity.Collections;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "DataManagerSettings", menuName = "ScriptableObject/DataManager Setting")]
    public class DataManagerSetting : ScriptableObject
    {
        [ReadOnly] public string DefaultSaveFolderPath;

        public void OnEnable()
        {
            DefaultSaveFolderPath = System.IO.Path.Combine(Application.persistentDataPath, "Saves");
        }
    }
}
