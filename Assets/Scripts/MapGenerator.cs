using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps; // ������Inspector��������ö��Ų�����ͬ�ĵ�ͼ
    public int mapIndex;

    public Transform tilePrefab; // ��שԤ����
    public Transform obstaclePrefeb; // �ϰ���Ԥ����
    public Transform navmeshFloor; // ��������������Ϊ���ܶ�̬�決�����Խ�����Ϊ����Ҳ����ӣ�
    public Transform navmeshMaskPrefeb; // ���������ɰ�Ԥ���壨����ͼ����ĵ���������Ϊ�ϰ��赲��
    //public Vector2 mapSize;
    public Vector2 maxMapSize; // ����ͼ�Ĵ�С������ȷ��Navigation�ĺ決��Χ

    [Range(0, 1)] // �޶���Χ��[0, 1]
    public float outlinePercent; // ���õ�ש�Ĵ�С�ٷֱ�
    //[Range(0, 1)] 
    //public float obstaclePercent = 0.1f; // �ϰ������ٷֱ�

    public float tileSize = 1; // һ���ש�Ĵ�С

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
        // �������վ�������collider

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
        // �����پɵ�ͼ
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
                // transform.rotation��ʵ��һ����Ԫ����Quaternion.Euler�������Խ�ŷ����ת��Ϊ��Ԫ��
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }


        // Spawning obstacles
        // ��¼�ϰ��������
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent); // �ϰ�����
        int currentObstacleCount = 0;
        // �����������ȡ��ͷobstacle����Ϊ�ϰ���
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            // ��鵱ǰ���λ���Ƿ���������ϰ����ֹΧסĳһ���֣�
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefeb, obstaclePosition + Vector3.up * obstacleHeight/2 * tileSize, Quaternion.identity);
                newObstacle.parent = mapHolder;
                //newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                //newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
                /*
                 * ��������ʦ��Ƶ���д��������obstacleHeightҲӦ�ó���tileSize��
                 * ����TileSize��Ϊ1ʱ�ϰ�������
                 */
                newObstacle.localScale = new Vector3(1 - outlinePercent, obstacleHeight, 1 - outlinePercent) * tileSize;

                // ��Ⱦ�ϰ�����ɫ
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial); // ʹ��material���Կ��ܻ�����ڴ�й©
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y; // ��ǰת��Ϊfloat��ֹint����
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
        // ���ϵ������������ϰ���
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

        // ���ϵ�������
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        // ��Ϊ֮ǰ�Ѿ�������x����ת90�ȣ����Դ�ʱ���Сֻ��x��yӰ�죬������x��z��
    }

    // �����ѡ����ϰ����Ƿ����
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

    // ����ש����ת��Ϊ��ͼʵ��λ��
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

    [System.Serializable] // �������������ʾ�������
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight; // ��С�ϰ���߶�
        public float maxObstacleHeight; // ����ϰ���߶�
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
