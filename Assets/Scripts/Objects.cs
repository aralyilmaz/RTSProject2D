using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Objects/Object")]
public class Objects : ScriptableObject
{
    new public string name = "NewObject";
    public Sprite icon = null;
    public bool placed = false;
    public float health = 10f;
    public float damage = 0f;
    public BoundsInt area;
    public float range = 0f;
    public Color color = Color.white;
}
