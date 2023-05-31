using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingEntity", menuName = "Entities/BuildingEntity")]
public class BuildingEntity : ScriptableObject
{
    new public string name = "NewBuildingEntity";
    public Sprite icon = null;
    public bool placed = false;
    public float health = 10f;
    public BoundsInt area;
    public Color color = Color.white;
    public bool haveProducts = false;
    public int productCount = 0;
}
