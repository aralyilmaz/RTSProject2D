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

    public Dictionary<Vector2, NodeBase> tiles { get; private set; }

    void Start()
    {
        gridMap = new GridMap(width, height, cellSize, originPosition);
        tiles = GenerateTiles();

        foreach (var tile in tiles.Values) tile.CacheNeighbors();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //grid.SetValue(MouseRTSController.instance.mouseWorldPosition, 1);
        }
    }

    private Dictionary<Vector2, NodeBase> GenerateTiles()
    {
        Dictionary<Vector2, NodeBase> tiles = new Dictionary<Vector2, NodeBase>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //var tile = Instantiate(nodeBasePrefab, grid.transform);
                //tile.Init(DecideIfObstacle(), new SquareCoords { Pos = new Vector3(x, y) });
                Vector2 coords = new Vector2(x, y);
                NodeBase tile = new NodeBase(coords, true);
                tiles.Add(coords, tile);
            }
        }
        return tiles;
    }

    public NodeBase GetTileAtPosition(Vector2 pos) => tiles.TryGetValue(pos, out var tile) ? tile : null;
}
