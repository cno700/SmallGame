using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial; // 攻击时敌人颜色发生变化，所以这里要获取材质组件
    LivingEntity targetEntity; // 获取目标实体
    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius; // 自身碰撞半径
    float targetCollisionRadius; // 目标碰撞半径，敌人最终导航的目的地是接触到玩家，而不是玩家的准确位置

    bool hasTarget;

    private void Awake()
    {
        // 由于Enemy.SetCharacteristics()在敌人生成的那一帧就调用，会在Start()前调用，所以pathfinder、harTarget等后执行，将其放在Awake里就能可以了
        pathfinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>(); // 获取类而不是GameObject
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start(); // 继承并保留了父类的Start()中执行的所有代码，否则完全覆盖掉父类的Start方法
        //pathfinder = GetComponent<NavMeshAgent>();
        ////skinMaterial = GetComponent<Renderer>().material;
        ////originalColour = skinMaterial.color;

        //if (GameObject.FindGameObjectWithTag("Player") != null)
        if (hasTarget)
        {
            //    hasTarget = true;
            currentState = State.Chasing; // 设置敌人初始状态为Chasing
            //    target = GameObject.FindGameObjectWithTag("Player").transform;
            //    targetEntity = target.GetComponent<LivingEntity>(); // 获取类而不是GameObject
            targetEntity.OnDeath += OnTargetDeath; // 将方法注册到事件中，等待触发
            //    myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            //    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdatePath()); // 开始协程
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColour)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColour;
        originalColour = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
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
            // 使用协程IEnumerator，让敌人每隔0.25秒而不是一帧就去计算新路径，这在多个敌人、多种路径情况下有很大负担
            //pathfinder.SetDestination(target.position);

            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; // 只用平方值可省去开方步骤
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
        // 做出一个向前弓步的动作

        currentState = State.Attacking;
        pathfinder.enabled = false; // 攻击时关闭寻路功能
        /*
         * 但是这里有个问题就是当关闭了pathfinder之后，UpdatePath协程中会在
         * pathfinder.SetDestination(targetPosition)调用该属性，进而报错。
         * 所以这里考虑设置一个敌人的状态，什么状态下该做什么事。
         * 
         * 拓展（未做）：既然已经设置了状态，那是不是可以不设置Pathfinder的活性呢？
         */

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false; // 是否已经触发攻击

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
            // Lerp(Vector3 a, Vector3 b, float t) 返回 a+t*(b-a)

            yield return null; // 用0或者null来yield的意思是告诉协程等待下一帧，直到继续执行为止。
        }

        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true; // 攻击之后恢复导航功能
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
                if (!dead) // 作者的原意：如果对象在销毁后仍会运行一次寻找路径，所以要加个判断
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
