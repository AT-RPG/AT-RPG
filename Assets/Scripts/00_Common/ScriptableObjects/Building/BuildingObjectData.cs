using System;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
    [CreateAssetMenu(fileName = "BuildingObjectData", menuName = "ScriptableObject/BuildingObject Data", order = int.MaxValue)]
    public class BuildingObjectData : ScriptableObject
    {
        // 이 값이 0이 되는경우, 객체가 파괴됩니다.
        [SerializeField, Range(0f, 999f)] public float MaxHP = 10f;

        // 이 건설 표시기가 지형에 어떻게 충돌되는지를 결정합니다.(ex. 물속에는 건물을 지을 수 없도록 제한.)
        [SerializeField] public LayerMask BuildingLayer;
    }
}