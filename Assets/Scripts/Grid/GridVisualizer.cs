using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tile gridTilePrefab;

    [SerializeField]
    private Transform tiles;

    [SerializeField]
    GridMapManager gridManager;

    private Vector3 offset = Vector3.one * 0.5f;

    private void Start()
    {
        offset.z = 0f;
    }

    public void VisualizeGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(gridTilePrefab, gridManager.gridMap.GetWorldPosition(x, y) + offset, Quaternion.identity, tiles);

                //offset means give slightly different color to tile
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.InitTile(isOffset);
            }
        }
    }
}
