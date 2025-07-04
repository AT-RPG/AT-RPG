using System;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
    public class SerializableData
    {
        // 이 데이터를 소유하고 있는 컴포넌트 이름
        [SerializeField] public string ComponentTypeName;

        [Obsolete] public SerializableData()
        {

        }

        public SerializableData(Component owner)
        {
            ComponentTypeName = owner.GetType().Name;
        }
    }
}