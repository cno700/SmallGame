using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab; // ��שԤ����
    public Transform obstaclePrefeb; // �ϰ���Ԥ����
    public Vector2 mapSize;

    [Range(0, 1)] // �޶���Χ��[0, 1]
    public float outlinePercent; // ���õ�ש�Ĵ�С�ٷֱ�
    [Range(0, 1)] 
    public float obstaclePercent = 0.1f; // �ϰ������ٷֱ�

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;
    Coord mapCenter;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y/2);
                
        // �����پɵ�ͼ
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(new Vector3(90, 0, 0)));
                // transform.rotation��ʵ��һ����Ԫ����Quaternion.Euler�������Խ�ŷ����ת��Ϊ��Ԫ��
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        // ��¼�ϰ��������
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent); // �ϰ�����
        int currentObstacleCount = 0;
        // �����������ȡ��ͷobstacle����Ϊ�ϰ���
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            // ��鵱ǰ���λ���Ƿ���������ϰ����ֹΧסĳһ���֣�
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefeb, obstaclePosition + Vector3.up, Quaternion.identity);
                newObstacle.parent = mapHolder;
            } 
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    // �����ѡ����ϰ����Ƿ����
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;

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

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y) - currentObstacleCount;
        return accessibleTileCount == targetAccessibleTileCount;
    }

    // ����ש����ת��Ϊ��ͼʵ��λ��
    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + .5f + x, 0, -mapSize.y / 2 + .5f + y);
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    // �Զ��������ࣨ�洢���е�ש��
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
}
