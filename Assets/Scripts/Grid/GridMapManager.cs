using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : MonoBehaviour
{
    public static GridMapManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GridMapManager found!");
            return;
        }
        instance = this;
    }

    public GridMap gridMap;

    public int width = 15;
    public int height = 10;
    public float cellSize = 1f;
    public Vector3 originPosition = new Vector3(-7, -5, 0);

    //Directions for getting neighbors
    private readonly List<Vector2Int> Directions = new List<Vector2Int>() {
            new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1)
        };

    public Dictionary<Vector2, NodeBase> nodes { get; private set; }

    void Start()
    {
        gridMap = new GridMap(width, height, cellSize, originPosition);

        nodes = GenerateNodes();
        foreach (var tile in nodes.Values) tile.CacheNeighbors();

        if(TryGetComponent<GridVisualizer>(out GridVisualizer visualizer))
        {
            visualizer.VisualizeGrid(width, height);
        }
    }

    //used to get neighbors of an object on grid
    public List<Vector2Int> GetObjectNeighbors(Vector2Int objectPosition, int objectWidth, int objectHeight)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int direction in Directions)
        {
            for (int x = 0; x < objectWidth; x++)
            {
                for (int y = 0; y < objectHeight; y++)
                {
                    int neighborX = objectPosition.x + direction.x + x;
                    int neighborY = objectPosition.y + direction.y + y;

                    if (neighborX >= objectPosition.x && neighborX < objectPosition.x + objectWidth
                        && neighborY >= objectPosition.y && neighborY < objectPosition.y + objectHeight)
                    {
                        //if true we are inside the object
                    }
                    else
                    {
                        Vector2Int neighbor = new Vector2Int(neighborX, neighborY);
                        if (!neighbors.Contains(neighbor))
                        {
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }
        return neighbors;
    }

    //nodes used to pathfind
    private Dictionary<Vector2, NodeBase> GenerateNodes()
    {
        Dictionary<Vector2, NodeBase> nodes = new Dictionary<Vector2, NodeBase>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 coords = new Vector2(x, y);
                NodeBase tile = new NodeBase(coords, true);
                nodes.Add(coords, tile);
            }
        }
        return nodes;
    }

    public NodeBase GetNodeAtPosition(Vector2 pos) => nodes.TryGetValue(pos, out var tile) ? tile : null;
}
