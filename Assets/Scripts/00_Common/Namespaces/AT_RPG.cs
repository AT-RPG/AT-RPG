using System;
using System.Collections.Generic;
using System.IO;
using Unity.Serialization.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
    #region ResourceManager

    /// <summary>
    /// '<see cref="AddressableAssetEntry.address"/>'와 런타임'<see cref="AssetReference.AssetGUID"/>'매핑을 관리하는 클래스 <br/>
    ///                                                                                                                      <br/>
    /// key1 = <see cref="AddressableAssetEntry.address"/>                                                                   <br/>
    /// value1 = <see cref="AssetReference.AssetGUID"/>
    /// </summary>
    [System.Serializable]
    public class AssetGuidMap
    {
        [SerializeField] private Dictionary<string, string> map = new();

        /// <summary>
        /// 매핑 파일을 불러옵니다.                                                  <br/>
        ///                                                                          <br/>
        /// 경로 : <see cref="ResourceManagerSettings.GetAssetGuidMapFilePath"/>     <br/>
        /// </summary>
        public static AssetGuidMap Load()
        {
            // 파일 저장 경로를 가져오기 위해 설정을 로드
            ResourceManagerSettings setting = Resources.Load<ResourceManagerSettings>($"{nameof(ResourceManagerSettings)}");
            if (!setting)
            {
                Debug.Log($"{nameof(ResourceManagerSettings)}이 {nameof(Resources)}에 없습니다.");
            }

            // Json파일의 Guid 매핑을 역직렬화
            AssetGuidMap map = new();
            using (FileStream stream = new FileStream(setting.GetAssetGuidMapFilePath(), FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                string mapFromJson = reader.ReadToEnd();
                map = JsonSerialization.FromJson<AssetGuidMap>(mapFromJson);
            }

            // 파일 저장 경로를 가져오기 위해 사용했던 설정을 언로드
            Resources.UnloadAsset(setting);

            return map;
        }

        public void Add(string key1, string value1) => map.Add(key1, value1);

        public void Remove(string key1) => map.Remove(key1);

        public string this[string key1]
        {
            get => map[key1];
            set => map[key1] = value;
        }
    }

    /// <summary>
    /// 리소스 매니저에서 현재 캐시된 어드레서블 리소스를 담아두는 클래스                      <br/>
    /// key1 = 어드레서블 에셋에 부여된 <see cref="AssetReference.AssetGUID"/>                 <br/>
    /// key2 = <see cref="Manager.ResourceManager.LoadAssetAsync"/>을 불러오는데 사용된 key    <br/>
    /// value1 = <see cref="AssetReference.Asset"/>                                            <br/>
    /// </summary>
    public class ResourceMap : Dictionary<string, KeyValuePair<string, UnityObject>> 
    {
        public void Add(string key1, string key2, UnityObject value1)
        {
            if (!ContainsKey(key1)) { Add(key1, new()); }
            this[key1] = new (key2, value1);
        }
    }

    /// <summary>
    /// <see cref="Manager.ResourceManager"/>에서 어드레서블을 로드한 리소스를 래퍼런싱하는 핸들을 담아두는 클래스                                                 <br/>
    /// Key1 = <see cref="AsyncOperationHandle"/>을 불러오는데 사용된 <see cref="AssetReference.AssetGUID"/>또는 <see cref="AssetLabelReference.labelString"/>     <br/>
    /// Key2 = Key1을 통해 리턴된 핸들                                                                                                                             <br/>
    /// </summary>
    public class ResourceHandleMap : Dictionary<string, AsyncOperationHandle> { }

    /// <summary>
    /// <see cref="Manager.ResourceManager"/>에서 리소스 로드/언로드 호출 시, 프로세스열에 전달되는 데이터 <br/>
    /// 현재 동작중인 로딩들을 관리하는데 사용됩니다.
    /// </summary>
    public struct ResourceRequest 
    {
        // 요청마다 고유 식별자를 부여합니다.
        public Guid RequestId;

        /// 키는 <see cref="AssetLabelReference.labelString"/>또는 <see cref="AssetReference.RuntimeKey"/> 입니다.
        public string Key;

        public ResourceRequest(string key)
        {
            Key = key;
            RequestId = Guid.NewGuid();
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
        public UpdateOption         UpdateOption;
        public Action<InputValue>   KeyAction;

        /// <summary>
        /// 키보드 입력 액션 생성
        /// </summary>
        public InputMappingContext(KeyCode keyboardCode, InputOption keyOption, UpdateOption updateOption = UpdateOption.Default, Action<InputValue> keyAction = null)
        {
            KeyCode = keyboardCode;
            KeyOption = keyOption;
            UpdateOption = updateOption;
            KeyAction = keyAction;
        }

        /// <summary>
        /// 마우스 입력 액션 생성
        /// </summary>
        public InputMappingContext(MouseKeyCode mouseCode, InputOption keyOption, UpdateOption updateOption = UpdateOption.Default, Action<InputValue> keyAction = null)
        {
            KeyCode = mouseCode;
            KeyOption = keyOption;
            UpdateOption = updateOption;
            KeyAction = keyAction;
        }

        /// <summary>
        /// 키 복사 + 입력 액션 생성
        /// </summary>
        public InputMappingContext(InputKeyCode keyCode, InputOption keyOption, UpdateOption updateOption = UpdateOption.Default, Action<InputValue> keyAction = null)
        {
            KeyCode = keyCode;
            KeyOption = keyOption;
            UpdateOption = updateOption;
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

    /// <summary>
    /// 키의 입력방식을 설정합니다.
    /// </summary>
    public enum InputOption
    {
        GetKeyDown,               // 키를 눌렀을 때 반응
        GetKeyUp,                 // 키를 때면 반응
        GetKey,                   // 키를 누르고 있을 때 반응
        GetAxisRaw,               // 마우스 입력값이 -1 ~ 1 사이로 반응
        GetAxis,                  // 마우스 입력값이 -1, 0, 1로 반응
    }

    /// <summary>
    /// 키 이벤트가 실질적으로 언제 실행되는지 설정합니다.
    /// </summary>
    public enum UpdateOption
    {
        // Update()
        Default,

        // FixedUpdate()
        Fixed,

        // LateUpdate()
        Late
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