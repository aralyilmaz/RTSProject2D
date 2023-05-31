using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Entities/Entity")]
public class Entities : ScriptableObject
{
    new public string name = "NewEntity";
    public Sprite icon = null;
    public bool placed = false;
    public float health = 10f;
    public float damage = 0f;
    public BoundsInt area;
    public float range = 0f;
    public Color color = Color.white;
}
