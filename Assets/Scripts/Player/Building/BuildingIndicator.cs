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
        [SerializeField] private Material outline = null;

        // 이 레이어에만 크로스헤어를 따라서 건설 위치가 업데이트됩니다.
        [SerializeField] private LayerMask collisionLayer = 0;

        // 현재 위치에 건물 건설이 가능한지를 알려줍니다.
        private IndicatorStatus status = IndicatorStatus.Approved;

        // 현재 위치에 건물 청사진을 보여주는 인스턴스
        // 건설할 건물의 충돌정보, 스냅을 구현하는데 사용됩니다.
        private BuildingObject indicatorBuildingInstance = null;
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
            List<Material> newMaterials = new() { indication, /*outline*/ };
            meshRenderer.SetMaterials(newMaterials);

            // 건설 표시기의 충돌 이벤트 적용
            // 건설 가능/불가능 위치를 판단하는 로직을 추가
            var buildingIndicatorObjectInstance = indicatorBuildingInstance.gameObject.AddComponent<BuildingIndicatorCollisionHandler>();
            buildingIndicatorObjectInstance.OnCollisionEnterAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnCollisionStayAction += other => OnApproveBulidObject(other);
            buildingIndicatorObjectInstance.OnCollisionStayAction += other => OnRejectBuildObject(other);
            buildingIndicatorObjectInstance.OnCollisionExitAction += other => OnApproveBulidObject(other);
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
                switch (hit.collider.gameObject.layer)
                {
                    case int ground when ground.Equals(LayerMask.NameToLayer("Ground")):
                        transform.position = hit.point + new Vector3(0, indicatorInitBuildingBounds.extents.y, 0);
                        break;

                    case int building when building.Equals(LayerMask.NameToLayer("Building")):
                        transform.rotation = hit.transform.rotation;

                        // 거리를 벌려야할 방향과 거리
                        Vector3 approxLocalUnitDir = MathfEx.CalculateApproxUnitVector(hit.point - hit.collider.bounds.center, hit.collider.transform);

                        // hit.collider의 local unit direciton을 저장합니다.
                        Vector3 localUp = hit.collider.transform.up;
                        Vector3 localDown = -hit.collider.transform.up;
                        Vector3 localRight = hit.collider.transform.right;
                        Vector3 localLeft = -hit.collider.transform.right;
                        Vector3 localForward = hit.collider.transform.forward;
                        Vector3 localBack = -hit.collider.transform.forward;

                        // OBB bounds의 x,y,z를 구하기 위해 잠시 회전시킨다.
                        Quaternion temp = hit.collider.transform.rotation;
                        hit.collider.transform.rotation = Quaternion.identity;
                        Physics.SyncTransforms();

                        // OBB bounds의 x,y,z 거리 구하기.
                        float approxLocalUnitDist = 0f;
                        switch (approxLocalUnitDir)
                        {
                            case Vector3 x when x.Equals(localRight) || x.Equals(localLeft):
                                approxLocalUnitDist = hit.collider.bounds.extents.x + indicatorInitBuildingBounds.extents.x;
                                break;

                            case Vector3 y when y.Equals(localUp) || y.Equals(localDown):
                                approxLocalUnitDist = hit.collider.bounds.extents.y + indicatorInitBuildingBounds.extents.y;
                                break;

                            case Vector3 z when z.Equals(localForward) || z.Equals(localBack):
                                approxLocalUnitDist = hit.collider.bounds.extents.z + indicatorInitBuildingBounds.extents.z;
                                break;
                        }

                        // 해당 방향의 중점으로 이동
                        transform.position = hit.collider.transform.position + approxLocalUnitDir * (approxLocalUnitDist + Physics.defaultContactOffset);

                        // OBB를 구하기 위해 회전했던걸 다시 되돌림
                        hit.collider.transform.rotation = temp;
                        Physics.SyncTransforms();

                        break;
                }
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