using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // hit�����ṩһЩ�������Ϣ�����������ﱻ����
    void TakeHit(float damage, RaycastHit hit);
}
