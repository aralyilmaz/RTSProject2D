using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool placed { get; private set; }
    public BoundsInt area;
    private Vector3 offset;
    
    void Start()
    {
        offset = new Vector3(this.area.size.x, this.area.size.y, 0) * 0.5f;
    }

    public bool CanBePlaced()
    {
        Vector3Int pos = GridBuildingSystem.instance.gridLayout.LocalToCell(transform.position - offset);
        BoundsInt areaTemp = area;
        areaTemp.position = pos;

        if (GridBuildingSystem.instance.IsAreaEmpty(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Place()
    {
        Vector3Int pos = GridBuildingSystem.instance.gridLayout.LocalToCell(transform.position - offset);
        BoundsInt areaTemp = area;
        areaTemp.position = pos;
        placed = true;
        GridBuildingSystem.instance.TakeArea(area);
    }
}
