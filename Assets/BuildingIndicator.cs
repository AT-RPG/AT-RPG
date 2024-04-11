using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어가 해당 위치에 건설이 가능한지를 설정합니다.
    /// </summary>
    public enum IndicatorStatus
    {
        // 건설이 가능합니다.
        Approved,

        // 건설할 수 없습니다.
        Rejected
    }

    /// <summary>
    /// 플레이어가 건설할 건물의 '위치/모양/건설가능' 여부를 구현합니다.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public partial class BuildingIndicator : MonoBehaviour
    {
        // 건설가능 여부
        private IndicatorStatus status = IndicatorStatus.Rejected;

        private MeshFilter filter = null;

        private MeshRenderer meshRenderer = null;

        private void Awake()
        {
            filter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    public partial class BuildingIndicator
    {
        public IndicatorStatus Status => status;
    }
}