using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ɵ��˵Ľű�
public class Spawner : MonoBehaviour
{
    public Wave[] waves; // �жನ����
    public Enemy enemy;

    Wave currentWave; // ��ǰ�Ⲩ����
    int currentWaveNumber; // ��ǰ����index

    int enemiesRemainingToSpawn; // ��ǰ�ⲨҪ���ɶ��ٵ���
    int enemiesReaminingAlive; // ��ǰ�Ⲩ��ʣ���ٵ���
    float nextSpawnTime; // �´β�����ʱ��

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
            // ֻ�����˽ű�ע��OnEnemyDeath�¼�����Enemy�ű����Ǹ�Player�ű�ע��OnTargetDeath�¼�
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

    [System.Serializable] // ���˸þ��������ʾ��Inspector�����
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
