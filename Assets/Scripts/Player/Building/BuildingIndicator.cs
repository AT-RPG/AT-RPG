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
    public partial class BuildingIndicator : MonoBehaviour
    {
        // 건설 사정거리
        [SerializeField, Range(1, 7.5f)] private float detectRange = 7.5f;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed = 90f;

        // 건설 가능 여부, 건설될 건물의 모양을 시각적으로 표현합니다.
        [SerializeField] private Material indication = null;

        // 이 레이어에만 크로스헤어를 따라서 건설 위치가 업데이트됩니다.
        [SerializeField] private LayerMask collisionLayer = 0;

        // 현재 위치에 건물 건설이 가능한지를 알려줍니다.
        private IndicatorStatus status = IndicatorStatus.Approved;

        // 현재 위치에 건물 청사진을 보여주는 인스턴스
        private BuildingObject indicatorBuildingInstance = null;

        // 건설할 건물의 충돌정보, 스냅을 구현하는데 사용됩니다.
        private Collider indicatorBuildingCollider = null;
        private Bounds indicatorInitBuildingBounds = default;

        // 건설위치를 업데이트 할 수 있는지
        private bool isMovable = false;
        private RaycastHit hit = default;
        


        private void Awake()
        {
            InputManager.AddKeyAction("RotateBuildingPositionL", RotateLeft);
            InputManager.AddKeyAction("RotateBuildingPositionR", RotateRight);
        }

        private void OnDestroy()
        {
            InputManager.RemoveKeyAction("RotateBuildingPositionL", RotateLeft);
            InputManager.RemoveKeyAction("RotateBuildingPositionR", RotateRight);
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
            indicatorBuildingCollider = indicatorBuildingInstance.GetComponent<Collider>();
            indicatorInitBuildingBounds = indicatorBuildingCollider.bounds;

            // 건설 표시기로 사용하도록 쉐이더 적용
            var meshRenderer = indicatorBuildingInstance.GetComponent<MeshRenderer>();
            List<Material> newMaterials = new() { indication };
            meshRenderer.SetMaterials(newMaterials);

            // 건설 표시기의 충돌 이벤트 적용
            // 건설 가능/불가능 위치를 판단하는 로직을 추가
            var buildingIndicatorObjectInstance = indicatorBuildingInstance.gameObject.AddComponent<BuildingIndicatorCollisionHandler>();
            buildingIndicatorObjectInstance.OnCollisionStayAction += other => OnSnap(other);
            buildingIndicatorObjectInstance.OnCollisionStayAction += other => OnApproveBulidObject(other);
            buildingIndicatorObjectInstance.OnCollisionStayAction += other => OnRejectBuildObject(other);
        }

        /// <summary>
        /// 건설위치를 조준점(<see cref="Camera.ScreenPointToRay(Vector3)"/>)이 가리키는 위치로 업데이트합니다.
        /// </summary>
        private void Move()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
            isMovable = Physics.Raycast(ray, out hit, detectRange, collisionLayer);
            if (isMovable) 
            {
                transform.position = hit.point;
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
        /// <paramref name="other"/>를 기준으로 <see cref="indicatorBuildingInstance"/>의 충돌 범위를 스냅합니다.
        /// </summary>
        private void OnSnap(Collider other)
        {
            if (!isMovable)
            {
                return;
            }

            switch (other.gameObject.layer)
            {
                // 단순히 땅의 법선벡터쪽으로 밀어냅니다.
                case int ground when ground.Equals(LayerMask.NameToLayer("Ground")):
                    transform.position = hit.point + new Vector3(0, indicatorInitBuildingBounds.extents.y, 0);
                    break;

                // 충돌한 방향에 맞춰 정렬합니다.
                case int building when building.Equals(LayerMask.NameToLayer("Building")):
                    Vector3 approxUnitDir = MathfEx.CalculateApproxUnitVector(indicatorBuildingCollider.bounds.center - other.bounds.center, other.transform);
                    Vector3 otherCenter = other.bounds.center;
                    Vector3 direction = Vector3.zero;
                    float distance = 0f;
                    switch (approxUnitDir)
                    {
                        case Vector3 up when up.Equals(other.transform.up):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + other.transform.up * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(other.transform.up, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }

                            break;

                        case Vector3 down when down.Equals(-other.transform.up):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + (-other.transform.up) * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(-other.transform.up, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }
                            break;

                        case Vector3 right when right.Equals(other.transform.right):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + other.transform.right * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(other.transform.right, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }

                            break;

                        case Vector3 left when left.Equals(-other.transform.right):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + (-other.transform.right) * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(-other.transform.right, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }
                            break;

                        case Vector3 forward when forward.Equals(other.transform.forward):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + (other.transform.forward) * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(other.transform.forward, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }
                            break;

                        case Vector3 back when back.Equals(-other.transform.forward):
                            transform.rotation = other.transform.rotation;
                            transform.position = other.transform.position + (-other.transform.forward) * Physics.defaultContactOffset;
                            Physics.SyncTransforms();
                            if (MathfEx.CalculatePenetration(-other.transform.forward, indicatorBuildingCollider, other, out direction, out distance))
                            {
                                transform.position = otherCenter + direction * distance;
                            }
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 있도록 표시합니다.
        /// </summary>
        private void OnApproveBulidObject(Collider other)
        {
            if ((indicatorBuildingInstance.Data.BuildingLayer & (1 <<  other.gameObject.layer)) == 0) 
            { 
                return; 
            }

            status = IndicatorStatus.Approved;
            indication.SetColor("_BuildingStatusColor", Color.green);
        }

        /// <summary>
        /// 플레이어가 해당 위치에 건설을 할 수 없도록 표시합니다.
        /// </summary>
        private void OnRejectBuildObject(Collider other)
        {
            if ((indicatorBuildingInstance.Data.BuildingLayer & (1 << other.gameObject.layer)) != 0) 
            { 
                return; 
            }

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