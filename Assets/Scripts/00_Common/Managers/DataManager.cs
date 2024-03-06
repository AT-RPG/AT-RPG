using UnityEngine;

namespace AT_RPG.Manager
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private DataManagerSetting setting;

        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }
    }
}
