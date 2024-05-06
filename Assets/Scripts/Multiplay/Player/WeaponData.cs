using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "WeaponDataObject", menuName = "ScriptableObject/Weapon Setting", order = int.MaxValue)]

    public class WeaponData : ScriptableObject
    {
        [SerializeField]
        private string weaponName;
        public string WeaponName { get { return weaponName; } }
        [SerializeField]
        private int damage;
        public int Damage { get { return damage; } }
        [SerializeField]
        private WeaponState myState;
        public WeaponState MyState { get { return myState; } }
        [SerializeField]
        private GameObject weaponPrefab;
        public GameObject WeaponPrefab { get { return weaponPrefab; } }
        [SerializeField]
        private RuntimeAnimatorController animatorOverride;
        public RuntimeAnimatorController AnimatorOverride { get { return animatorOverride; } }
    }
}
