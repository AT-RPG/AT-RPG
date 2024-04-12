using AT_RPG.Manager;
using System.Collections.Generic;
using TMPro;
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
        // 건설 사정거리
        [SerializeField, Range(1, 7.5f)] private float detectRange;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed;

        // 이 레이어에 건설 표시기가 충돌되어, 충돌체 위에 건설위치가 표시됩니다.
        [SerializeField] private List<LayerMask> collisionLayer = new();

        // 건설모드에서 스냅 활성/비활성
        private bool isSnapEnabled = false;

        // 건설 시, 건물의 모양 표시에 사용
        private MeshFilter filter = null;

        // 건설 시, 건물의 위치 표시에 사용
        private MeshRenderer meshRenderer = null;



        private void Awake()
        {
            filter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

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
            ApproveBulidObject();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 건물이 발판인가? 땅에 심을 수 있음
            if (other.gameObject.layer == LayerMask.GetMask("Ground"))
            {
                if ((Building.Data.Attribute & BuildingAttribute.Platform) == 0)
                {
                    RejectBuildObject();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // 건물이 발판인가? 땅에 심을 수 있음
            if (other.gameObject.layer == LayerMask.GetMask("Ground"))
            {
                if ((Building.Data.Attribute & BuildingAttribute.Platform) == 0)
                {
                    RejectBuildObject();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ApproveBulidObject();
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
            LayerMask targetLayer = 0;
            collisionLayer.ForEach(layer => targetLayer |= layer.value);

            if (!Physics.Raycast(ray, out hit, detectRange, targetLayer)) { return; }

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
        /// 플레이어가 해당 위치에 건설을 할 수 있도록 표시합니다.
        /// </summary>
        private void ApproveBulidObject()
        {
            Status = IndicatorStatus.Approved;
            meshRenderer.material.SetColor("_Color", Color.green);
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 없도록 표시합니다.
        /// </summary>
        private void RejectBuildObject()
        {
            Status = IndicatorStatus.Rejected;
            meshRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public partial class BuildingIndicator
    {
        // 건설가능 여부
        public IndicatorStatus Status { get; set; }
        
        // 건설을 할 건물
        public BuildingObject Building { get; set; }
    }
}