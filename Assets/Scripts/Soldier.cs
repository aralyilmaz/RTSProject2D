using UnityEngine;

public class Soldier : Interactable
{
    public float health;
    public float damage;
    public bool placed;
    public BoundsInt area;

    public override void Interact()
    {
        base.Interact();
    }

    public void InitSoldier()
    {
        health = 10f;
        damage = 5f;
        placed = false;
        area.size = Vector3Int.one;
    }
}
