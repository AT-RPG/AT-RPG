using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class TestMon : CommonBattle
    {
        private void OnDestroy() 
        {
            DropItemPoolManager.Instance.Get(DropType.Monster, transform);
        }
    }
}

