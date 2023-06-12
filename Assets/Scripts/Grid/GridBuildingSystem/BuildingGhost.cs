using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingGhost : MonoBehaviour
{
    public static BuildingGhost instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of BuildingGhost found!");
            return;
        }
        instance = this;
    }

    [SerializeField] private GridBaseBuildingSystem buildingSystem;

    private Transform visual;
    private BuildingObject building;

    [SerializeField] private DynamicScrollView buttons;

    void Start()
    {
        DestroyVisual();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (visual != null)
            {
                //try placing building
                buildingSystem.SetBuilding(building);
                if (buildingSystem.PlaceBuilding())
                {
                    DestroyVisual();
                    buttons.EnableDisableAllButtons(true);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if esc pressed cancel building
            DestroyVisual();
            buttons.EnableDisableAllButtons(true);
        }
    }

    void LateUpdate()
    {
        FollowMouse();
    }

    public void CreateVisual()
    {
        DestroyVisual();

        if (building != null && building.prefab != null)
        {
            visual = Instantiate(building.prefab, Vector3.zero, Quaternion.identity);
            if(visual.TryGetComponent<Building>(out Building buildingInteractable))
            {
                buildingInteractable.InitBuilding(building);
            }
            buttons.EnableDisableAllButtons(false); //disable buttons until player places building or cancel placement
        }
    }

    //Destroys builidng ghost that follows mouse position
    public void DestroyVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
    }

    public void SetBuilding(BuildingObject building)
    {
        this.building = building;
    }

    public void FollowMouse()
    {
        if (visual != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            GridUtility.GetXY(MouseRTSController.instance.mouseWorldPosition, out int x, out int y);
            Vector3 targetPosition = GridUtility.GetWorldPosition(x, y); //get grid position that mouse standing on
            visual.position = Vector3.Lerp(visual.position, targetPosition, Time.deltaTime * 10f); //slide building ghost in place

            //get points to check
            List<Vector2Int> gridPositionList = building.GetGridPositionList(new Vector2Int(x, y));

            //if any point not suitable show red
            if (!buildingSystem.CheckPlacement(gridPositionList))
            {
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    buildingSystem.SetTileColorRed(gridPosition.x, gridPosition.y);
                }
            }
        }
    }
}
