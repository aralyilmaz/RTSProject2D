using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform healthBarTransform;
    private Vector3 scale = Vector3.one;

    public void SetSize(float sizeNormalized)
    {
        if(sizeNormalized >= 0 && sizeNormalized <= 1)
        {
            scale.x = sizeNormalized;
            healthBarTransform.localScale = scale;
        }
    }

    public void SetHealthBarVisible(bool visible)
    {
        healthBarTransform.gameObject.SetActive(visible);
    }

}
