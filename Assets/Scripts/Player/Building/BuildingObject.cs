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

        private Animator animator;

        private float hp;



        private void Awake()
        {
            hp = data.MaxHP;

            animator = GetComponent<Animator>();
        }



        public void TakeDamage(float dmg)
        {
            hp -= dmg;

            if (hp < 0f) { animator.SetTrigger("Destroy"); }
        }
    }

    public partial class BuildingObject
    {
        public BuildingObjectData Data => data;
    }
}