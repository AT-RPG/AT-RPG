using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 멀티플레이와 관련된 모든 기능을 관리합니다.
    /// </summary>
    public partial class MultiplayManager : Singleton<MultiplayManager>
    {
        private static MultiplayManagerSetting setting;

        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<MultiplayManagerSetting>("MultiplayManagerSettings");
        }
    }

    public partial class MultiplayManager
    {
        public static MultiplayManagerSetting Setting => setting;
    }
}
