using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBaseBuildingSystem : MonoBehaviour
{
    private GridMapManager gridManager;

    [SerializeField]
    private BuildingObject buildingObject;

    public GameObject redTile;
    private GameObject[,] tileArray;

    void Start()
    {
        gridManager = GridMapManager.instance;
        tileArray = new GameObject[gridManager.width, gridManager.height];
    }

    public void SetBuilding(BuildingObject building)
    {
        this.buildingObject = building;
    }

    public bool CheckPlacement(List<Vector2Int> gridPositionList)
    {
        //grid value 0 means grid is suitable for placement
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (gridManager.gridMap.GetValue(gridPosition.x, gridPosition.y) != 0)
            {
                return false;
            }
        }
        return true;
    }

    public bool PlaceBuilding()
    {
        if (buildingObject != null)
        {
            int x, y;
            gridManager.gridMap.GetXY(MouseRTSController.instance.mouseWorldPosition, out x, out y);

            List<Vector2Int> gridPositionList = buildingObject.GetGridPositionList(new Vector2Int(x, y));

            Vector3 position = gridManager.gridMap.GetWorldPosition(x, y);

            //if it is ok to place than place building
            if (CheckPlacement(gridPositionList))
            {
                Transform placedBuilding = Instantiate(buildingObject.prefab, position, Quaternion.identity);

                if (placedBuilding.TryGetComponent<Building>(out Building buildingInteractable))
                {
                    //delay after placing to prevent interacting directly
                    StartCoroutine(SetPlaced(buildingInteractable, 0.5f));
                    buildingInteractable.InitBuilding(buildingObject);
                }
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    gridManager.gridMap.SetValue(gridPosition.x, gridPosition.y, 1); //for building
                    gridManager.GetNodeAtPosition(gridPosition).walkable = false; //for pathfindig
                }
                return true;
            }
        }
        return false;
    }

    public void SetTileColorRed(int x, int y)
    {
        PlaceTile(x, y, redTile);
    }

    private void PlaceTile(int x, int y, GameObject tile)
    {
        if (tile != null && x >= 0 && y >= 0 && x < gridManager.width && y < gridManager.height)
        {
            if (tileArray[x, y] == null)
            {
                Vector3 position = gridManager.gridMap.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
                tileArray[x, y] = Instantiate(tile, position, Quaternion.identity);
                Destroy(tileArray[x, y], 0.2f);
            }
            else
            {
                Vector3 position = gridManager.gridMap.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
                Destroy(tileArray[x, y]);
                tileArray[x, y] = Instantiate(tile, position, Quaternion.identity);
                Destroy(tileArray[x, y], 0.2f);
            }
        }
    }

    private IEnumerator SetPlaced(Building building, float delay)
    {
        yield return new WaitForSeconds(delay);
        building.placed = true;
    }
}
