using System;

namespace AT_RPG
{
    /// <summary>
    /// System.Guid를 JsonUtility로 직렬화 할 수 있게 해주는 래핑 클래스
    /// </summary>
    [Serializable]
    public struct SerializableGuid : IComparable, IComparable<SerializableGuid>, IEquatable<SerializableGuid>
    {
        public string Value;

        private SerializableGuid(string value)
        {
            Value = value;
        }

        public static bool operator ==(SerializableGuid lhs, SerializableGuid rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializableGuid lhs, SerializableGuid rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid.ToString());
        }

        public static implicit operator Guid(SerializableGuid serializableGuid)
        {
            return new Guid(serializableGuid.Value);
        }

        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (!(value is SerializableGuid))
                throw new ArgumentException("Must be SerializableGuid");
            SerializableGuid guid = (SerializableGuid)value;
            return guid.Value == Value ? 0 : 1;
        }

        public int CompareTo(SerializableGuid other)
        {
            return other.Value == Value ? 0 : 1;
        }

        public bool Equals(SerializableGuid other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return (Value != null ? new Guid(Value).ToString() : string.Empty);
        }
    }
}