using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Random = UnityEngine.Random;

public class CreateDungeon : MonoBehaviour
{
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapDepth = 50;
    [SerializeField] private int scale = 2;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab; // Reference to the Cinemachine virtual camera prefab

    private Leaf root;
    private byte[,] map;
    private List<Vector2> rooms = new List<Vector2>();
    private List<Vector2> corridors = new List<Vector2>();
    private List<Room> roomObjects = new List<Room>();

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

        EnsureConnectivity();

        DrawMap();
        CreateFloor();
        InstantiatePlayer();
        InstantiateCamera();
    }

    void EnsureConnectivity()
    {
        if (rooms.Count < 2)
            return;

        // Minimum Spanning Tree (MST) algorithm (Prim's algorithm)
        List<Vector2> connectedRooms = new List<Vector2> { rooms[0] };
        HashSet<Vector2> unconnectedRooms = new HashSet<Vector2>(rooms);
        unconnectedRooms.Remove(rooms[0]);

        while (unconnectedRooms.Count > 0)
        {
            Vector2 closestRoom = Vector2.zero;
            float closestDistance = float.MaxValue;

            foreach (Vector2 connectedRoom in connectedRooms)
            {
                foreach (Vector2 unconnectedRoom in unconnectedRooms)
                {
                    float distance = Vector2.Distance(connectedRoom, unconnectedRoom);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestRoom = unconnectedRoom;
                    }
                }
            }

            if (closestRoom != Vector2.zero)
            {
                // Draw a corridor to connect closestRoom with the closest connectedRoom
                Vector2 lastRoom = connectedRooms[connectedRooms.Count - 1];
                DrawCorridor(lastRoom, closestRoom);

                connectedRooms.Add(closestRoom);
                unconnectedRooms.Remove(closestRoom);
            }
        }

        AddRandomCorridors(10);
    }

    private void BSP(Leaf l, int sDepth)
    {
        if (l == null) return;

        if (sDepth <= 0)
        {
            l.Draw(map);
            rooms.Add(new Vector2(l.xpos + l.width / 2, l.zpos + l.depth / 2));
            roomObjects.Add(new Room(l.xpos, l.zpos, l.width, l.depth, scale));
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
            rooms.Add(new Vector2(l.xpos + l.width / 2, l.zpos + l.depth / 2));
            roomObjects.Add(new Room(l.xpos, l.zpos, l.width, l.depth, scale));
        }
    }

    void DrawCorridor(Vector2 start, Vector2 end)
    {
        int x1 = (int)start.x;
        int y1 = (int)start.y;
        int x2 = (int)end.x;
        int y2 = (int)end.y;

        if (Random.Range(0, 2) == 0)
        {
            line(x1, y1, x2, y1);
            line(x2, y1, x2, y2);
        }
        else
        {
            line(x1, y1, x1, y2);
            line(x1, y2, x2, y2);
        }
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
        floor.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));

        floor.layer = LayerMask.NameToLayer("Ground");
    }

    private void InstantiatePlayer()
    {
        for (int z = 0; z < mapDepth; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[x, z] == 0)
                {
                    Vector3 playerPosition = new Vector3(x * scale, 1, z * scale);
                    GameObject player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);

                    return;
                }
            }
        }
    }

    private void InstantiateCamera()
    {
        if (cameraPrefab == null)
        {
            Debug.LogError("Camera prefab is not assigned.");
            return;
        }

        GameObject cameraObject = Instantiate(cameraPrefab);
        CinemachineVirtualCamera virtualCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Transform targetTransform = player.transform.Find("target");
                if (targetTransform != null)
                {
                    virtualCamera.Follow = targetTransform;
                    virtualCamera.LookAt = targetTransform;
                }
                else
                {
                    Debug.LogError("Target GameObject not found as a child of the player.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }
        else
        {
            Debug.LogError("The camera prefab does not contain a CinemachineVirtualCamera component.");
        }
    }
}

public class Room
{
    public int xpos;
    public int zpos;
    public int width;
    public int depth;
    private int scale;

    public Room(int x, int z, int w, int d, int s)
    {
        xpos = x;
        zpos = z;
        width = w;
        depth = d;
        scale = s;
    }
    
}
