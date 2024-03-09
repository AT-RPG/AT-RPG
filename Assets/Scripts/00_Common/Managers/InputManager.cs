using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace AT_RPG.Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        // 인게임 키보드 매핑에 키 값을 설정하기 위해 사용됩니다.
        [SerializeField] private KeyActionMap keyActionsMap = new KeyActionMap()
        {
            {MoveForward, new InputMappingContext(KeyCode.W, InputOption.GetKey)},
            {MoveBackward, new InputMappingContext(KeyCode.S, InputOption.GetKey)},
            {MoveLeft, new InputMappingContext(KeyCode.A, InputOption.GetKey)},
            {MoveRight, new InputMappingContext(KeyCode.D, InputOption.GetKey)},
            {Crouch, new InputMappingContext(KeyCode.LeftControl, InputOption.GetKeyDown)},
            {Inventory, new InputMappingContext(KeyCode.I, InputOption.GetKeyDown)},
            {AttackFire, new InputMappingContext(KeyCode.Mouse0, InputOption.GetKey)},
            {Aim, new InputMappingContext(MouseKeyCode.MouseX | MouseKeyCode.MouseY, InputOption.GetAxis)},
            {Jump, new InputMappingContext(KeyCode.Space, InputOption.GetKeyDown)},
            {Equipment1, new InputMappingContext(KeyCode.Alpha1, InputOption.GetKeyDown)},
            {Equipment2, new InputMappingContext(KeyCode.Alpha2, InputOption.GetKeyDown)},
            {Equipment3, new InputMappingContext(KeyCode.Alpha3, InputOption.GetKeyDown)},
            {Equipment4, new InputMappingContext(KeyCode.Alpha4, InputOption.GetKeyDown)},
            {SettingUndo, new InputMappingContext(KeyCode.Escape, InputOption.GetKeyDown)}
        };



        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            InvokeKeyActions();
        }



        /// <summary>
        /// 해당 키 이름에 새로운 액션을 바인딩합니다.
        /// </summary>
        public void AddKeyAction(string keyName, Action<InputValue> actionToAdd)
        {
            if (!IsKeyRegistered(keyName))
            {         
                return;
            }

            // 변경사항 적용
            Action<InputValue> newKeyAction = keyActionsMap[keyName].KeyAction;
            newKeyAction += actionToAdd;
            keyActionsMap[keyName].KeyAction = newKeyAction;
        }

        /// <summary>
        /// 해당 키 이름에 바인딩된 액션을 삭제합니다.
        /// </summary>
        public void RemoveKeyAction(string keyName, Action<InputValue> actionToErase)
        {
            if (!IsKeyRegistered(keyName))
            {
                return;
            }

            // 변경사항 적용
            Action<InputValue> newKeyAction = keyActionsMap[keyName].KeyAction;
            newKeyAction -= actionToErase;
            keyActionsMap[keyName].KeyAction = newKeyAction;
        }

        /// <summary>
        /// 해당 키 이름에 바인딩된 액션을 교체합니다.
        /// </summary>
        public void ChangeKeyAction(string keyName, Action<InputValue> actionToChange)
        {
            if (!IsKeyRegistered(keyName))
            {
                return;
            }

            // 변경사항 적용
            keyActionsMap[keyName].KeyAction = actionToChange;
        }

        /// <summary>
        /// 키 이름에 바인딩 된 액션들을 실행합니다.
        /// </summary>
        private void InvokeKeyActions()
        {
            foreach (var keyAction in keyActionsMap)
            {
                InputMappingContext inputMappingContext = keyAction.Value;

                // 키보드 키가 매핑되어있는지?
                if (inputMappingContext.KeyCode.KeyboardCode != KeyCode.None)
                {
                    InvokeKeyboardActions(inputMappingContext);
                }
                else
                // 마우스 키가 매핑되어있는지?
                if (inputMappingContext.KeyCode.MouseKeyCode != MouseKeyCode.None)
                {
                    InvokeMouseKeyActions(inputMappingContext);
                }
            }
        }

        /// <summary>
        /// 키보드 키 액션을 실행합니다.
        /// </summary>
        private void InvokeKeyboardActions(InputMappingContext inputMappingContext)
        {
            KeyCode keyboardCode = inputMappingContext.KeyCode.KeyboardCode.Value;
            InputOption keyInputOption = inputMappingContext.KeyOption;

            // 키 매핑에 등록된 키가 눌렸는지?
            bool isActionsTriggered = false;
            if ((keyInputOption & InputOption.GetKeyDown) != 0 && Input.GetKeyDown(keyboardCode))
            {
                isActionsTriggered = true;
            }
            else
            if ((keyInputOption & InputOption.GetKeyUp) != 0 && Input.GetKeyUp(keyboardCode))
            {
                isActionsTriggered = true;
            }
            else
            if ((keyInputOption & InputOption.GetKey) != 0 && Input.GetKey(keyboardCode))
            {
                isActionsTriggered = true;
            }

            // 키보드 키에 바인딩된 액션 실행
            if (isActionsTriggered)
            {
                inputMappingContext.KeyAction?.Invoke(true);
            }
        }

        /// <summary>
        /// 마우스 키 액션을 실행합니다.
        /// </summary>
        private void InvokeMouseKeyActions(InputMappingContext inputMappingContext)
        {
            MouseKeyCode mouseKeyCode = inputMappingContext.KeyCode.MouseKeyCode.Value;
            InputOption keyInputOption = inputMappingContext.KeyOption;

            // 마우스 입력 값 구하기 (설정값에 따라서)
            float horizontalInputValue = 0f;
            float verticalInputValue = 0f;
            if ((keyInputOption & InputOption.GetAxisRaw) != 0)
            {
                horizontalInputValue = GetFloatRaw(Input.GetAxisRaw("Mouse X"));
                verticalInputValue = GetFloatRaw(Input.GetAxisRaw("Mouse Y"));
            }
            else
            if ((keyInputOption & InputOption.GetAxis) != 0)
            {
                horizontalInputValue = Input.GetAxis("Mouse X");
                verticalInputValue = Input.GetAxis("Mouse Y");
            }
            else
            {
                Debug.LogWarning($"마우스 키의 InputOption은 GetMouseUpdateRaw나 GetMouseUpdate로 해주세요");
                return;
            }

            // 마우스가 움직였는지?
            bool isActionsTriggered = false;
            if (Math.Abs(horizontalInputValue) > 0 || Math.Abs(verticalInputValue) > 0)
            {
                isActionsTriggered = true;
            }

            // 마우스 키에 바인딩된 액션 실행
            if (isActionsTriggered)
            {
                inputMappingContext.KeyAction?.Invoke(new Vector2(
                   (mouseKeyCode & MouseKeyCode.MouseX) != 0 ? horizontalInputValue : 0f,
                   (mouseKeyCode & MouseKeyCode.MouseY) != 0 ? verticalInputValue : 0f));
            }
        }

        /// <summary>
        /// 값을 -1, 0, 1로 치환합니다.
        /// </summary>
        private float GetFloatRaw(float value)
        {
            if (value < 0)
            {
                return -1f;
            }
            else if (value > 0)
            {
                return 1f;
            }
            else
            {
                return 0f;
            }
        }

        /// <summary>
        /// 키 설정에서 키를 변경합니다.
        /// </summary>
        public void EditKeyMap(string keyName, InputMappingContext inputMappingContext)
        {
            if (!IsKeyRegistered(keyName))
            {
                return;
            }

            keyActionsMap[keyName] = inputMappingContext;
        }

        /// <summary>
        /// 키 설정에서 마우스 키가 중복되었는지 확인합니다.
        /// </summary>
        /// <param name="keyCode">중복 확인할 키</param>
        /// <returns>키가 중복되었는지?</returns>
        private bool IsKeyDuplicated(InputKeyCode keyCode)
        {
            int keyMatchedCount = 0;
            foreach (var keySetting in keyActionsMap)
            {
                if (keySetting.Value.KeyCode == keyCode)
                {
                    keyMatchedCount++;
                }
            }

            return keyMatchedCount >= 2 ? true : false;
        }

        /// <summary>
        /// 키 설정에서 키보드 키가 등록되었는지 확인합니다.
        /// </summary>
        /// <param name="keyCode">등록 확인할 키</param>
        /// <returns>키가 등록되었는지?</returns>
        private bool IsKeyRegistered(InputKeyCode keyCode)
        {
            if (!keyActionsMap.ContainsKeyCode(keyCode))
            {
                Debug.LogError($"지정되지않은 키...," +
                               $" {keyCode}는 {keyActionsMap}에 등록되지 않았습니다.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 키 설정에서 키 이름이 등록되었는지 확인합니다.
        /// </summary>
        /// <param name="keyName">등록 확인할 키</param>
        /// <returns>키가 등록되었는지?</returns>
        private bool IsKeyRegistered(string keyName)
        {
            if (!keyActionsMap.ContainsKey(keyName))
            {
                Debug.LogError($"지정되지않은 키...," +
                               $" {keyName}은 {keyActionsMap}에 등록되지 않았습니다.");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// 키에 매핑할 액션의 이름 정보
    /// </summary>
    public partial class InputManager
    {
        [SerializeField] public static readonly string MoveForward = "Move Forward";
        [SerializeField] public static readonly string MoveBackward = "Move Backward";
        [SerializeField] public static readonly string MoveLeft = "Move Left";
        [SerializeField] public static readonly string MoveRight = "Move Right";
        [SerializeField] public static readonly string Crouch = "Crouch";
        [SerializeField] public static readonly string Inventory = "Inventory";
        [SerializeField] public static readonly string AttackFire = "Attack/Fire";
        [SerializeField] public static readonly string Aim = "Aim";
        [SerializeField] public static readonly string Jump = "Jump";
        [SerializeField] public static readonly string Equipment1 = "Equipment1";
        [SerializeField] public static readonly string Equipment2 = "Equipment2";
        [SerializeField] public static readonly string Equipment3 = "Equipment3";
        [SerializeField] public static readonly string Equipment4 = "Equipment4";
        [SerializeField] public static readonly string SettingUndo = "Setting/Undo";
    }
}