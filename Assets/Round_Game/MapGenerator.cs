using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Parameters")]
    public int width = 20;
    public int height = 20;
    
    [Header("Tuning")]
    [Range(0f, 1f)]
    public float emptyAreaPercentage = 0.65f;
    public int smoothingRuns = 1;
    [Range(0f, 1f)]
    public float obstacleCoverage = 0.8f; 

    private int walkSteps = 200;
    private int[,] map; // 2D grid of tiles
    public List<(int x,int y)> spawns = new List<(int x, int y)>();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BeginGeneration()
    {
        //make map size proportional to # players
        //width += 2 * GameManager.instance.playerCount;
        //height += 2 * GameManager.instance.playerCount;

        //make emptiness relative to map size
        walkSteps = (int) (((width-1)*(height-1))*emptyAreaPercentage);
        //Debug.Log("Width,Height:"+width+","+height+"\nWalk Steps:" + walkSteps);

        //--- generate map shape with random walk
        generateMap();
        //optional smoothing to rough edges
        for(int i = 0; i < smoothingRuns; i++)
        {
            smoothEdges();
        }
        //add tiles to tile map
        drawMap();
        //-- add spawn points & carve out surrounding walls
        //generateSpawns(GameManager.instance.playerCount);
        generateSpawns(2);//                                    FOR TESTING ONLY
    }

    //random walk algorithm
    void generateMap()
    {
        // Initialize the map with solid tiles (1 = Wall)
        map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 1; // Set everything as solid
            }
        }

        // Start position in the middle
        Vector2Int position = new Vector2Int(width / 2, height / 2);
        map[position.x, position.y] = 0; // Open space

        int stepsLeft = walkSteps;
        while (stepsLeft > 0)
        {
            int direction = Random.Range(0, 4);
            switch (direction)
            {
                case 0: position.x += 1; break; // Right
                case 1: position.x -= 1; break; // Left
                case 2: position.y += 1; break; // Up
                case 3: position.y -= 1; break; // Down
            }
            if(map[position.x, position.y] == 0)
            {
                //stepsLeft -= Random.Range(0, 2);
                continue;
            }

            if(position.x < 1 || position.x > width - 2 || position.y < 1 || position.y > height - 2)
            {
                position.x = Mathf.Clamp(position.x, 1, width - 2);
                position.y = Mathf.Clamp(position.y, 1, height - 2);
                continue;
            }
            else
            {
                map[position.x, position.y] = 0;
                stepsLeft--;
            }
        }

    }

    void smoothEdges()
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int wallCount = countWallNeighbors(x, y);

                if (wallCount > 2)
                    newMap[x, y] = 1; // Stay a wall
                else if (wallCount < 3)
                    newMap[x, y] = 0; // Become open space
                else
                    newMap[x, y] = map[x, y]; // Stay the same
            }
        }

        map = newMap;
    }

    int countWallNeighbors(int x, int y)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = x + dx;
                int neighborY = y + dy;

                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (map[neighborX, neighborY] == 1) count++;
                }
                else
                {
                    count++; // Treat out-of-bounds as walls
                }
            }
        }

        return count;
    }

    [Header("Tile Parameters")]
    public Tilemap wallTileMap;
    public TileBase wallTile;
    public Tilemap groundTileMap;
    public TileBase groundTile;
    public Tilemap obstacleTileMap;
    public TileBase obstacleTile;

    void drawMap()
    {
        wallTileMap.ClearAllTiles();
        groundTileMap.ClearAllTiles();
        obstacleTileMap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (map[x, y] == 1)
                {
                    wallTileMap.SetTile(position, wallTile);
                }
                else
                {
                    groundTileMap.SetTile(position, groundTile);
                    if(Random.value<obstacleCoverage)
                        obstacleTileMap.SetTile(position, obstacleTile);
                }
            }
        }
    }


    void generateSpawns(int numOfSpawns)
    {
        for(int i = 0;i<numOfSpawns; i++)
        {
            bool pointValid = false;
            while (!pointValid)
            {
                int x = Random.Range(1, width);
                int y = Random.Range(1, height);

                Vector3Int position = new Vector3Int(x, y, 0);
                if(groundTileMap.GetTile(position) != null && !spawns.Contains((x,y))){ //inefficient but its just for a small number of players ,hopefully...
                    spawns.Add((x,y));
                    clearObstacles(x,y);
                    pointValid = true;
                }
            }
        
        }
    }

    /// <summary>
    /// Clears all tiles around a point in the form of a cross 
    /// </summary>
    /// <param name="x">x coordinate of point</param>
    /// <param name="y">y coordinate of point</param>
    void clearObstacles(int x, int y)
    {
        for (int i = 0; i < 3; i++)
        {
            obstacleTileMap.SetTile(new Vector3Int(x - 1 + i, y, 0),null);
            obstacleTileMap.SetTile(new Vector3Int(x, y - 1 + i, 0), null);
        }
    }

}
