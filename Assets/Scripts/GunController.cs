using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold; // ��ȡ��ɫ��ǹ��λ��
    public Gun startingGun;
    Gun equipedGun;

    private void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    // װ���µ�ǹ
    public void EquipGun(Gun gunToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipedGun.transform.parent = weaponHold; // ����ǹ����ͬ���ƶ�����ת
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
