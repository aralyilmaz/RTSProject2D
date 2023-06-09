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

    public override void Interact()
    {
        Debug.Log("Interacting with: " + this.name);
    }

    public override Vector2Int GetGridPosition()
    {
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public override List<Vector2Int> GetNeighbors()
    {
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        return GridMapManager.instance.GetObjectNeighbors(new Vector2Int(x, y), width, height);
    }

    public override void TakeDamage(float damage)
    {
        health = health - damage;
        healthBar.SetHealthBarVisible(true);
        healthBar.UpdateHealthBar(health);
        if (health <= 0)
        {
            //Debug.Log("Die" + this.name);
            Die();
        }
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
                    //if hit interactable than give attack order
                    isAttacking = true;
                    AttackOrder(attackTarget);
                }
            }
            else
            {
                //if not hit interactable than give move order
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

    public void InitSoldier(UnitObjects soldierObject)
    {
        healthBar = GetComponent<HealthBar>();
        motor = GetComponent<UnitMotor>();
        pathList = new List<NodeBase>();

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

            //Adjust unit graphics
            graphics.localPosition = graphics.localPosition + offset;

            this.soldierObject = soldierObject;
            if (TryGetComponent<CircleCollider2D>(out CircleCollider2D soldierCollider))
            {
                //Adjust unit collider
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
            healthBar.SetMaxHealth(soldierObject.health);
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

        //if had a valid path than moving
        if (!(pathList == null || (pathList != null && pathList.Count <= 0)))
        {
            isMoving = true;
        }
    }

    public void MoveOrder(Vector2Int gridPosition)
    {
        pathList = null;
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        //find the shortest path with A*
        pathList = PathfindingManager.instance.FindPath(new Vector2(x, y), gridPosition);

        //if had a valid path than moving
        if (!(pathList == null || (pathList != null && pathList.Count <= 0)))
        {
            isMoving = true;
        }
    }

    public void MoveOrder(NodeBase targetNode)
    {
        pathList = null;
        //find the shortest path with A*
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        NodeBase node = gridManager.GetNodeAtPosition(new Vector2(x, y));
        if (node != null)
        {
            pathList = PathfindingManager.instance.FindPath(node, targetNode);
        }

        //if had a valid path than moving
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
        Vector3 targetPosition = GridUtility.GetWorldPosition((int)currPath.coords.x, (int)currPath.coords.y);

        float distanceCheck = 0.05f;
        
        if (Vector3.Distance(currentPosition, targetPosition) < distanceCheck)
        {
            //Continue Path
            pathList.RemoveAt(remainingPath - 1);

            //path completed
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
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        NodeBase node = gridManager.GetNodeAtPosition(new Vector2(x, y));
        if(node != null)
        {
            node.walkable = walkable;
        }
    }

    private Vector2 GetStandingOnTile()
    {
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
        return new Vector2(x, y);
    }

    private void SetStandingOnTile(int value)
    {
        GridUtility.SetValue(graphics.transform.position, value);
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
                    NodeBase targetNode = gridManager.GetNodeAtPosition(neighbor);
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
        GridUtility.GetXY(graphics.transform.position, out int x, out int y);
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
