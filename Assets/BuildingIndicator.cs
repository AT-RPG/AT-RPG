using AT_RPG.Manager;
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
        // 건설 사정거리
        [SerializeField, Range(1, 7.5f)] private float detectRange;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed;

        // 건설위치 충돌 레이어
        [SerializeField] private List<LayerMask> collisionLayer = new();

        // 건설가능 여부
        private IndicatorStatus status = IndicatorStatus.Approved;

        // 건설모드에서 스냅 활성/비활성
        private bool isSnapEnabled = false;

        private MeshFilter filter = null;

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
            RejectBuildObject();
        }

        private void OnTriggerStay(Collider other)
        {
            RejectBuildObject();
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

            if (Physics.Raycast(ray, out hit, detectRange, targetLayer))
            {
                transform.position = hit.point;
            }
            // 건설 사정거리가 벗어나면, 최대 사정거리로 업데이트합니다.
            else
            {

            }
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
        public IndicatorStatus Status => status;
    }
}