using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(HealthBar))]
public class Building : Interactable
{
    public BuildingObject buildingObject;

    [SerializeField]
    Transform graphics;

    [SerializeField]
    TextMeshPro text;

    Vector3 offset;

    public float health;

    public List<Vector2Int> neighbors { get; private set; }
    private Vector3 spawnLocation;

    [SerializeField] private SpriteRenderer gfxRenderer;

    [SerializeField] private HealthBar healthBar;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
    }

    public override Vector2Int GetGridPosition()
    {
        GridMapManager.instance.gridMap.GetXY(transform.position, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public override List<Vector2Int> GetNeighbors()
    {
        return neighbors;
    }

    public override void TakeDamage(float damage)
    {
        health = health - damage;
        healthBar.SetHealthBarVisible(true);
        healthBar.SetSize(health / buildingObject.health);
        if (health <= 0)
        {
            //Debug.Log("Die " + this.name);
            Die();
        }
    }

    //Create building and adjust hitboxes, pivots to make it fit into tiles
    public void InitBuilding(BuildingObject buildingObject)
    {
        if (buildingObject != null)
        {
            this.buildingObject = buildingObject;
            offset = new Vector3(buildingObject.width, buildingObject.height, 0) * 0.5f;

            //Adjust building graphics
            graphics.localScale = new Vector3(buildingObject.width, buildingObject.height, 1);
            graphics.localPosition = graphics.localPosition + offset;

            if (TryGetComponent<BoxCollider2D>(out BoxCollider2D buildingCollider))
            {
                //Adjust building collider
                buildingCollider.size = new Vector2(buildingObject.width, buildingObject.height);
                buildingCollider.offset = offset;
            }
            text.text = buildingObject.name;
            health = buildingObject.health;
            width = buildingObject.width;
            height = buildingObject.height;

            gfxRenderer.sprite = buildingObject.icon;

            CacheNeighbors();
        }
    }

    public void ProductionButtons(int buttonNumber)
    {
        //Check for suitable spawn position
        if (CheckUnitSpawnArea())
        {
            Transform production;
            production = Instantiate(buildingObject.productPrefab, spawnLocation, Quaternion.identity);
            if (production.TryGetComponent<Soldier>(out Soldier soldierInteractable))
            {
                soldierInteractable.InitSoldier(buildingObject.products[buttonNumber]);
                soldierInteractable.placed = true;
            }
        }
    }

    private bool CheckUnitSpawnArea()
    {
        foreach (Vector2Int neighbor in neighbors)
        {
            //grid value 0 means suitable for placement
            if (GridMapManager.instance.gridMap.GetValue(neighbor.x, neighbor.y) == 0)
            {
                spawnLocation = GridMapManager.instance.gridMap.GetWorldPosition(neighbor.x, neighbor.y);
                return true;
            }
        }
        return false;
    }

    private void CacheNeighbors()
    {
        GridMapManager.instance.gridMap.GetXY(transform.position, out int x, out int y);
        neighbors = GridMapManager.instance.GetObjectNeighbors(new Vector2Int(x, y), width, height);
    }

    public void Die()
    {
        healthBar.SetHealthBarVisible(false);

        GridMapManager.instance.gridMap.GetXY(transform.position, out int x, out int y);
        List<Vector2Int> gridPositionList = buildingObject.GetGridPositionList(new Vector2Int(x, y));

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridMapManager.instance.gridMap.SetValue(gridPosition.x, gridPosition.y, 0); //for building
            NodeBase node = GridMapManager.instance.GetNodeAtPosition(gridPosition); //for pathfindig
            if (node != null)
            {
                node.walkable = true; //for pathfindig
            }
        }

        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
}
