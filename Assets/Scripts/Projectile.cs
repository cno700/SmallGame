using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // 只选择Layermask层内的碰撞器，其他层内碰撞器忽略
    public Color trailColour; // ##### 通过该属性修改子弹轨迹颜色（有问题，未实现）

    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = .1f; // 碰撞检测时补偿敌人移动的距离

    private void Start()
    {
        Destroy(gameObject, lifetime); // 保证子弹超出一定时间会消失

        // 如果生成的子弹在敌人内部
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        // 返回以第一个参数为原点、第二个参数为半径的球体内，所有Layer为第三个参数的碰撞体集合
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColour); // ####### 无反应
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //hit: 使用out关键字传入一个空的碰撞信息类，然后碰撞后赋值。可以得到碰撞物体的transform,rigidbody,point等信息。
        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        // 只有拥有IDamageable接口的object才能触发击中方法
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}