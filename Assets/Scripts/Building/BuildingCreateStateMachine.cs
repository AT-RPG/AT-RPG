using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AT_RPG
{
    public class BuildingCreateStateMachine : StateMachineBehaviour
    {
        /// <summary>
        /// 'Destroy.anim', 'Creation.anim'에서 사용되는 'Building_Dissolve.shader'값
        /// 인스턴스가 생성되면, Dissolve효과를 실행합니다.
        /// </summary>
        [Range(0.0f, 1.0f)] public float DissolveThreshold;
        [Range(0.0f, 0.1f)] public float DissolveEdgeWidth;

        [Tooltip("현재 인스턴스에 추가할 BuildingObject의 Dissolve 쉐이더")]
        [SerializeField] public Material Dissolve;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Dissolve 쉐이더 부착
            MeshRenderer renderer = animator.gameObject.GetComponent<MeshRenderer>();
            List<Material> materials = renderer.materials.ToList();
            materials.Add(Dissolve);
            materials.AddRange(materials);
        }
    }

}