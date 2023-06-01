using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class GridBaseBuildingSystem : MonoBehaviour
{
    private GridMapManager grid;

    [SerializeField]
    private BuildingObject buildingObject;

    public GameObject whiteTile;
    public GameObject redTile;
    public GameObject greenTile;
    private GameObject[,] tileArray;

    [SerializeField]
    Transform tiles;

    void Start()
    {
        grid = GridMapManager.instance;
        tileArray = new GameObject[grid.width, grid.height];
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        PlaceBuilding();
    //    }
    //}

    public void SetBuilding(BuildingObject building)
    {
        this.buildingObject = building;
    }

    public bool CheckPlacement(List<Vector2Int> gridPositionList)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (grid.gridMap.GetValue(gridPosition.x, gridPosition.y) != 0)
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
            grid.gridMap.GetXY(MouseRTSController.instance.mouseWorldPosition, out x, out y);

            List<Vector2Int> gridPositionList = buildingObject.GetGridPositionList(new Vector2Int(x, y));

            Vector3 position = grid.gridMap.GetWorldPosition(x, y);

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
                    grid.gridMap.SetValue(gridPosition.x, gridPosition.y, 1);
                    //SetTileColor(gridPosition.x, gridPosition.y, 1);
                }
                return true;
            }
        }
        return false;
    }

    public void SetTileColor(int x, int y, int color)
    {
        switch (color)
        {
            case 0:
                //White
                PlaceTile(x, y, whiteTile);
                break;

            case 1:
                //Red
                //DestroyOldTile();
                PlaceTile(x, y, redTile);
                break;

            case 2:
                //Green
                PlaceTile(x, y, greenTile);
                break;

            default:
                if(tileArray[x, y] == null)
                {
                    Destroy(tileArray[x, y]);
                }
                break;
        }
    }

    private void PlaceTile(int x, int y, GameObject tile)
    {
        if (tile != null)
        {
            if (tileArray[x, y] == null)
            {
                Vector3 position = grid.gridMap.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
                tileArray[x, y] = Instantiate(tile, position, Quaternion.identity, tiles);
                Destroy(tileArray[x, y], 0.2f);
            }
            else
            {
                Vector3 position = grid.gridMap.GetWorldPosition(x, y) + (Vector3.one * 0.5f);
                Destroy(tileArray[x, y]);
                tileArray[x, y] = Instantiate(tile, position, Quaternion.identity, tiles);
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
