using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer tileRenderer;

    public void InitTile(bool isOffset)
    {
        tileRenderer.color = isOffset ? offsetColor : baseColor;
        tileRenderer.sortingOrder = -2;
    }
}
