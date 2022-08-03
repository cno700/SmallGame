using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 只有拥有IDamageable接口的object才能触发击中方法
public interface IDamageable
{
    // hit用来提供一些额外的信息，比如在哪里被击中
    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection);

    void TakeDamage(float damage); // TakeHit的简化版本，因为在敌人攻击玩家时无法提供Ray
}
