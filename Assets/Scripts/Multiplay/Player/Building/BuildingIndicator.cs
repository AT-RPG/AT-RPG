using AT_RPG.Manager;
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
        [SerializeField, Range(1, 7.5f)] private float detectRange = 7.5f;

        // 건설위치 회전속도
        [SerializeField, Range(1, 360f)] private float rotationSpeed = 90f;

        // 건설 가능 여부, 건설될 건물의 모양을 시각적으로 표현합니다.
        [SerializeField] private Material indication = null;

        // 이 레이어에만 크로스헤어를 따라서 건설 위치가 업데이트됩니다.
        [SerializeField] private LayerMask collisionLayer = 0;

        // 현재 위치에 건설할 건물의 정보
        // 건설 가능 여부, 다른 건물에 스냅을 구현하는데 사용됩니다.
        private Vector3 buildingPosition = Vector3.zero;
        private Quaternion buildingRotation = Quaternion.identity;
        private BuildingObjectData buildingData = null;
        private MeshFilter buildingMeshFilter = null;
        private MeshRenderer buildingMeshRenderer = null;
        private Bounds buildingBounds = default;

        // 건설위치를 업데이트 할 수 있는지
        private bool isMovable = false;
        private RaycastHit hit = default;

        // 현재 위치에 건물 건설이 가능한지를 알려줍니다.
        private IndicatorStatus status = IndicatorStatus.Approved;
        private bool statusDirty = true;



        private void Awake()
        {
            InputManager.AddKeyAction("RotateBuildingPositionL", RotateLeft);
            InputManager.AddKeyAction("RotateBuildingPositionR", RotateRight);

            buildingMeshFilter = GetComponent<MeshFilter>();
            buildingMeshRenderer = GetComponent<MeshRenderer>();
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

            // 현재 transform.position 위치에 건설이 가능하도록 일단 초기화
            // 추후에 OnTrigger...등의 충돌 이벤트에서 걸러집니다.
            statusDirty = true;
            status = IndicatorStatus.Approved;
            indication.SetColor("_BuildingStatusColor", Color.green);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((buildingData.BuildingLayer & (1 << other.gameObject.layer)) != 0)
            {
                return;
            }

            status = IndicatorStatus.Rejected;
            indication.SetColor("_BuildingStatusColor", Color.red);
            statusDirty = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if ((buildingData.BuildingLayer & (1 << other.gameObject.layer)) == 0)
            {
                status = IndicatorStatus.Rejected;
                indication.SetColor("_BuildingStatusColor", Color.red);
                statusDirty = false;
            }

            if (statusDirty && (buildingData.BuildingLayer & (1 << other.gameObject.layer)) != 0)
            {
                status = IndicatorStatus.Approved;
                indication.SetColor("_BuildingStatusColor", Color.green);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((buildingData.BuildingLayer & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            status = IndicatorStatus.Approved;
            indication.SetColor("_BuildingStatusColor", Color.green);
        }



        /// <summary>
        /// 건설 표시기의 건물을 재설정합니다.
        /// </summary>
        /// <remarks>
        /// <paramref name="buildingPrefab"/>는 <see cref="BuildingObject"/>를 가지고 있어야합니다.
        /// </remarks>
        public void SetBuilding(AssetReferenceResource<GameObject> buildingPrefab)
        {
            // 건물 크기 복사
            var buildingResource = buildingPrefab.Resource;
            transform.localScale = buildingResource.transform.localScale * 0.99f;

            // 건물의 쉐이더와 메쉬를 복사
            buildingData = buildingResource.GetComponent<BuildingObject>().Data;
            buildingMeshFilter.mesh = buildingResource.GetComponent<MeshFilter>().sharedMesh;
            buildingMeshRenderer.materials = new Material[] { indication };

            // 건물의 충돌 범위를 복사
            var temp = Instantiate(buildingResource);
            buildingBounds = temp.GetComponent<Collider>().bounds;
            Destroy(temp);

            // 건물 메쉬를 토대로 콜라이더를 생성
            var collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = buildingMeshFilter.mesh;
            collider.convex = true;
            collider.isTrigger = true;
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
                        transform.position = hit.point + new Vector3(0, buildingBounds.extents.y + Physics.defaultContactOffset, 0);

                        // Physics.defaultContactOffset 에 의해 건물이 서로 살짝 띄워지는거 방지
                        buildingPosition = hit.point + new Vector3(0, buildingBounds.extents.y, 0);
                        buildingRotation = transform.rotation;

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

                        // 1. hit.collider의 OBB bounds x,y,z 거리 구하기.
                        // 2. 현재 선택한 건물의 OBB bounds x, y, z 거리 구하기.
                        float approxLocalUnitDist = 0f;
                        switch (approxLocalUnitDir)
                        {
                            case Vector3 x when x.Equals(localRight) || x.Equals(localLeft):
                                approxLocalUnitDist = hit.collider.bounds.extents.x + buildingBounds.extents.x;
                                break;

                            case Vector3 y when y.Equals(localUp) || y.Equals(localDown):
                                approxLocalUnitDist = hit.collider.bounds.extents.y + buildingBounds.extents.y;
                                break;

                            case Vector3 z when z.Equals(localForward) || z.Equals(localBack):
                                approxLocalUnitDist = hit.collider.bounds.extents.z + buildingBounds.extents.z;
                                break;
                        }

                        // 크로스헤어로 충돌된 콜라이더의 위치로 현재 건물을 이동 후, hit.collider의 bound + 현재 선택한 건물의 bound를 더해서 거리를 벌린다.
                        transform.position = hit.collider.transform.position + approxLocalUnitDir * (approxLocalUnitDist + Physics.defaultContactOffset);

                        // Physics.defaultContactOffset 에 의해 건물이 서로 살짝 띄워지는거 방지
                        buildingPosition = hit.collider.transform.position + approxLocalUnitDir * approxLocalUnitDist;
                        buildingRotation = transform.rotation;

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
    }

    public partial class BuildingIndicator
    {
        // 건설가능 여부
        public IndicatorStatus Status => status;

        public Vector3 BuildingPosition => buildingPosition;

        public Quaternion BuildingRotation => buildingRotation;
    }
}