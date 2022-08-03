using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold; // 获取角色持枪的位置
    public Gun startingGun;
    Gun equipedGun;

    private void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    // 装备新的枪
    public void EquipGun(Gun gunToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipedGun.transform.parent = weaponHold; // 保持枪和手同步移动和旋转
    }

    public void OnTriggerHold()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerRelease();
        }
    }
}
