using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AT_RPG.Manager.ResourceManager;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
    #region ResourceManager

    /// <summary>
    /// <see cref="AssetReference.AssetGUID"/>와 로드한 리소스를 매핑하는 클래스 <br/>
    /// Key = <see cref="AssetReference.AssetGUID"/> <br/>
    /// Value = <see cref="AssetReference.Asset"/>
    /// </summary>
    public class ResourceMap : Dictionary<string, UnityObject> { }

    public class ResourceHandleList : List<AsyncOperationHandle> { }

    /// <summary>
    /// <see cref="Manager.ResourceManager"/>에서 리소스 로드 호출 시, 로드 대기열에 전달되는 데이터
    /// </summary>
    public struct LoadRequest { }

    /// <summary>
    /// <see cref="Manager.ResourceManager"/>에서 리소스 언로드 호출 시, 언로드 대기열에 전달되는 데이터
    /// </summary>
    public struct UnloadRequest
    {
        public string SceneName { get; set; }
        public LoadStartCondition StartCondition { get; set; }
        public LoadCompleted Completed { get; set; }

        public UnloadRequest(string sceneName, LoadStartCondition startCondition, LoadCompleted completed = null)
        {
            SceneName = sceneName;
            StartCondition = startCondition;
            Completed = completed;
        }
    }

    #endregion

    #region InputManager

    // Key1 = 액션 이름,   Value1 = AT_RPG.InputMappingContext 클래스
    public class KeyActionMap : Dictionary<string, InputMappingContext> 
    {
        /// <summary>
        /// 맵에 KeyCode가 있는지 확인
        /// </summary>
        public bool ContainsKeyCode(InputKeyCode keyCode)
        {
            foreach (var keySetting in this)
            {
                if (keySetting.Value.KeyCode == keyCode)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 입력 키에 키 입력 옵션, 키에 바인딩할 구현등을 모아둔 클래스
    /// </summary>
    public class InputMappingContext
    {
        public InputKeyCode         KeyCode;    
        public InputOption          KeyOption;
        public Action<InputValue>   KeyAction;

        /// <summary>
        /// 키보드 입력 액션 생성
        /// </summary>
        public InputMappingContext(KeyCode keyboardCode, InputOption option, Action<InputValue> keyAction = null)
        {
            KeyCode = keyboardCode;
            KeyOption = option;
            KeyAction = keyAction;
        }

        /// <summary>
        /// 마우스 입력 액션 생성
        /// </summary>
        public InputMappingContext(MouseKeyCode mouseCode, InputOption option, Action<InputValue> keyAction = null)
        {
            KeyCode = mouseCode;
            KeyOption = option;
            KeyAction = keyAction;
        }

        /// <summary>
        /// 키 복사 + 입력 액션 생성
        /// </summary>
        public InputMappingContext(InputKeyCode keyCode, InputOption option, Action<InputValue> keyAction = null)
        {
            KeyCode = keyCode;
            KeyOption = option;
            KeyAction = keyAction;
        }
    }

    [Flags]
    public enum MouseKeyCode
    {
        None = 0,
        MouseY = 1,
        MouseX = 2,
    }

    public enum InputOption
    {
        GetKeyDown,               // 키를 눌렀을 때 반응
        GetKeyUp,                 // 키를 때면 반응
        GetKey,                   // 키를 누르고 있을 때 반응
        GetAxisRaw,               // 마우스 입력값이 -1 ~ 1 사이로 반응
        GetAxis,                  // 마우스 입력값이 -1, 0, 1로 반응
    }

    public struct InputKeyCode
    {
        public KeyCode      KeyboardCode { get; private set; }
        public MouseKeyCode MouseKeyCode { get; private set; }

        private InputKeyCode(KeyCode keyboardCode) : this()
        {
            KeyboardCode = keyboardCode;
            MouseKeyCode = MouseKeyCode.None;
        }

        private InputKeyCode(MouseKeyCode mouseCode) : this()
        {
            MouseKeyCode = mouseCode;
            KeyboardCode = KeyCode.None;
        }

        public static implicit operator InputKeyCode(KeyCode keyboardCode) => new InputKeyCode(keyboardCode);
        public static implicit operator InputKeyCode(MouseKeyCode mouseCode) => new InputKeyCode(mouseCode);
        public static bool operator !=(InputKeyCode lhs, InputKeyCode rhs)
        {
            return lhs.KeyboardCode != rhs.KeyboardCode ||
                   lhs.MouseKeyCode != rhs.MouseKeyCode ? true : false;
        }
        public static bool operator ==(InputKeyCode lhs, InputKeyCode rhs)
        {
            return lhs.KeyboardCode == rhs.KeyboardCode && 
                   lhs.MouseKeyCode == rhs.MouseKeyCode ? true : false;
        }
        public override bool Equals(object obj)
        {
            return obj is InputKeyCode other &&
                   EqualityComparer<KeyCode?>.Default.Equals(KeyboardCode, other.KeyboardCode) &&
                   EqualityComparer<MouseKeyCode?>.Default.Equals(MouseKeyCode, other.MouseKeyCode);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(KeyboardCode, MouseKeyCode);
        }
    }

    public struct InputValue
    {
        public object Value;
        public Type ValueType;

        private InputValue(object value, Type type) : this()
        {
            Value = value;
            ValueType = type;
        }

        public T GetValue<T>()
        {
            if (Value is T)
            {
                return (T)Value;
            }
            else
            {
                Debug.LogError($"InputValue : InputValue가 {typeof(T)}로 캐스팅 할 수 없습니다. " +
                               $"{ValueType}으로 캐스팅 해주세요.");
                return default(T);
            }
        }

        public static implicit operator InputValue(Vector2 value)
            => new InputValue(value, typeof(Vector2));
        public static implicit operator InputValue(bool value)
            => new InputValue(value, typeof(bool));
    }

    #endregion

    #region SaveLoadManager

    /// <summary>
    /// 직렬화 데이터를 포함하는 게임 오브젝트 리스트
    /// </summary>
    public class SerializedGameObjectDataList : List<List<GameObjectData>> { }
    #endregion

    #region MultiplayManager

    // 포톤 클라우드 애플리케이션 서버에 연결이 성공한 경우
    public delegate void ConnectedCallback();

    // 포톤 클라우드 애플리케이션 서버에 연결이 실패한 경우
    public delegate void DisconnectedCallback();

    #endregion
}