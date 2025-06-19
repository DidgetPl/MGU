using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 TODO
 [+] Przygotowaæ modele w folderach
 [+] Spawning hangarów i lotnisk
 [+] Przygotowaæ prefaby samolotów
 [+] Spawning samolotów w powietrzu i na ziemi
 [+] Poruszanie siê samolotów niektórych
 [] Repo...?
 [] Prezentacja
 
 */

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int length = 512;
    [SerializeField] int depth = 20;
    [SerializeField] float scale = 20f;

    [SerializeField] float offsetX = 100f;
    [SerializeField] float offsetY = 100f;

    [SerializeField] List<Vector2> roadPositions = new List<Vector2>();
    [SerializeField] int roadRadius = 10;

    [SerializeField] List<Vector2Int> hillCenters = new List<Vector2Int>();
    [SerializeField] float hillRadius = 10f;
    [SerializeField] float hillBoost = 15f;

    [SerializeField] GameObject airport;
    [SerializeField] GameObject road;
    [SerializeField] GameObject planesParent;

    [SerializeField] Vector2 playerPosition = Vector2.one * 256;
    [SerializeField] float planeTargetDirectionModifier = 150f;

    [SerializeField] List<GameObject> planes = new List<GameObject>();
    [SerializeField] int numOfPlanes = 40;

    float[,] heights;
    List<Vector2> dir = new List<Vector2>() {Vector2.down, Vector2.left, Vector2.up, Vector2.right};

    void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        CreateRandomRoad();
        CreateRandomRoad();
        CreateRandomRoad();

        int startingCount = roadPositions.Count;

        for(int i = 0; i< startingCount; i++)
            for (int j = i+1; j < startingCount; j++)
                if( j!=i && Vector2.Distance(roadPositions[i], roadPositions[j]) < 15)
                    roadPositions.Add((roadPositions[i] + roadPositions[j]) / 2);

        while(hillCenters.Count < 5)
            CreateHill(Random.Range(50, length-50), Random.Range(50, length - 50));

        GenerateTerrain();
        GeneratePlanes(numOfPlanes);
    }

    void CreateRandomStraightRoad()
    {
        bool isHorizontal = Random.Range(0f, 1f) > 0.5f;
        Vector2 firstRoadTile = isHorizontal ? new Vector2(7.5f + Random.Range(0, length / 10) * 10f, 7.5f) :
                                               new Vector2(7.5f, 7.5f + Random.Range(0, length / 10) * 10f);
                  
        CreateRoadTile(firstRoadTile.x, firstRoadTile.y);
        for (int i = 1; i < length/10; i++)
        {
            CreateRoadTile(isHorizontal ? firstRoadTile.x : 7.5f + 10f*i, isHorizontal ? 7.5f + 10f*i : firstRoadTile.y);
        }
    }

    void CreateRandomRoad()
    {
        int did = Random.Range(0, 4);
        int numOfTiles = 0;
        Vector2 direction = dir[did];
        Vector2 roadTilePos = did == 0 ? new Vector2(7.5f + Random.Range(0, length / 10) * 10f, length - 7.5f) :
                                did == 1 ? new Vector2(length - 7.5f, 7.5f + Random.Range(0, length / 10) * 10f) :
                                did == 2 ? new Vector2(7.5f + Random.Range(0, length / 10) * 10f, 7.5f) :
                                new Vector2(7.5f, 7.5f + Random.Range(0, length / 10) * 10f);
        int epLength = Random.Range(10, 14);
        while(roadTilePos.x > 0 && roadTilePos.y > 0 && roadTilePos.x < length && roadTilePos.y < length && numOfTiles < 100)
        {
            CreateRoadTile(roadTilePos.x, roadTilePos.y);
            for(int i = 0; i < epLength; i++)
            {
                roadTilePos += direction * 10f;
                CreateRoadTile(roadTilePos.x, roadTilePos.y);
                numOfTiles++;
            }
            int newDid = Random.Range(0, 4);
            while ((newDid + did) % 2 == 0)
                newDid = Random.Range(0, 4);
            did = newDid;
            direction = dir[did];
            epLength = Random.Range(4, 12);
        }

    }

    void CreateRoadTile(float x, float y)
    {
        if(!roadPositions.Contains(new Vector2(x, y)) && x > 7.5f && y > 7.5f && x < length && y < length)
        {
            roadPositions.Add(new Vector2(x, y));
            Instantiate(road).transform.position = new Vector3(y, 0, x);
        }
    }

    bool CreateHill(int x, int y)
    {
        bool canCreate = true;
        foreach (Vector2 roadTile in roadPositions)
            if (Mathf.Max(Mathf.Abs(roadTile.x - x), Mathf.Abs(roadTile.y - y)) <= 40)
                canCreate = false;

        foreach (Vector2 hill in hillCenters)
            if (Mathf.Max(Mathf.Abs(hill.x - x), Mathf.Abs(hill.y - y)) <= 50)
                canCreate = false;

        if (canCreate)
        {
            hillCenters.Add(new Vector2Int(x, y));
            Instantiate(airport).transform.position = new Vector3(y, 15, x);
        }

        return canCreate;
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    public void GeneratePlanes(int numOfPlanes)
    {
        for (int i = 0; i < numOfPlanes; i++)
        {
            GameObject plane = Instantiate(planes[Random.Range(0, planes.Count)], planesParent.transform);
            Transform planeT = plane.transform;
            Plane planeP = plane.GetComponent<Plane>();

            planeP.terrainLength = length;
            planeP.tg = this;

            if (Random.Range(0f, 1f) > 0.35f)
            {
                planeP.onGround = false;
                planeT.position = new Vector3(Random.Range(0, length), Random.Range(55f, 100f), Random.Range(0, length));

                (float, float, LineType) line = LineMath.GetPerpendicularLineThroughB(new Vector2(planeT.position.x, planeT.position.z), playerPosition);
                Vector2 vectorToLook;
                float modifier = Random.Range(-planeTargetDirectionModifier, planeTargetDirectionModifier);
                    vectorToLook = line.Item3 == LineType.Regular ?
                        LineMath.GetPointsAlongLine(playerPosition, line.Item1, line.Item3, modifier)[Random.Range(0, 1)] :
                        new Vector2(playerPosition.x, playerPosition.y + modifier);
                planeT.LookAt(new Vector3(vectorToLook.x, planeT.position.y, vectorToLook.y));
            }
            else
            {
                planeP.onGround = true;
                Vector2 hill = hillCenters[Random.Range(0, hillCenters.Count)];
                planeT.position = new Vector3(hill.y, 16 + planeP.airportShift, hill.x) + new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
                planeT.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            }
        }
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = length + 1;
        terrainData.size = new Vector3(length, depth, length);
        heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[length, length];
        for (int x = 0; x < length; x++)
            for (int y = 0; y < length; y++)
                heights[x, y] = CalculateHeight(x, y);
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / length * scale + offsetX;
        float yCoord = (float)y / length * scale + offsetY;
        float baseHeight = Mathf.Pow(Mathf.PerlinNoise(xCoord, yCoord), 1.5f);

        float hillFactor = 0f;

        foreach (var hill in hillCenters)
        {
            float dist = Mathf.Max(Mathf.Abs(x - hill.x), Mathf.Abs(y - hill.y));
            if (dist < hillRadius)
            {
                float t = 1f - (dist / hillRadius);
                hillFactor += t * t * hillBoost;
            }
        }

        foreach (var pos in roadPositions)
        {
            float dist = Mathf.Max(Mathf.Abs(x - pos.x), Mathf.Abs(y - pos.y));
            if (dist < roadRadius)
            {
                float t = 1f - (dist / roadRadius);
                hillFactor -= t * t * 10;
            }
        }

        return Mathf.Clamp01(baseHeight + hillFactor);
    }
}
