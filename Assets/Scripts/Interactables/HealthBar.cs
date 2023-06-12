using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float maxHealth;

    [SerializeField] private Transform healthBarTransform;
    private Vector3 scale = Vector3.one;

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    private void SetSize(float sizeNormalized)
    {
        if(sizeNormalized >= 0 && sizeNormalized <= 1)
        {
            scale.x = sizeNormalized;
            healthBarTransform.localScale = scale;
        }
    }

    public void UpdateHealthBar(float currentHealth)
    {
        SetSize(currentHealth / maxHealth);
    }

    public void SetHealthBarVisible(bool visible)
    {
        healthBarTransform.parent.gameObject.SetActive(visible);
    }

}
