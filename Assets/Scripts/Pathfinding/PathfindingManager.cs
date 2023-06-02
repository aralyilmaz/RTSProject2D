using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //for list.any()
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PathfindingManager found!");
            return;
        }
        instance = this;
    }

    GridMapManager gridManager;

    private void Start()
    {
        gridManager = GridMapManager.instance;
    }

    public List<NodeBase> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        //Debug.Log("Vector3start: " + startPosition + " target: " + targetPosition);
        return ConvertPositionToGrid(startPosition, targetPosition);
    }

    private List<NodeBase> ConvertPositionToGrid(Vector3 startPosition, Vector3 targetPosition)
    {
        gridManager.gridMap.GetXY(startPosition, out int xStart, out int yStart);
        gridManager.gridMap.GetXY(targetPosition, out int xTarget, out int yTarget);

        Vector2 startGrid = new Vector2(xStart, yStart);
        Vector2 targetGrid = new Vector2(xTarget, yTarget);

        //Debug.Log("XYstart: " + startGrid + " target: " + targetGrid);

        return FindPath(startGrid, targetGrid);
    }

    public List<NodeBase> FindPath(Vector2 startGrid, Vector2 targetGrid)
    {
        NodeBase startNode = gridManager.GetTileAtPosition(startGrid);
        NodeBase targetNode = gridManager.GetTileAtPosition(targetGrid);
        if(startNode != null && targetNode != null)
        {
            return FindPath(startNode, targetNode);
        }
        return null;
    }

    public List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
    {
        //Debug.Log("start: " + startNode.coords + " target: " + targetNode.coords);
        List<NodeBase> toSearch = new List<NodeBase>() { startNode };
        List<NodeBase> searched = new List<NodeBase>();

        while (toSearch.Any())
        {
            NodeBase currentNode = toSearch[0];
            foreach (NodeBase node in toSearch)
            {
                if (node.F < currentNode.F || node.F == currentNode.F && node.H < currentNode.H)
                {
                    currentNode = node;
                }
            }

            searched.Add(currentNode);
            toSearch.Remove(currentNode);

            if (currentNode == targetNode)
            {
                var currentPathTile = targetNode;
                var path = new List<NodeBase>();
                var count = 100;
                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.connection;
                    count--;
                    if (count < 0) throw new Exception();
                }

                //Debug.Log("THEPATH:" + path.Count);
                return path;
            }

            foreach (var neighbor in currentNode.neighbors.Where(t => t.walkable && !searched.Contains(t)))
            {
                var inSearch = toSearch.Contains(neighbor);

                var costToNeighbor = currentNode.G + currentNode.GetDistance(neighbor.coords);

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(currentNode);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode.coords));
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }
}
