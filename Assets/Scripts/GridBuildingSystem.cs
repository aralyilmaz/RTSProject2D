using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GridBuildingSystem found!");
            return;
        }
        instance = this;
    }

    public Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    public GridLayout gridLayout;

    //used for placed buildings
    public Tilemap mainTileMap;

    //used for showing indications
    public Tilemap tempTileMap;

    [SerializeField]
    private Building buildingToBePlaced;
    private Vector3 buildingOldPos;
    private BoundsInt oldArea;
    private Vector3 offset;

    public TileBase whiteTile;
    public TileBase greenTile;
    public TileBase redTile;
    public TileBase whiteTileTransparant;
    public TileBase greenTileTransparant;
    public TileBase redTileTransparant;

    private void Start()
    {
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, whiteTile);
        tileBases.Add(TileType.Green, greenTile);
        tileBases.Add(TileType.Red, redTile);
        tileBases.Add(TileType.WhiteTransparant, whiteTileTransparant);
        tileBases.Add(TileType.GreenTransparant, greenTileTransparant);
        tileBases.Add(TileType.RedTransparant, redTileTransparant);
    }

    private void Update()
    {
        if(buildingToBePlaced == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                return;
            }

            if (!buildingToBePlaced.placed)
            {
                Vector3 mousePosition = MouseRTSController.instance.mouseWorldPosition;
                Vector3Int cellPosition = gridLayout.WorldToCell(mousePosition);
                
                if(buildingOldPos != cellPosition)
                {
                    //offset = new Vector3(buildingToBePlaced.area.size.x, buildingToBePlaced.area.size.y, 0) * 0.5f;
                    buildingToBePlaced.transform.position = gridLayout.CellToLocalInterpolated(cellPosition + offset);
                    //buildingToBePlaced.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPosition + new Vector3(0.5f, 0.5f, 0));
                    buildingOldPos = cellPosition;
                    FollowBuilding();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (buildingToBePlaced.CanBePlaced())
            {
                buildingToBePlaced.Place();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearArea();
            Destroy(buildingToBePlaced.gameObject);
        }
    }

    public enum TileType
    {
        Empty,
        White,
        Green,
        Red,
        WhiteTransparant,
        GreenTransparant,
        RedTransparant
    }


    //try unitys method
    //Get tiles one by one and return an array of them
    private TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach(Vector3Int v in area.allPositionsWithin)
        {
            Vector3Int position = new Vector3Int(v.x, v.y, 0);
            tileArray[counter] = tilemap.GetTile(position);
            counter++;
        }

        return tileArray;
    }

    private void FillTiles(TileBase[] tileArray, TileType type)
    {
        for(int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = tileBases[type];
        }
    }

    private void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    public void InitBuilding(GameObject building)
    {
        if (building != null)
        {
            buildingToBePlaced = Instantiate(building, Vector3.one, Quaternion.identity).GetComponent<Building>();
            offset = new Vector3(buildingToBePlaced.area.size.x, buildingToBePlaced.area.size.y, 0) * 0.5f;
            FollowBuilding();
        }
    }

    private void ClearArea()
    {
        TileBase[] areaToClear = new TileBase[oldArea.size.x * oldArea.size.y * oldArea.size.z];
        FillTiles(areaToClear, TileType.Empty);
        tempTileMap.SetTilesBlock(oldArea, areaToClear);
    }

    private void FollowBuilding()
    {
        ClearArea();

        buildingToBePlaced.area.position = gridLayout.WorldToCell(buildingToBePlaced.transform.position - offset);
        BoundsInt buildingArea = buildingToBePlaced.area;

        TileBase[] buildingAreaArray = GetTilesBlock(buildingArea, mainTileMap);

        int size = buildingAreaArray.Length;
        TileBase[] buildingAreaIndicationArray = new TileBase[size];

        for(int i = 0; i < buildingAreaArray.Length; i++)
        {
            if (buildingAreaArray[i] == tileBases[TileType.White])
            {
                buildingAreaIndicationArray[i] = tileBases[TileType.GreenTransparant];
            }
            else
            {
                FillTiles(buildingAreaIndicationArray, TileType.RedTransparant);
                break;
            }
        }
        tempTileMap.SetTilesBlock(buildingArea, buildingAreaIndicationArray);
        oldArea = buildingArea;
    }

    public bool IsAreaEmpty(BoundsInt area)
    {
        TileBase[] tileAreaArray = GetTilesBlock(area, mainTileMap);
        foreach(TileBase tile in tileAreaArray)
        {
            if(tile != tileBases[TileType.White])
            {
                Debug.Log("Cannot place");
                return false;
            }
        }
        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, tempTileMap);
        SetTilesBlock(area, TileType.Green, mainTileMap);
    }

}
