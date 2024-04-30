using AT_RPG.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 건설기능을 구현하는 클래스            <br/>
    /// 1. 건설 인디케이터를 표시합니다.                 <br/>
    /// 2. 지정된 건설 오브젝트를 인스턴싱합니다.        <br/>
    /// </summary>
    public partial class PlayerBuildingController : MonoBehaviour
    {
        // 건설할 건물 프리팹
        private AssetReferenceResource<GameObject> selectedBuildingPrefab = null;

        // 건설 인디케이터 프리팹
        [SerializeField] private AssetReferenceResource<GameObject> buildingIndicatorPrefab = null;

        // 건설할 프리팹 선택 UI
        [SerializeField] private AssetReferenceResource<GameObject> buildingPopupPrefab = null;

        // 건설 인디케이터 인스턴스
        private BuildingIndicator buildingIndicatorInstance = null;

        // 건설 UI
        private BuildingPopup buildingPopupInstance = null;

        // 건설모드 활성/비활성
        private static bool isBuildModeEnabled = false;

        // 건설 시, 플레이어의 공격을 잠시 막기위해 사용
        private PlayerController playerController = null;

        // 건설 가능한 건물들은 여기에
        [SerializeField] private List<AssetReferenceResource<GameObject>> buildingPrefabs = null;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();

            InputManager.AddKeyAction("BuildMode", EnableBuildingMode);
            InputManager.AddKeyAction("Attack/Fire", Build);
        }

        private void OnDestroy()
        {
            InputManager.RemoveKeyAction("BuildMode", EnableBuildingMode);
            InputManager.RemoveKeyAction("Attack/Fire", Build);
        }



        /// <summary>
        /// 건설 인디케이터가 화면에 나오도록 건설모드를 활성/비활성합니다.
        /// </summary>ㄴ
        public void EnableBuildingMode(InputValue value)
        {
            // 토글
            isBuildModeEnabled = isBuildModeEnabled ? false : true;

            if (isBuildModeEnabled) 
            {
                // 건설 UI 생성
                buildingPopupInstance = UIManager.InstantiatePopup(buildingPopupPrefab, PopupRenderMode.Default).GetComponent<BuildingPopup>();

                // 건설 인디케이터 생성
                buildingIndicatorInstance = Instantiate<GameObject>(buildingIndicatorPrefab, transform).GetComponent<BuildingIndicator>();

                // 플레이어의 공격을 잠시 제거
                InputManager.RemoveKeyAction("Attack/Fire", playerController.Attack);
            }
            else
            {
                Destroy(buildingIndicatorInstance.gameObject);
                Destroy(buildingPopupInstance.gameObject);

                // 플레이어의 공격을 활성화
                InputManager.AddKeyAction("Attack/Fire", playerController.Attack);
            }
        }

        /// <summary>
        /// <see cref="selectedBuildingPrefab"/>를 건설 인디케이터에서 표시하는 위치에 건설합니다.
        /// </summary>
        public void Build(InputValue value)
        {
            if (!isBuildModeEnabled) { return; }

            if (buildingIndicatorInstance.Status != IndicatorStatus.Approved) { return; }

            Instantiate<GameObject>(selectedBuildingPrefab, buildingIndicatorInstance.BuildingPosition, buildingIndicatorInstance.BuildingRotation);
        }

        private void OnSetBuilding(AssetReferenceResource<GameObject> buildingPrefab)
        {
            this.selectedBuildingPrefab = buildingPrefab;
        }
    }

    public partial class PlayerBuildingController
    {
        // 건설모드 활성/비활성
        public static bool IsBuildModeEnabled => isBuildModeEnabled;
    }
}
