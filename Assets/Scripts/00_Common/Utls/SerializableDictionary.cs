using System.Collections.Generic;
using System;
using UnityEngine;

namespace AT_RPG
{
    // UnityEngine.CoreRP에서 가져옴
    // https://docs.unity3d.com/Packages/com.unity.render-pipelines.core@14.0/api/UnityEngine.Rendering.SerializableDictionary-2.html

    /// <summary>
    /// Unity can't serialize Dictionary so here's a custom wrapper that does. Note that you have to
    /// extend it before it can be serialized as Unity won't serialized generic-based types either.
    /// </summary>
    /// <typeparam name="K">The key KeyType;</typeparam>
    /// <typeparam name="V">The value</typeparam>
    /// <example>
    /// public sealed class MyDictionary : SerializedDictionary&lt;KeyType, ValueType&gt; {}
    /// </example>
    [Serializable]
    public class SerializableDictionary<K, V> : SerializableDictionary<K, V, K, V>
    {
        /// <summary>
        /// Conversion to serialize a key
        /// </summary>
        /// <param name="key">The key to serialize</param>
        /// <returns>The KeyboardCode that has been serialized</returns>
        public override K SerializeKey(K key) => key;

        /// <summary>
        /// Conversion to serialize a value
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The value</returns>
        public override V SerializeValue(V val) => val;

        /// <summary>
        /// Conversion to serialize a key
        /// </summary>
        /// <param name="key">The key to serialize</param>
        /// <returns>The KeyboardCode that has been serialized</returns>
        public override K DeserializeKey(K key) => key;

        /// <summary>
        /// Conversion to serialize a value
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The value</returns>
        public override V DeserializeValue(V val) => val;
    }

    /// <summary>
    /// Dictionary that can serialize keys and values as other types
    /// </summary>
    /// <typeparam name="K">The key KeyType;</typeparam>
    /// <typeparam name="V">The value KeyType;</typeparam>
    /// <typeparam name="SK">The KeyType; which the key will be serialized for</typeparam>
    /// <typeparam name="SV">The KeyType; which the value will be serialized for</typeparam>
    [Serializable]
    public abstract class SerializableDictionary<K, V, SK, SV> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<SK> m_Keys = new List<SK>();

        [SerializeField]
        List<SV> m_Values = new List<SV>();

        /// <summary>
        /// From <see cref="K"/> to <see cref="SK"/>
        /// </summary>
        /// <param name="key">They key in <see cref="K"/></param>
        /// <returns>The key in <see cref="SK"/></returns>
        public abstract SK SerializeKey(K key);

        /// <summary>
        /// From <see cref="V"/> to <see cref="SV"/>
        /// </summary>
        /// <param name="value">The value in <see cref="V"/></param>
        /// <returns>The value in <see cref="SV"/></returns>
        public abstract SV SerializeValue(V value);


        /// <summary>
        /// From <see cref="SK"/> to <see cref="K"/>
        /// </summary>
        /// <param name="serializedKey">They key in <see cref="SK"/></param>
        /// <returns>The key in <see cref="K"/></returns>
        public abstract K DeserializeKey(SK serializedKey);

        /// <summary>
        /// From <see cref="SV"/> to <see cref="V"/>
        /// </summary>
        /// <param name="serializedValue">The value in <see cref="SV"/></param>
        /// <returns>The value in <see cref="V"/></returns>
        public abstract V DeserializeValue(SV serializedValue);

        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var kvp in this)
            {
                m_Keys.Add(SerializeKey(kvp.Key));
                m_Values.Add(SerializeValue(kvp.Value));
            }
        }

        /// <summary>
        /// OnAfterDeserialize implementation.
        /// </summary>
        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Count; i++)
                Add(DeserializeKey(m_Keys[i]), DeserializeValue(m_Values[i]));

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}
