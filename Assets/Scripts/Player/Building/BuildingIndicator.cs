using AT_RPG.Manager;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public partial class BuildingIndicator : MonoBehaviour
    {
        // 건설 사정거리
        [SerializeField, Range(1, 7.5f)] private float detectRange = 7.5f;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed = 90f;

        // 건설 가능 여부, 건설될 건물의 모양을 시각적으로 표현합니다.
        [SerializeField] private Material indication;

        // 이 레이어에만 크로스헤어를 따라서 건설 위치가 업데이트됩니다.
        [SerializeField] private LayerMask collisionLayer;

        private IndicatorStatus status = IndicatorStatus.Rejected;

        private BuildingObject buildingObjectInstance;

        private MeshRenderer meshRenderer;

        // 건설모드에서 스냅 활성/비활성
        private bool isSnapEnabled = false;



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
        public void SetBuilding(AssetReferenceResource<GameObject> buildingPrefab)
        {
            // 이전 정보가 있다면, 초기화
            if (buildingObjectInstance != null)
            {
                Destroy(buildingObjectInstance.gameObject);
            }

            // 건물 정보를 로드
            buildingObjectInstance = Instantiate(buildingPrefab, transform).GetComponent<BuildingObject>();
            buildingObjectInstance.GetComponent<Collider>().isTrigger = true;
            buildingObjectInstance.gameObject.layer = 0;

            // 건설 표시기로 사용하도록 쉐이더 적용
            meshRenderer = buildingObjectInstance.GetComponent<MeshRenderer>();
            List<Material> newMaterials = new() { indication };
            meshRenderer.SetMaterials(newMaterials);

            // 건설 표시기의 충돌 이벤트 적용
            // 건설 가능/불가능 위치를 판단하는 로직을 추가
            var buildingIndicatorObjectInstance = buildingObjectInstance.AddComponent<BuildingIndicatorObject>();
            buildingIndicatorObjectInstance.OnTriggerEnterAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnTriggerStayAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnTriggerExitAction += other => OnApproveBulidObject(other);
        }



        /// <summary>
        /// 건설모드에서 건물이 다른 건물에 맞춰서 건설될 수 있도록합니다.
        /// </summary>
        private void OnEnableSnap(InputValue value)
        {
            isSnapEnabled = isSnapEnabled ? false : true;
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
            if ((buildingObjectInstance.Data.BuildingLayer & other.gameObject.layer) != 0)
            {
                ApproveBulidObject();
            }
        }

        /// <summary>
        /// <see cref="BuildingIndicatorObject"/>의 충돌 이벤트에서 트리거됩니다.
        /// </summary>
        private void OnRejectBuildObject(Collider other)
        {
            RejectBuildObject();
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 있도록 표시합니다.
        /// </summary>
        private void ApproveBulidObject()
        {
            status = IndicatorStatus.Approved;
            meshRenderer.material.SetColor("_Color", Color.green);
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 없도록 표시합니다.
        /// </summary>
        private void RejectBuildObject()
        {
            status = IndicatorStatus.Rejected;
            meshRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public partial class BuildingIndicator
    {
        // 건설가능 여부
        public IndicatorStatus Status => status;
    }
}