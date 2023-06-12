using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //for list.any()
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

    //For using pathfinding with world position
    public List<NodeBase> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        //Debug.Log("Vector3start: " + startPosition + " target: " + targetPosition);
        return ConvertPositionToGrid(startPosition, targetPosition);
    }

    private List<NodeBase> ConvertPositionToGrid(Vector3 startPosition, Vector3 targetPosition)
    {
        GridUtility.GetXY(startPosition, out int xStart, out int yStart);
        GridUtility.GetXY(targetPosition, out int xTarget, out int yTarget);

        Vector2 startGrid = new Vector2(xStart, yStart);
        Vector2 targetGrid = new Vector2(xTarget, yTarget);

        //Debug.Log("XYstart: " + startGrid + " target: " + targetGrid);

        return FindPath(startGrid, targetGrid);
    }

    //For using pathfinding with grid position
    public List<NodeBase> FindPath(Vector2 startGrid, Vector2 targetGrid)
    {
        NodeBase startNode = GridMapManager.instance.GetNodeAtPosition(startGrid);
        NodeBase targetNode = GridMapManager.instance.GetNodeAtPosition(targetGrid);
        if(startNode != null && targetNode != null)
        {
            return FindPath(startNode, targetNode);
        }
        return null;
    }

    //A*
    public List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
    {
        List<NodeBase> nodesToSearch = new List<NodeBase>() { startNode };
        List<NodeBase> searched = new List<NodeBase>();

        while (nodesToSearch.Any())
        {
            //find current best F cost node
            NodeBase currentNode = nodesToSearch[0];
            foreach (NodeBase node in nodesToSearch)
            {
                if (node.F < currentNode.F || node.F == currentNode.F && node.H < currentNode.H)
                {
                    currentNode = node;
                }
            }

            searched.Add(currentNode);
            nodesToSearch.Remove(currentNode);

            if (currentNode == targetNode)
            {
                NodeBase currentPathTile = targetNode;
                List<NodeBase> path = new List<NodeBase>();
                int count = 100;
                while (currentPathTile != startNode)
                {
                    //when path found retrace the path and add it to list
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.connection;
                    count--;
                    if (count < 0) throw new Exception();
                }

                //Debug.Log("THEPATH:" + path.Count);
                return path;
            }

            //Loop all walkable and not searched neighbor nodes
            foreach (var neighbor in currentNode.neighbors.Where(t => t.walkable && !searched.Contains(t)))
            {
                bool inSearch = nodesToSearch.Contains(neighbor);

                float costToNeighbor = currentNode.G + currentNode.GetDistance(neighbor.coords);

                //if not searched then search or if found a better path to node update the G cost
                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(currentNode);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode.coords));
                        nodesToSearch.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }
}
