using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial; // ����ʱ������ɫ�����仯����������Ҫ��ȡ�������
    LivingEntity targetEntity; // ��ȡĿ��ʵ��
    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius; // ������ײ�뾶
    float targetCollisionRadius; // Ŀ����ײ�뾶���������յ�����Ŀ�ĵ��ǽӴ�����ң���������ҵ�׼ȷλ��

    bool hasTarget;

    protected override void Start()
    {
        base.Start(); // �̳в������˸����Start()��ִ�е����д��룬������ȫ���ǵ������Start����
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            currentState = State.Chasing; // ���õ��˳�ʼ״̬ΪChasing
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>(); // ��ȡ�������GameObject
            targetEntity.OnDeath += OnTargetDeath; // ������ע�ᵽ�¼��У��ȴ�����


            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath()); // ��ʼЭ��
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if (hasTarget)
        {
            // ʹ��Э��IEnumerator���õ���ÿ��0.25�������һ֡��ȥ������·�������ڶ�����ˡ�����·��������кܴ󸺵�
            //pathfinder.SetDestination(target.position);

            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; // ֻ��ƽ��ֵ��ʡȥ��������
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        // ����һ����ǰ�����Ķ���

        currentState = State.Attacking;
        pathfinder.enabled = false; // ����ʱ�ر�Ѱ·����
        /*
         * ���������и�������ǵ��ر���pathfinder֮��UpdatePathЭ���л���
         * pathfinder.SetDestination(targetPosition)���ø����ԣ���������
         * �������￼������һ�����˵�״̬��ʲô״̬�¸���ʲô�¡�
         * 
         * ��չ��δ��������Ȼ�Ѿ�������״̬�����ǲ��ǿ��Բ�����Pathfinder�Ļ����أ�
         */

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false; // �Ƿ��Ѿ���������

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            // y = 4(-x^2 + x) [0,0.5,1] -> [0,1,0]
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            // Lerp(Vector3 a, Vector3 b, float t) ���� a+t*(b-a)

            yield return null; // ��0����null��yield����˼�Ǹ���Э�̵ȴ���һ֡��ֱ������ִ��Ϊֹ��
        }

        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true; // ����֮��ָ���������
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead) // ���ߵ�ԭ�⣺������������ٺ��Ի�����һ��Ѱ��·��������Ҫ�Ӹ��ж�
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
