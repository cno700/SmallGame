using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // hit�����ṩһЩ�������Ϣ�����������ﱻ����
    void TakeHit(float damage, RaycastHit hit);

    void TakeDamage(float damage); // TakeHit�ļ򻯰汾����Ϊ�ڵ��˹������ʱ�޷��ṩRay
}
