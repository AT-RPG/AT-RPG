using System;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 건설 표시기가 다른 오브젝트와 충돌했을때를 정의합니다. <br/>
    /// (ex. 건물 겹침에 의해, 건설할 수 없는 경우를 구현)
    /// </summary>
    public class BuildingIndicatorCollisionHandler : MonoBehaviour
    {
        public Action<Collider> OnCollisionEnterAction;
        public Action<Collider> OnCollisionStayAction;
        public Action<Collider> OnCollisionExitAction;

        private void OnTriggerEnter(Collider other)
        {
            OnCollisionEnterAction?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnCollisionStayAction?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnCollisionExitAction?.Invoke(other);
        }
    }

}