using UnityEngine;

public class PlacementHandler : MonoBehaviour
{
    public bool placed { get; private set; }
    public BoundsInt area;
    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(this.area.size.x, this.area.size.y, 0) * 0.5f;
        if(area.size.z != 1)
        {
            Debug.Log("Set z to 1");
        }
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
