using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // hit用来提供一些额外的信息，比如在哪里被击中
    void TakeHit(float damage, RaycastHit hit);

    void TakeDamage(float damage); // TakeHit的简化版本，因为在敌人攻击玩家时无法提供Ray
}
