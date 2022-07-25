using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath; // 1.��Spawner��ʾ����һ������ 2.��Enemy��ʾ��ұ�����
    // ����Ŀǰ�и����䷽����ί������ϣ��������ʲô�£����˻�������
    // ��ô��Ҫ���Լ������ﶨ���¼����ڱ������ｫϣ����������ע�ᵽ�ҵ��¼��
    // ֮��ȵ���ʲôʱ���������ˣ���ô����֮ǰע����ı��˵ķ�����

    protected virtual void Start() // virtual�鷽��
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit) // �յ��˺�������ֵ��һ
    {
        // do some stuff here with hit var
        // ����ʵ����ײ����Ч��
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

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
