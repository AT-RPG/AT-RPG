using System;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG.Manager
{
    public class InputManager : Singleton<InputManager>
    {
        // 인게임 키보드 매핑
        [SerializeField] private KeyMap keyMap
            = new KeyMap()
            {
                // 플레이어
                {KeyCode.W, new KeyValuePair<string, Action>("앞으로 이동", null)  },
                {KeyCode.S, new KeyValuePair<string, Action>("뒤로 이동", null)  },
                {KeyCode.A, new KeyValuePair<string, Action>("왼쪽으로 이동", null)  },
                {KeyCode.D, new KeyValuePair<string, Action>("오른쪽으로 이동", null)  },
                {KeyCode.C, new KeyValuePair<string, Action>("앉기", null)  },
                {KeyCode.I, new KeyValuePair<string, Action>("인벤토리", null)  },
                {KeyCode.Mouse0, new KeyValuePair<string, Action>("공격/사격", null)  },
                {KeyCode.Mouse1, new KeyValuePair<string, Action>("조준", null) },
                {KeyCode.Space, new KeyValuePair<string, Action>("점프", null)  },
                {KeyCode.Alpha1, new KeyValuePair<string, Action>("1번 무기", null)  },
                {KeyCode.Alpha2, new KeyValuePair<string, Action>("2번 무기", null)  },
                {KeyCode.Alpha3, new KeyValuePair<string, Action>("3번 무기", null)  },
                {KeyCode.Alpha4, new KeyValuePair<string, Action>("4번 무기", null)  },

                // 설정
                {KeyCode.Escape, new KeyValuePair<string, Action>("설정/뒤로 가기", null)  },
            };

        /// <summary>
        /// 키가 눌려진 액션들이 이곳에 저장됩니다. <br/>
        /// 호출되면 액션들이 모두 지워집니다.
        /// </summary>
        private Action keyActions;


        protected override void Awake()
        {
            base.Awake();

        }

        private void Update()
        {
            keyActions?.Invoke();
            keyActions = null;
        }



        public void AddKeyAction(KeyCode key, Action action)
        {
            if (!keyMap.ContainsKey(key))
            {
                Debug.LogError("지정되지않은 액션...," +
                               " InputManager의 keyMap에 기본 설정을 새로 만들어주세요.");
                return;
            }

            Action keyAction = keyMap[key].Value;
            keyAction += action;
        }

        public void EraseKeyAction(KeyCode key, Action action)
        {

        }

        /// <summary>
        /// key와 비교해서, keyMap에 중복된 키 액션이 있는지
        /// </summary>
        public bool CheckIsKeyDuplicated(KeyCode key, Action action)
        {
            return false;
        }
    }
}