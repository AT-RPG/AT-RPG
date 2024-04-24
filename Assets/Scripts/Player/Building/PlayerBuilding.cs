using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 건설기능을 구현하는 클래스            <br/>
    /// 1. 건설 인디케이터를 표시합니다.                 <br/>
    /// 2. 지정된 건설 오브젝트를 인스턴싱합니다.        <br/>
    /// </summary>
    public partial class PlayerBuilding : MonoBehaviour
    {
        // 건설할 프리팹
        [SerializeField] private AssetReferenceResource<GameObject> buildingPrefab = null;

        // 건설 인디케이터 프리팹
        [SerializeField] private AssetReferenceResource<GameObject> buildingIndicatorPrefab = null;

        // 건설할 프리팹 선택 UI
        [SerializeField] private AssetReferenceResource<GameObject> buildingSystemPopupPrefab = null;

        // 건설 인디케이터 인스턴스
        private BuildingIndicator buildingIndicatorInstance = null;

        // 건설모드 활성/비활성
        private static bool isBuildModeEnabled = false;

        // 건설 시, 플레이어의 공격을 잠시 막기위해 사용
        private PlayerController playerController = null;


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
                UIManager.InstantiatePopup(buildingSystemPopupPrefab, PopupRenderMode.Default);

                buildingIndicatorInstance = Instantiate<GameObject>(buildingIndicatorPrefab, transform).GetComponent<BuildingIndicator>();
                buildingIndicatorInstance.SetBuilding(buildingPrefab);

                // 플레이어의 공격을 잠시 제거
                InputManager.RemoveKeyAction("Attack/Fire", playerController.Attack);
            }
            else
            {
                Destroy(buildingIndicatorInstance.gameObject);

                // 플레이어의 공격을 활성화
                InputManager.AddKeyAction("Attack/Fire", playerController.Attack);
            }
        }

        /// <summary>
        /// <see cref="buildingPrefab"/>를 건설 인디케이터에서 표시하는 위치에 건설합니다.
        /// </summary>
        public void Build(InputValue value)
        {
            if (!isBuildModeEnabled) { return; }

            if (buildingIndicatorInstance.Status != IndicatorStatus.Approved) { return; }

            Instantiate<GameObject>(buildingPrefab, buildingIndicatorInstance.BuildingPosition, buildingIndicatorInstance.BuildingRotation);
        }
    }

    public partial class PlayerBuilding
    {
        // 건설모드 활성/비활성
        public static bool IsBuildModeEnabled => isBuildModeEnabled;
    }
}
