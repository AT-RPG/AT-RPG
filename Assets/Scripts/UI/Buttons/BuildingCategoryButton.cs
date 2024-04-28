using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class BuildingCategoryButton : Button
    {
        [Header("이 카테고리에 묶여있는 건설 가능한 대상을 여기에 바인딩")]
        [Space(5)]
        [SerializeField] private List<GameObject> buildingPrefabs;
    }
}