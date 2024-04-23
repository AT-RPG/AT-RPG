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

        [SerializeField] private Material dissolvePrefab;

        private Material dissolveInstance;

        private Animator animator = null;

        private float hp;



        private void Awake()
        {
            hp = data.MaxHP;

            // Dissolve 쉐이더 부착
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            List<Material> materials = renderer.materials.ToList();
            materials.Add(dissolvePrefab);
            renderer.SetMaterials(materials);
            dissolveInstance = renderer.materials.Last();

            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator.SetTrigger("Create");
        }

        public void TakeDamage(float dmg)
        {
            hp -= dmg;

            animator.SetTrigger("Hit");

            if (hp <= 0f) { animator.SetTrigger("Destroy"); }
        }

        /// <summary>
        /// <see cref="Animator.SetTrigger(string)"/>가 'Destroy'될 경우, 애니메이션 이벤트에 의해 트리거됩니다. <br/>
        /// 인스턴스가 파괴될 때 호출됩니다.
        /// </summary>
        private void OnKill()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// <see cref="Animator.SetTrigger(string)"/>가 'Hit'될 경우, 애니메이션 이벤트에 의해 트리거됩니다. <br/>
        /// <see cref="TakeDamage(float)"/>될 때 호출됩니다.
        /// </summary>
        private void OnShake()
        {
            transform.DOShakePosition(0.5f, 0.2f, 15);
        }

        /// <summary>
        /// <see cref="Animator.SetTrigger(string)"/>가 'Create'될 경우, 애니메이션 이벤트에 의해 트리거됩니다. <br/>
        /// 인스턴싱될 때 호출됩니다.
        /// </summary>
        private void OnCreate(float duration)
        {
            StartCoroutine(OnCreateImpl(duration));
        }

        /// <summary>
        /// <see cref="OnCreate"/>의 구현부 <br/>
        /// Dissolve효과를 적용합니다.
        /// </summary>
        private IEnumerator OnCreateImpl(float duration)
        {
            float time = 0f;
            while (time <= duration)
            {
                time += Time.deltaTime;

                dissolveInstance.SetFloat("_DissolveThreshold", Mathf.Lerp(1f, 0f, time / duration));
                Debug.Log($"{time}, {Mathf.Lerp(1f, 0f, time / duration)}");

                yield return null;
            }
        }
    }

    public partial class BuildingObject
    {
        public BuildingObjectData Data => data;
    }
}