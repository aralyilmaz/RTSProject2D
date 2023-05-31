using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : MonoBehaviour
{
    public static GridMapManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GridMapManager found!");
            return;
        }
        instance = this;
    }

    public GridMap gridMap;

    public int width = 15;
    public int height = 10;
    public float cellSize = 1f;
    public Vector3 originPosition = new Vector3(-7, -5, 0);

    void Start()
    {
        gridMap = new GridMap(width, height, cellSize, originPosition);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //grid.SetValue(MouseRTSController.instance.mouseWorldPosition, 1);
        }
    }
}
