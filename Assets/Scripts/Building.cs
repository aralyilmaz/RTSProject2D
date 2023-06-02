using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Building : Interactable
{
    public BuildingObject buildingObject;

    [SerializeField]
    Transform graphics;

    [SerializeField]
    TextMeshPro text;

    Vector3 offset;

    public override void Interact()
    {
        base.Interact();
    }

    //Create building and adjust pivots to make it fit into tiles
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
        }
    }

    public void ProductionButtons(int buttonNumber)
    {
        //Debug.Log(buttonNumber + " --- " + this.transform.position);
        Transform production;
        production = Instantiate(buildingObject.productPrefab, Vector3.zero, Quaternion.identity);
        if (production.TryGetComponent<Soldier>(out Soldier soldierInteractable))
        {
            soldierInteractable.InitSoldier(buildingObject.products[buttonNumber]);
            soldierInteractable.placed = true;
        }
    }

}
