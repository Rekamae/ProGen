using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateDungeon : MonoBehaviour
{
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapDepth = 50;
    [SerializeField] private int scale = 2;
    [SerializeField] private GameObject playerPrefab;  // Reference to the player prefab

    private Leaf root;
    private byte[,] map;
    private List<Vector2> corridors = new List<Vector2>();

    private void Start()
    {
        map = new byte[mapWidth, mapDepth];
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, z] = 1;
            }
        }

        root = new Leaf(0, 0, mapWidth, mapDepth, scale);
        BSP(root, 6);

        AddCorridors();
        AddRandomCorridors(10);

        DrawMap();

        CreateFloor();

        InstantiatePlayer();
    }

    void AddRandomCorridors(int numHalls)
    {
        for (int i = 0; i < numHalls; i++)
        {
            int startX = Random.Range(5, mapWidth - 5);
            int startZ = Random.Range(5, mapDepth - 5);
            int length = Random.Range(5, mapWidth - 5);

            if (Random.Range(0, 100) < 50)
            {
                line(startX, startZ, length, startZ);
            }
            else
            {
                line(startX, startZ, startX, length);
            }
        }
    }

    private void BSP(Leaf l, int sDepth)
    {
        if (l == null) return;

        if (sDepth <= 0)
        {
            l.Draw(map);
            corridors.Add(new Vector2(l.xpos + l.width / 2, l.zpos + l.depth / 2));
            return;
        }

        if (l.Split())
        {
            BSP(l.leftChild, sDepth - 1);
            BSP(l.rightChild, sDepth - 1);
        }
        else
        {
            l.Draw(map);
            corridors.Add(new Vector2(l.xpos + l.width / 2, l.zpos + l.depth / 2));
        }
    }

    void AddCorridors()
    {
        for (int i = 1; i < corridors.Count; i++)
        {
            if ((int)corridors[i].x == (int)corridors[i - 1].x || (int)corridors[i].y == (int)corridors[i - 1].y)
            {
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i - 1].y);
            }
            else
            {
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i].x, (int)corridors[i - 1].y);
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i].y);
            }
        }
    }

    private void DrawMap()
    {
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[x, z] == 1)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * scale, 1, z * scale);
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                }
                else if (map[x, z] == 2)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * scale, 1, z * scale);
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                    cube.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 0, 0));
                }
            }
        }
    }

    public void line(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            map[x, y] = 0;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    private void CreateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3(mapWidth * scale / 2, 0, mapDepth * scale / 2);
        floor.transform.localScale = new Vector3(mapWidth * scale / 10, 1, mapDepth * scale / 10);
        floor.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f)); // Optional: set floor color
    }

    private void InstantiatePlayer()
    {
        // Find an open spot to place the player
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[x, z] == 0) // This is an open space
                {
                    Vector3 playerPosition = new Vector3(x * scale, 1, z * scale); // Adjust Y position if needed
                    Instantiate(playerPrefab, playerPosition, Quaternion.identity);
                    return; // Player instantiated, no need to continue
                }
            }
        }
    }
}
