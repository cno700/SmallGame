using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 只有拥有IDamageable接口的object才能触发击中方法
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath; // 1.向Spawner提示消灭一个敌人 2.向Enemy提示玩家被消灭
    // 这里目前有个记忆方法：委托是我希望发生了什么事，别人会怎样，
    // 那么就要在自己的类里定义事件，在别人那里将希望他做的事注册到我的事件里，
    // 之后等到我什么时候发生事情了，那么调用之前注册过的别人的方法。

    protected virtual void Start() // virtual虚方法
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) // 收到伤害，生命值减一
    {
        // do some stuff here with hit var
        // 比如实现碰撞粒子效果
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")] // （方便在编辑器不运行的状态下进行调试——运行后右击脚本组件）
    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
