using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps; // 自行在Inspector面板里设置多张参数不同的地图
    public int mapIndex;

    public Transform tilePrefab; // 地砖预制体
    public Transform obstaclePrefeb; // 障碍物预制体
    public Transform navmeshFloor; // 导航网格流（因为不能动态烘焙，所以将其设为最大且不可视）
    public Transform navmeshMaskPrefeb; // 导航网格蒙版预制体（将地图以外的导航区域设为障碍阻挡）
    //public Vector2 mapSize;
    public Vector2 maxMapSize; // 最大地图的大小，用来确定Navigation的烘焙范围

    [Range(0, 1)] // 限定范围在[0, 1]
    public float outlinePercent; // 设置地砖的大小百分比
    //[Range(0, 1)] 
    //public float obstaclePercent = 0.1f; // 障碍数量百分比

    public float tileSize = 1; // 一块地砖的大小

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    //public int seed = 10;
    //Coord mapCenter;

    Map currentMap;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);
        // 能让玩家站在上面的collider

        // Generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        //mapCenter = new Coord((int)currentMap.mapSize.x / 2, (int)currentMap.mapSize.y/2);
                
        
        // Creating map holder object
        // 先销毁旧地图
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;


        // Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for(int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(new Vector3(90, 0, 0)));
                // transform.rotation其实是一个四元数，Quaternion.Euler函数可以将欧拉角转化为四元数
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }


        // Spawning obstacles
        // 记录障碍物的索引
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent); // 障碍数量
        int currentObstacleCount = 0;
        // 从随机队列里取队头obstacle个作为障碍物
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            // 检查当前这个位置是否可以生成障碍物（防止围住某一部分）
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefeb, obstaclePosition + Vector3.up * obstacleHeight/2 * tileSize, Quaternion.identity);
                newObstacle.parent = mapHolder;
                //newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                //newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
                /*
                 * 上面是老师视频里的写法，但是obstacleHeight也应该乘上tileSize，
                 * 否则当TileSize不为1时障碍物会离地
                 */
                newObstacle.localScale = new Vector3(1 - outlinePercent, obstacleHeight, 1 - outlinePercent) * tileSize;

                // 渲染障碍物颜色
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial); // 使用material属性可能会造成内存泄漏
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y; // 提前转化为float防止int整除
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            } 
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }


        // Creating navmesh mask
        // 铺上导航网格四周障碍物
        Transform maskLeft = Instantiate(navmeshMaskPrefeb, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefeb, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefeb, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y)/2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefeb, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        // 铺上导航网格
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        // 因为之前已经将其绕x轴旋转90度，所以此时其大小只受x、y影响，而不是x、z。
    }

    // 检查新选择的障碍物是否可行
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }

                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y) - currentObstacleCount;
        return accessibleTileCount == targetAccessibleTileCount;
    }

    // 将地砖索引转化为地图实际位置
    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + .5f + x, 0, -currentMap.mapSize.y / 2f + .5f + y) * tileSize;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
    // 自定义坐标类（存储所有地砖）
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable] // 将该类的属性显示在面板上
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight; // 最小障碍物高度
        public float maxObstacleHeight; // 最大障碍物高度
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
