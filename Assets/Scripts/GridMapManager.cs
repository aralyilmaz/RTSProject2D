using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : MonoBehaviour
{
    private GridMap grid;

    void Start()
    {
        grid = new GridMap(4, 2, 1f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(MouseRTSController.instance.mouseWorldPosition, 1);
        }
    }
}
