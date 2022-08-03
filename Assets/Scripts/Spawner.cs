using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成敌人的脚本
public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves; // 有多波敌人
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave; // 当前这波敌人
    int currentWaveNumber; // 当前波的index

    int enemiesRemainingToSpawn; // 当前这波要生成多少敌人
    int enemiesReaminingAlive; // 当前这波还剩多少敌人
    float nextSpawnTime; // 下次产生的时间

    MapGenerator map; // 获取MapGenerator中没有生成障碍物的地砖列表

    float timeBetweenCampingChecks = 3; // 设置检查玩家位置时间段（如每3秒检查一次）
    float nextCampCheckTime; // 下次检查玩家位置时间
    float campThresholdDistance = 1.5f; // 每次检查玩家需要移动的最少距离（否则会在当前位置刷新敌人）
    Vector3 campPositionOld; // 上次检查时玩家位置
    bool isCamping;

    bool isDisabled;
    // 只有该值为false时才会执行update方法，目前影响该值的因素有：
    // 玩家是否死亡

    public event System.Action<int> OnNewWave; // 生成下一波敌人时（条件）切换新的地图（目标）

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();

        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            // 首先判断玩家在一定时间内是否移动一定距离
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance;
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");

            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1; // 地砖闪烁时间
        float tileFlashSpeed = 4; // 每秒闪烁次数？？（实际效果看来并不是，大概闪两下）

        Transform spawnTile = map.GetRandomOpenTile(); // 获取随机无障碍物地砖
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        // 使地砖先呈现闪烁效果，再生成敌人
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = tileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            // Mathf.PingPong 第一个参数为时间轴，一般用Time.time，这样会在1秒内插值会从0到1再到0
            // 这里乘了一个倍数加快闪烁，具体怎么加快的还不清楚
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        // 只给敌人脚本注册OnEnemyDeath事件，而Enemy脚本中是给Player脚本注册OnTargetDeath事件

        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColour);
        // Enemy.SetCharacteristics()将会在Start()前调用
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesReaminingAlive--;
        if (enemiesReaminingAlive == 0)
        {
            NextWave();
        }
    }

    // 更新玩家位置至地图中心
    void ResetPlayerPosition()
    {
        //playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
        // 教程里用的上面写法，但传的其实是（0,0）坐标的地砖，而不是地图中心，直接传(0,0,0)应该就可以
        playerT.position = new Vector3(0, 2, 0);
    }

    void NextWave()
    {
        if (currentWaveNumber < waves.Length)
        {
            //print("Wave: " + currentWaveNumber);
            currentWave = waves[currentWaveNumber];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesReaminingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            currentWaveNumber++;
            ResetPlayerPosition(); // 每次更新地图时将玩家移至地图中心，防止地图大小发生变化时玩家从图上掉落
        }
    }

    [System.Serializable] // 加了该句后该 类 会显示在Inspector面板上
    public class Wave
    {
        public bool infinite; // 最后一波无限敌人
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer = 2;
        public float enemyHealth = 2;
        public Color skinColour;
    }
}
