using UnityEngine;
using UnityEngine.Events;
using AT_RPG.Manager;

namespace AT_RPG
{
    public enum WeaponState
    {
        None, OneHand, TwoHand
    }

    public class PlayerWeapon : MonoBehaviour
    {
        public UnityEvent<WeaponData> weaponChangeAct;
        [SerializeField] WeaponState myState;
        [SerializeField] WeaponData defaultWeapon;
        [SerializeField] WeaponData currentWeapon;

        private void Awake() 
        {
            InputManager.AddKeyAction("ChangeNoneWeapon", ChangeNoneWeapon);
        }

        private void ChangeWeapon(WeaponState s)
        {
            if (myState == s) return;
            myState = s;
            switch (myState)
            {
                case WeaponState.None:
                case WeaponState.OneHand:
                case WeaponState.TwoHand:
                weaponChangeAct?.Invoke(currentWeapon);
                break;
            }
        }
        private void Start()
        {
            currentWeapon = defaultWeapon;
            ChangeWeapon(currentWeapon.MyState);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Weapon newWeapon = other.GetComponent<Weapon>();
                if(newWeapon != null)
                {
                    currentWeapon = newWeapon.weaponData;
                    ChangeWeapon(currentWeapon.MyState);
                }
            }
        }

        private void ChangeNoneWeapon(InputValue v)
        {
            currentWeapon = defaultWeapon;
            ChangeWeapon(currentWeapon.MyState);
        }

        private void OnDestroy() 
        {
            InputManager.RemoveKeyAction("ChangeNoneWeapon", ChangeNoneWeapon);
        }
    }
}

