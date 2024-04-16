using System;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 건설 표시기가 다른 오브젝트와 충돌했을때를 정의합니다. <br/>
    /// (ex. 건물 겹침에 의해, 건설할 수 없는 경우를 구현)
    /// </summary>
    public class BuildingIndicatorObject : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterAction;
        public Action<Collider> OnTriggerStayAction;
        public Action<Collider> OnTriggerExitAction;



        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayAction?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction?.Invoke(other);
        }
    }

}