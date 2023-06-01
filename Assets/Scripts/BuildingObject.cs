using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingObject", menuName = "Objects/BuildingObject")]
public class BuildingObject : ScriptableObject
{
    new public string name = "NewBuildingObject";
    public Sprite icon = null;
    public float health = 10f;
    public int width;
    public int height;
    public Color color = Color.white;
    public Transform prefab = null;

    public bool haveProducts = false;
    public int productCount = 0;
    public List<UnitObjects> products;
    public Transform productPrefab;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset + new Vector2Int(x, y));
            }
        }

        return gridPositionList;
    }
}
