using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 건설 기능(<see cref="PlayerBuilding"/>)에서 사용되는 건설 오브젝트의 정보를 저장하는 클래스
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Animator))]
    public partial class BuildingObject : MonoBehaviour, ICharacterDamage
    {
        [SerializeField] private BuildingObjectData data;

        [Tooltip("'Destroy.anim', 'Creation.anim'에서 사용되는 'Building_Dissolve.shader'값, 인스턴스가 생성되면, Dissolve효과를 실행합니다.")]
        [Range(0.0f, 1.0f)] public float DissolveThreshold;

        [Tooltip("현재 인스턴스에 추가할 BuildingObject의 Dissolve 쉐이더")]
        [SerializeField] private Material dissolvePrefab;

        private Material dissolveInstance = null;

        private Animator animator = null;

        private float hp;


        private void Awake()
        {
            hp = data.MaxHP;

            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AddDissolve();
            animator.SetTrigger("Create");
        }

        private void Update()
        {
            dissolveInstance.SetFloat("_DissolveThreshold", DissolveThreshold);
            Debug.Log(dissolveInstance.GetFloat("_DissolveThreshold"));
        }

        private void OnDestroy()
        {
            RemoveDissolve();
        }

        private void AddDissolve()
        {
            // 현재 인스턴스 쉐이더 리스트 가져오기
            MeshRenderer renderer = animator.gameObject.GetComponent<MeshRenderer>();
            List<Material> materials = renderer.materials.ToList();

            // Dissolve 쉐이더 부착
            materials.Add(dissolvePrefab);

            // 업데이트
            renderer.SetMaterials(materials);
            dissolveInstance = materials[materials.Count - 1];
        }

        private void RemoveDissolve()
        {
            // 현재 인스턴스 쉐이더 리스트 가져오기
            MeshRenderer renderer = animator.gameObject.GetComponent<MeshRenderer>();
            List<Material> materials = renderer.materials.ToList();

            // Dissolve 찾기
            Material dissolve = null;
            foreach (var material in materials)
            {
                if (this.dissolvePrefab.name.Contains(material.name))
                {
                    dissolve = material;
                    break;
                }
            }

            // Dissolve 쉐이더 제거
            materials.Remove(dissolve);

            // 업데이트
            renderer.SetMaterials(materials);
            dissolveInstance = null;
        }

        public void TakeDamage(float dmg)
        {
            hp -= dmg;

            animator.SetTrigger("Hit");

            if (hp <= 0f) { animator.SetTrigger("Destroy"); }
        }


        /// <summary>
        /// <see cref="Animator.SetTrigger(string)"/>가 'Destroy'될 경우, 애니메이션 이벤트에 의해 트리거됩니다.
        /// </summary>
        private void OnKill()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// <see cref="Animator.SetTrigger(string)"/>가 'Hit'될 경우, 애니메이션 이벤트에 의해 트리거됩니다.
        /// </summary>
        private void OnShake()
        {
            transform.DOShakePosition(0.75f);
        }
    }

    public partial class BuildingObject
    {
        public BuildingObjectData Data => data;
    }
}