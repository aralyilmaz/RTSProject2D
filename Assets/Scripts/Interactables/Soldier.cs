using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMotor), typeof(HealthBar))]
public class Soldier : Interactable
{
    private Vector3 offset;

    [SerializeField]
    private Transform graphics;

    public UnitObjects soldierObject;

    public float health;
    public float damage;

    [SerializeField]
    private GameObject selectedGameObject;

    private GridMapManager gridManager;
    private UnitMotor motor;
    private List<NodeBase> pathList;
    private bool isMoving = false;

    private bool isAttacking = false;
    List<Vector2Int> targetNeighbors;
    Interactable attackTarget = null;
    private float attackCooldown;
    private Coroutine attackCoroutine;
    private bool isAttackCoroutineRunning = false;

    [SerializeField] private SpriteRenderer unitRenderer;

    [SerializeField] private HealthBar healthBar;

    private bool hasOrder = false;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
        motor = GetComponent<UnitMotor>();
        pathList = new List<NodeBase>();
    }

    private void Update()
    {
        if (attackTarget == null)
        {
            if (isAttackCoroutineRunning)
            {
                isAttackCoroutineRunning = false;
                StopCoroutine(attackCoroutine);
            }
        }

        SetSelectedVisible(isFocus);

        //Right mouse button pressed
        if (Input.GetMouseButtonDown(1) && isFocus)
        {
            Vector3 mouseWorldPosition = MouseRTSController.instance.mouseWorldPosition;
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<Interactable>(out Interactable attackTarget))
                {
                    isAttacking = true;
                    AttackOrder(attackTarget);
                }
            }
            else
            {
                isAttacking = false;
                if (isAttackCoroutineRunning)
                {
                    isAttackCoroutineRunning = false;
                    StopCoroutine(attackCoroutine);
                }
                MoveOrder(mouseWorldPosition);
            }
        }
        DoOrder();
    }

    public override Vector2Int GetGridPosition()
    {
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public override List<Vector2Int> GetNeighbors()
    {
        GridMapManager.instance.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        return GridMapManager.instance.GetObjectNeighbors(new Vector2Int(x, y), width, height);
    }

    public override void TakeDamage(float damage)
    {
        health = health - damage;
        healthBar.SetHealthBarVisible(true);
        healthBar.SetSize(health / soldierObject.health);
        if (health <= 0)
        {
            //Debug.Log("Die" + this.name);
            Die();
        }
    }

    public void InitSoldier(UnitObjects soldierObject)
    {
        gridManager = GridMapManager.instance;
        health = soldierObject.health;
        damage = soldierObject.damage;
        placed = false;

        width = 1;
        height = 1;

        attackCooldown = 1f;

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

            if(soldierObject.icon != null)
            {
                unitRenderer.sprite = soldierObject.icon;
            }

            SetStandingOnTile(1);
            SetStandingOnNodeWalkable(false);

            SetSelectedVisible(false);
        }
    }

    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }

    private void DoOrder()
    {
        if (hasOrder)
        {
            if (isAttacking)
            {
                AttackToObject();
            }
            else
            {
                MoveToPath();
            }
        }
    }

    public void MoveOrder(Vector3 mousePosition)
    {
        hasOrder = true;
        pathList = null;
        //find the shortest path with A*
        pathList = PathfindingManager.instance.FindPath(graphics.transform.position, mousePosition);
        if(pathList == null)
        {
            Debug.Log("List null");
        }
    }

    public void MoveOrder(Vector2Int gridPosition)
    {
        pathList = null;
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        //find the shortest path with A*
        pathList = PathfindingManager.instance.FindPath(new Vector2(x, y), gridPosition);
    }

    public void MoveOrder(NodeBase targetNode)
    {
        pathList = null;
        //find the shortest path with A*
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        NodeBase node = gridManager.GetTileAtPosition(new Vector2(x, y));
        if (node != null)
        {
            pathList = PathfindingManager.instance.FindPath(node, targetNode);
        }

        if (!(pathList == null || (pathList != null && pathList.Count <= 0)))
        {
            isMoving = true;
        }
    }

    private void MoveToPath()
    {
        //return if no path found
        if (pathList == null || (pathList != null && pathList.Count <= 0))
        {
            return;
        }

        //Start moving
        int remainingPath = pathList.Count;
        isMoving = true;
        SetStandingOnTile(0);
        SetStandingOnNodeWalkable(true);
        NodeBase currPath = pathList[pathList.Count - 1];
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = gridManager.gridMap.GetWorldPosition((int)currPath.coords.x, (int)currPath.coords.y);

        float distanceCheck = 0.05f;
        
        if (Vector3.Distance(currentPosition, targetPosition) < distanceCheck)
        {
            //Continue Path
            pathList.RemoveAt(remainingPath - 1);

            if(pathList.Count == 0)
            {
                pathList = null;
                SetStandingOnTile(1);
                SetStandingOnNodeWalkable(false);
                isMoving = false;
            }
        }
        motor.SetTargetPosition(targetPosition);
    }

    private void SetStandingOnNodeWalkable(bool walkable)
    {
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        NodeBase node = gridManager.GetTileAtPosition(new Vector2(x, y));
        if(node != null)
        {
            node.walkable = walkable;
        }
    }

    private Vector2 GetStandingOnTile()
    {
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        return new Vector2(x, y);
    }

    private void SetStandingOnTile(int value)
    {
        gridManager.gridMap.SetValue(graphics.transform.position, value);
    }

    private void AttackOrder(Interactable attackTarget)
    {
        hasOrder = true;
        this.attackTarget = attackTarget;
        //Get neighbor grids of attack target
        targetNeighbors = attackTarget.GetNeighbors();
    }

    private void AttackToObject()
    {
        if (targetNeighbors == null || (targetNeighbors != null && targetNeighbors.Count <= 0))
        {
            return;
        }

        //Check if in attack range and not moving
        if (!isMoving && CheckAttackRange())
        {
            //in range then attack
            if (!isAttackCoroutineRunning)
            {
                isAttackCoroutineRunning = true;
                attackCoroutine = StartCoroutine(Attack(attackCooldown));
            }
        }
        else
        {
            //if not moving try moving in range
            if (isMoving)
            {
                MoveToPath();
            }
            else
            {
                //give order to move
                foreach (Vector2Int neighbor in targetNeighbors)
                {
                    NodeBase targetNode = gridManager.GetTileAtPosition(neighbor);
                    if (targetNode != null && targetNode.walkable)
                    {
                        MoveOrder(targetNode);
                        return;
                    }
                }
            }
        }

    }

    private bool CheckAttackRange()
    {
        gridManager.gridMap.GetXY(graphics.transform.position, out int x, out int y);
        Vector2 myPosition = new Vector2Int(x, y);

        foreach (Vector2Int neighbor in targetNeighbors)
        {
            if (neighbor == myPosition)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator Attack(float attackCooldown)
    {
        while (isAttackCoroutineRunning)
        {
            yield return new WaitForSeconds(attackCooldown);
            if(attackTarget != null)
            {
                attackTarget.TakeDamage(damage);
            }
        }
    }

    public void Die()
    {
        hasOrder = false;
        SetStandingOnTile(0);
        SetStandingOnNodeWalkable(true);
        healthBar.SetHealthBarVisible(false);
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
}
