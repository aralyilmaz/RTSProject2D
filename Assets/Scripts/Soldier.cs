using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        SetSelectedVisible(false);
    }

    private void Update()
    {
        SetSelectedVisible(isFocus);
    }

    public override void Interact()
    {
        base.Interact();
    }

    public void InitSoldier(UnitObjects soldierObject)
    {
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
}
