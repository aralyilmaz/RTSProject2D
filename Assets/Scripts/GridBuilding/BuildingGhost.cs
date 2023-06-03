using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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

    [SerializeField]
    private GridBaseBuildingSystem buildingSystem;

    private GridMapManager grid;
    private Transform visual;
    private BuildingObject building;

    private ScrollViewItem button;

    void Start()
    {
        grid = GridMapManager.instance;
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
                buildingSystem.SetBuilding(building);
                if (buildingSystem.PlaceBuilding())
                {
                    DestroyVisual();
                    button.EnableDisableButton();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyVisual();
            button.EnableDisableButton();
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
            button.EnableDisableButton();
        }
    }

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

    public void SetButton(ScrollViewItem button)
    {
        this.button = button;
    }

    public void FollowMouse()
    {
        if (visual != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            grid.gridMap.GetXY(MouseRTSController.instance.mouseWorldPosition, out int x, out int y);
            Vector3 targetPosition = grid.gridMap.GetWorldPosition(x, y);
            visual.position = Vector3.Lerp(visual.position, targetPosition, Time.deltaTime * 10f);

            //get points to check
            List<Vector2Int> gridPositionList = building.GetGridPositionList(new Vector2Int(x, y));

            //if any point not suitable show red
            if (!buildingSystem.CheckPlacement(gridPositionList))
            {
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    buildingSystem.SetTileColor(gridPosition.x, gridPosition.y, 1);
                }
            }
        }
    }
}
