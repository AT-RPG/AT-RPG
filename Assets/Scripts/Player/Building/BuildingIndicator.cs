using AT_RPG.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    public partial class BuildingIndicator : MonoBehaviour
    {
        // 건설 사정거리
        [SerializeField, Range(1, 7.5f)] private float detectRange = 7.5f;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed = 90f;

        // 건설 가능 여부, 건설될 건물의 모양을 시각적으로 표현합니다.
        [SerializeField] private Material indication;

        [SerializeField] private Material outliner;

        // 이 레이어에만 크로스헤어를 따라서 건설 위치가 업데이트됩니다.
        [SerializeField] private LayerMask collisionLayer;

        private IndicatorStatus status = IndicatorStatus.Rejected;

        private BuildingObject indicatorBuildingInstance;


        // 건설모드에서 스냅 활성/비활성
        private bool isSnapEnabled = false;

        // 건설모드에서 스냅의 기준점 
        private BuildingObject snapbuildingInstance;


        private void Awake()
        {
            InputManager.AddKeyAction("RotateBuildingPositionL", RotateLeft);
            InputManager.AddKeyAction("RotateBuildingPositionR", RotateRight);
            InputManager.AddKeyAction("BuildOption_Snap", OnEnableSnap);
        }

        private void OnDestroy()
        {
            InputManager.RemoveKeyAction("RotateBuildingPositionL", RotateLeft);
            InputManager.RemoveKeyAction("RotateBuildingPositionR", RotateRight);
            InputManager.RemoveKeyAction("BuildOption_Snap", OnEnableSnap);

        }

        private void Start()
        {
            Move();
        }

        private void FixedUpdate()
        {
            Move();
        }



        /// <summary>
        /// 건물 프리팹을 건설 표시기로 사용할 수 있도록 설정합니다.
        /// </summary>
        public void SetBuildingInstance(AssetReferenceResource<GameObject> buildingPrefab)
        {
            // 이전 정보가 있다면, 초기화
            if (indicatorBuildingInstance != null)
            {
                Destroy(indicatorBuildingInstance.gameObject);
            }

            // 건물 정보를 로드
            indicatorBuildingInstance = Instantiate<GameObject>(buildingPrefab, transform).GetComponent<BuildingObject>();
            indicatorBuildingInstance.GetComponent<Collider>().isTrigger = true;
            indicatorBuildingInstance.gameObject.layer = 0;

            // 건설 표시기로 사용하도록 쉐이더 적용
            var meshRenderer = indicatorBuildingInstance.GetComponent<MeshRenderer>();
            List<Material> newMaterials = new() { indication };
            meshRenderer.SetMaterials(newMaterials);

            // 건설 표시기의 충돌 이벤트 적용
            // 건설 가능/불가능 위치를 판단하는 로직을 추가
            var buildingIndicatorObjectInstance = indicatorBuildingInstance.gameObject.AddComponent<BuildingIndicatorObject>();
            buildingIndicatorObjectInstance.OnTriggerEnterAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnTriggerEnterAction += other => OnApproveBulidObject(other);
            buildingIndicatorObjectInstance.OnTriggerStayAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnTriggerExitAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnTriggerExitAction += other => OnApproveBulidObject(other);
        }



        /// <summary>
        /// 건설모드에서 건물이 다른 건물에 맞춰서 건설될 수 있도록합니다.
        /// </summary>
        private void OnEnableSnap(InputValue value)
        {
            // 스냅 켜지있음?
            if (isSnapEnabled) 
            { 
                isSnapEnabled = false;
                SetSnapBuildingInstance(null);
                return;
            }
            // 스냄 켜야함?
            else
            {
                // 스냅 대상(Building)을 지정
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, detectRange, LayerMask.GetMask("Building")))
                {
                    // 스냅 대상으로 등록합니다. 이 Building을 기준으로 건설 위치가 정렬됩니다.
                    SetSnapBuildingInstance(hit.collider.gameObject);
                    isSnapEnabled = true;
                }
                else
                {
                    // 스냅 대상이 아님
                    var logPopup = UIManager.InstantiatePopup(UIManager.Setting.logPopupPrefab, PopupRenderMode.Default).GetComponent<LogPopup>();
                    logPopup.Log = "스냅을 활성화 할려면, 건물을 지정해야합니다.";
                }
            }
        }

        /// <summary>
        /// 스냅을 지정할 건물을 선택합니다.
        /// </summary>
        /// <param name="building">지정할 건물의 gameObject, null인 경우, 초기화합니다.</param>
        /// <remarks><paramref name="building"/>은 <see cref="BuildingObject"/>를 가지고 있어야합니다.</remarks>
        private void SetSnapBuildingInstance(GameObject building)
        {
            // 초기화임?
            if (!building && snapbuildingInstance)
            {
                // 이전에 달아줬던 외각선 쉐이더를 달아줍니다.
                var renderer = snapbuildingInstance.GetComponent<MeshRenderer>();
                var materials = renderer.materials.ToList();
                materials.Remove(materials[materials.Count - 1]);
                renderer.SetMaterials(materials);

                snapbuildingInstance = null;
            }
            // 등록임?
            else
            {
                // 스냅이 지정된 건물인지 알 수 있도록, 외각선 쉐이더를 달아줍니다.
                var renderer = building.GetComponent<MeshRenderer>();
                var materials = renderer.materials.ToList();
                materials.Add(outliner);
                renderer.SetMaterials(materials);

                // 스냅한 건물을 건물 표시기에 등록합니다.
                snapbuildingInstance = building.GetComponent<BuildingObject>();
            }
        }

        /// <summary>
        /// 건설위치를 조준점(<see cref="Camera.ScreenPointToRay(Vector3)"/>)이 가리키는 위치로 업데이트합니다.
        /// </summary>
        private void Move()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, detectRange, collisionLayer)) { return; }

            transform.position = hit.point;
        }

        /// <summary>
        /// 건설위치를 회전시킵니다.
        /// </summary>
        private void RotateLeft(InputValue inputValue)
        {
            transform.Rotate(Vector3.down, rotationSpeed * Time.fixedDeltaTime, Space.World);
        }

        /// <summary>
        /// 건설위치를 회전시킵니다.
        /// </summary>
        private void RotateRight(InputValue inputValue)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime, Space.World);
        }

        /// <summary>
        /// <see cref="BuildingIndicatorObject"/>의 충돌 이벤트에서 트리거됩니다.
        /// </summary>
        private void OnApproveBulidObject(Collider other)
        {
            ApproveBulidObject();
        }

        /// <summary>
        /// <see cref="BuildingIndicatorObject"/>의 충돌 이벤트에서 트리거됩니다.
        /// </summary>
        private void OnRejectBuildObject(Collider other)
        {
            if ((indicatorBuildingInstance.Data.BuildingLayer & (1 << other.gameObject.layer)) == 0)
            {
                RejectBuildObject();
            }
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 있도록 표시합니다.
        /// </summary>
        private void ApproveBulidObject()
        {
            status = IndicatorStatus.Approved;
            indication.SetColor("_BuildingStatusColor", Color.green);
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 없도록 표시합니다.
        /// </summary>
        private void RejectBuildObject()
        {
            status = IndicatorStatus.Rejected;
            indication.SetColor("_BuildingStatusColor", Color.red);
        }
    }

    public partial class BuildingIndicator
    {
        // 건설가능 여부
        public IndicatorStatus Status => status;
    }
}