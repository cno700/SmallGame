using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // ֻѡ��Layermask���ڵ���ײ��������������ײ������
    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = .1f; // ��ײ���ʱ���������ƶ��ľ���

    private void Start()
    {
        Destroy(gameObject, lifetime); // ��֤�ӵ�����һ��ʱ�����ʧ

        // ������ɵ��ӵ��ڵ����ڲ�
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        // �����Ե�һ������Ϊԭ�㡢�ڶ�������Ϊ�뾶�������ڣ�����LayerΪ��������������ײ�弯��
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
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

        //hit: ʹ��out�ؼ��ִ���һ���յ���ײ��Ϣ�࣬Ȼ����ײ��ֵ�����Եõ���ײ�����transform,rigidbody,point����Ϣ��
        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        //print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        // ֻ��ӵ��IDamageable�ӿڵ�object���ܴ������з���
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}