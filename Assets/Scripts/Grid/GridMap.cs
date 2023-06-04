using System.Collections;
using UnityEngine;

public class GridMap
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;

    public GridMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = 0;
                //string text = x.ToString() + "," + y.ToString();
                //debugTextArray[x, y] = CreateWorldText(gridArray[x, y].ToString(), GetWorldPosition(x, y) + new Vector3(cellSize, cellSize, 0) * 0.5f);
                //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        //Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        //Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (debugTextArray[x, y] != null)
            {
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    //function used for debug that puts number in tiles
    private TextMesh CreateWorldText(string text, Vector3 localPosition)
    {
        GameObject textObject = new GameObject("WorldText", typeof(TextMesh));
        Transform transform = textObject.transform;
        //transform.SetParent(null, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = textObject.GetComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Left;
        textMesh.text = text;
        textMesh.fontSize = 5;
        textMesh.color = Color.white;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 5000;
        return textMesh;
    }
}
