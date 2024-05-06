using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class TestMon : CommonBattle
    {
        // Start is called before the first frame update
        void Start()
        {
            base.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                Destroy(gameObject);
            }
        }
        private void OnDestroy() 
        {
            DropItemPoolManager.Instance.Get(DropType.Monster, transform);
        }
    }
}

