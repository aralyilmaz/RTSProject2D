using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Helper class for A* pathfinding
public class NodeBase
{
    public List<NodeBase> neighbors { get; protected set; }

    //for retracing the path once path found
    public NodeBase connection { get; private set; }

    //distance from start node
    public float G { get; private set; }

    //distance to target node
    public float H { get; private set; }

    //whenever accessed will dynamically calculate the value of F. 
    public float F => G + H;

    public bool walkable;

    public Vector2 coords;

    private static readonly List<Vector2> Dirs = new List<Vector2>() {
            new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0)
            //new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1)
        };

    public NodeBase(Vector2 coords, bool walkable)
    {
        this.coords = coords;
        this.walkable = walkable;
    }

    public void SetConnection(NodeBase nodeBase)
    {
        connection = nodeBase;
    }

    public void SetG(float g)
    {
        G = g;
    }

    public void SetH(float h)
    {
        H = h;
    }

    public void CacheNeighbors()
    {
        neighbors = new List<NodeBase>();

        foreach (var tile in Dirs.Select(dir => GridMapManager.instance.GetNodeAtPosition(coords + dir)).Where(tile => tile != null))
        {
            neighbors.Add(tile);
        }
    }

    public float GetDistance(Vector2 other)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs((int)coords.x - (int)other.x), Mathf.Abs((int)coords.y - (int)other.y));

        int lowest = Mathf.Min(dist.x, dist.y);
        int highest = Mathf.Max(dist.x, dist.y);

        int horizontalMovesRequired = highest - lowest;

        return lowest * 14 + horizontalMovesRequired * 10;
    }
}
