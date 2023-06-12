using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBaseBuildingSystem : MonoBehaviour
{

    [SerializeField]
    private BuildingObject buildingObject;

    public GameObject redTile;
    private GameObject[,] tileArray;

    void Start()
    {
        tileArray = new GameObject[GridMapManager.instance.width, GridMapManager.instance.height];
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
            if (GridUtility.GetValue(gridPosition.x, gridPosition.y) != 0)
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
            GridUtility.GetXY(MouseRTSController.instance.mouseWorldPosition, out x, out y);

            List<Vector2Int> gridPositionList = buildingObject.GetGridPositionList(new Vector2Int(x, y));

            Vector3 position = GridUtility.GetWorldPosition(x, y);

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
                    GridUtility.SetValue(gridPosition.x, gridPosition.y, 1); //for building
                    GridMapManager.instance.GetNodeAtPosition(gridPosition).walkable = false; //for pathfindig
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
        if (tile != null && x >= 0 && y >= 0 && x < GridMapManager.instance.width && y < GridMapManager.instance.height)
        {
            if (tileArray[x, y] == null)
            {
                Vector3 position = GridUtility.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
                tileArray[x, y] = Instantiate(tile, position, Quaternion.identity);
                Destroy(tileArray[x, y], 0.2f);
            }
            else
            {
                Vector3 position = GridUtility.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
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
