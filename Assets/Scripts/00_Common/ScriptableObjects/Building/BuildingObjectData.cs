using System;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
    [CreateAssetMenu(fileName = "BuildingObjectData", menuName = "ScriptableObject/BuildingObject Data", order = int.MaxValue)]
    public class BuildingObjectData : ScriptableObject
    {
        // 이 값이 0이 되는경우, 객체가 파괴됩니다.
        [SerializeField, Range(0f, 999f)] public float maxHP;
    }
}