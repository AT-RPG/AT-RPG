using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 건설기능을 구현하는 클래스            <br/>
    /// 1. 건설 인디케이터를 표시합니다.                 <br/>
    /// 2. 지정된 건설 오브젝트를 인스턴싱합니다.        <br/>
    /// </summary>
    public class PlayerBuilding : MonoBehaviour
    {
        // 건설할 프리팹
        [SerializeField] private AssetReferenceResource<GameObject> buildingPrefab = null;

        // 건설 인디케이터 프리팹
        [SerializeField] private AssetReferenceResource<GameObject> buildingIndicatorPrefab = null;

        // 건설 인디케이터 인스턴스
        private BuildingIndicator buildingIndicatorInstance = null;

        // 건설모드 활성/비활성
        private bool isBuildModeEnabled = false;



        private void Awake()
        {
            InputManager.AddKeyAction("BuildMode", OnEnableBuildingMode);
            InputManager.AddKeyAction("Attack/Fire", OnBuild);
        }

        private void OnDestroy()
        {
            InputManager.RemoveKeyAction("BuildMode", OnEnableBuildingMode);
            InputManager.RemoveKeyAction("Attack/Fire", OnBuild);
        }



        /// <summary>
        /// 건설 인디케이터가 화면에 나오도록 건설모드를 활성/비활성합니다.
        /// </summary>ㄴ
        private void OnEnableBuildingMode(InputValue value)
        {
            // 토글
            isBuildModeEnabled = isBuildModeEnabled ? false : true;

            if (isBuildModeEnabled) 
            {
                buildingIndicatorInstance = Instantiate<GameObject>(buildingIndicatorPrefab, transform).GetComponent<BuildingIndicator>();
                buildingIndicatorInstance.SetBuildingInstance(buildingPrefab);
            }
            else
            {
                Destroy(buildingIndicatorInstance.gameObject);
            }
        }

        /// <summary>
        /// <see cref="buildingPrefab"/>를 건설 인디케이터에서 표시하는 위치에 건설합니다.
        /// </summary>
        private void OnBuild(InputValue value)
        {
            if (!isBuildModeEnabled) { return; }

            if (buildingIndicatorInstance.Status != IndicatorStatus.Approved) { return; }

            GameObject building = Instantiate<GameObject>(buildingPrefab, buildingIndicatorInstance.transform.position, buildingIndicatorInstance.transform.rotation);
            if (buildingIndicatorInstance.IsSnapEnable)
            {
                buildingIndicatorInstance.SetSnapBuildingInstance(building);
            }
        }
    }
}
