using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitObject", menuName = "Objects/UnitObject")]
public class UnitObjects : ScriptableObject
{
    new public string name = "NewUnitObject";
    public Sprite icon = null;
    public float health = 10f;
    public float damage = 5f;
    public Color color = Color.white;

    //public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    //{
    //    List<Vector2Int> gridPositionList = new List<Vector2Int>();

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            gridPositionList.Add(offset + new Vector2Int(x, y));
    //        }
    //    }

    //    return gridPositionList;
    //}
}
