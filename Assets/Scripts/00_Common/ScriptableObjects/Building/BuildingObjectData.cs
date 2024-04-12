using System;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 건물이 가지고 있는 특성입니다.
    /// </summary>
    [Flags]
    public enum BuildingAttribute
    {
        // 아무런 기능이 없습니다.
        None,

        // 건물의 기본 발판이 됩니다. 
        // 땅에 건물이 식물을 심듯이 건설될 수 있습니다.
        Platform,
    }


    [Serializable]
    [CreateAssetMenu(fileName = "BuildingObjectData", menuName = "ScriptableObject/BuildingObject Data", order = int.MaxValue)]
    public class BuildingObjectData : ScriptableObject
    {
        // 이 값이 0이 되는경우, 객체가 파괴됩니다.
        [SerializeField, Range(0f, 999f)] public float MaxHP;

        // 건물이 가지는 특성입니다.
        [SerializeField] public BuildingAttribute Attribute = BuildingAttribute.None;
    }
}