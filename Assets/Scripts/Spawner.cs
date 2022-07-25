using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成敌人的脚本
public class Spawner : MonoBehaviour
{
    public Wave[] waves; // 有多波敌人
    public Enemy enemy;

    Wave currentWave; // 当前这波敌人
    int currentWaveNumber; // 当前波的index

    int enemiesRemainingToSpawn; // 当前这波要生成多少敌人
    int enemiesReaminingAlive; // 当前这波还剩多少敌人
    float nextSpawnTime; // 下次产生的时间

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath; 
            // 只给敌人脚本注册OnEnemyDeath事件，而Enemy脚本中是给Player脚本注册OnTargetDeath事件
        }
    }

    void OnEnemyDeath()
    {
        enemiesReaminingAlive--;
        if (enemiesReaminingAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        if (currentWaveNumber < waves.Length)
        {
            print("Wave: " + currentWaveNumber);
            currentWave = waves[currentWaveNumber++];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesReaminingAlive = enemiesRemainingToSpawn;
        }
    }

    [System.Serializable] // 加了该句后该类会显示在Inspector面板上
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
