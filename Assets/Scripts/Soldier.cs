using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnitMotor))]
public class Soldier : Interactable
{
    private Vector3 offset;

    [SerializeField]
    private Transform graphics;

    private UnitObjects soldierObject;

    public float health;
    public float damage;

    [SerializeField]
    private GameObject selectedGameObject;

    private GridMapManager gridManager;
    private UnitMotor motor;
    private List<NodeBase> pathList;
    //private bool isMoving = false;

    private void Start()
    {
        SetSelectedVisible(false);
        motor = GetComponent<UnitMotor>();
        pathList = new List<NodeBase>();
    }

    private void Update()
    {
        SetSelectedVisible(isFocus);

        if (Input.GetMouseButtonDown(1) && isFocus)
        {
            //Stop focusing any objects
            MoveSoldier(MouseRTSController.instance.mouseWorldPosition);
        }

        MoveToPath();
    }

    public override void Interact()
    {
        base.Interact();
    }

    public void InitSoldier(UnitObjects soldierObject)
    {
        gridManager = GridMapManager.instance;
        health = soldierObject.health;
        damage = soldierObject.damage;
        placed = false;

        if (soldierObject != null)
        {
            offset = new Vector3(1f, 1f, 0) * 0.5f;

            //Adjust building graphics
            graphics.localPosition = graphics.localPosition + offset;

            this.soldierObject = soldierObject;
            if (TryGetComponent<CircleCollider2D>(out CircleCollider2D soldierCollider))
            {
                //Adjust building collider
                soldierCollider.offset = offset;
            }
            selectedGameObject.transform.localPosition = graphics.localPosition;
        }
    }

    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }

    public void MoveSoldier(Vector3 mousePosition)
    {
        pathList = null;
        //pathList = PathfindingManager.instance.FindPath(transform.position, mousePosition);
        pathList = PathfindingManager.instance.FindPath(graphics.transform.position, mousePosition);
    }

    private void MoveToPath()
    {
        if (pathList == null || (pathList != null && pathList.Count <= 0))
        {
            return;
        }
        int remainingPath = pathList.Count;
        //isMoving = true;
        NodeBase currPath = pathList[pathList.Count - 1];

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = gridManager.gridMap.GetWorldPosition((int)currPath.coords.x, (int)currPath.coords.y);

        //gridManager.gridMap.GetXY(currentPosition, out int x, out int y);
        //Debug.Log(x + "," + y + " - - - " + Time.time);

        float distanceCheck = 0.05f;
        
        if (Vector3.Distance(currentPosition, targetPosition) < distanceCheck)
        {
            //NextPath
            pathList.RemoveAt(remainingPath - 1);

            if(pathList.Count == 0)
            {
                pathList = null;
            }
        }
        motor.SetTargetPosition(targetPosition);
    }
}
