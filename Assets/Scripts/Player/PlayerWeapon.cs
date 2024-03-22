using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using AT_RPG;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponState
{
    None, OneHand, TwoHand
}

public class PlayerWeapon : MonoBehaviour
{
    public UnityEvent<WeaponData> weaponChangeAct;
    [SerializeField] WeaponState myState;
    [SerializeField] WeaponData defaultWeapon = null;
    [SerializeField] WeaponData currentWeapon;

    void ChangeWeapon(WeaponState s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case WeaponState.None:
            break;
            case WeaponState.OneHand:
            case WeaponState.TwoHand:
            weaponChangeAct?.Invoke(currentWeapon);
            break;
        }
    }
    void Start()
    {
        ChangeWeapon(WeaponState.None);
        currentWeapon = defaultWeapon;
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
