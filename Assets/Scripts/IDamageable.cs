using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // hit用来提供一些额外的信息，比如在哪里被击中
    void TakeHit(float damage, RaycastHit hit);
}
