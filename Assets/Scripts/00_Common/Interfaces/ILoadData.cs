using UnityEngine;

namespace AT_RPG
{
    public interface ILoadData
    {
        /// <summary>
        /// SaveLoadManager를 통해 데이터를 불러온 후, 호출
        /// </summary>
        public void LoadData(GameObjectData data);
    }
}