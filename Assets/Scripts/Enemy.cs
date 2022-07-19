using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent pathfinder;
    Transform target;

    protected override void Start()
    {
        base.Start(); // �̳в������˸����Start()��ִ�е����д��룬������ȫ���ǵ������Start����
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        // ʹ��Э��IEnumerator���õ���ÿ��0.25�������һ֡��ȥ������·�������ڶ�����ˡ�����·��������кܴ󸺵�
        //pathfinder.SetDestination(target.position);
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            if (!dead) // ���ߵ�ԭ�⣺������������ٺ��Ի�����һ��Ѱ��·��������Ҫ�Ӹ��ж�
            {
                pathfinder.SetDestination(targetPosition);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
