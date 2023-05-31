using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ScrollViewItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonText;

    private Button button;

    public GameObject objectToSpawn;

    public void InitItemButton(Sprite image, string text, GameObject gameObject)
    {
        button = GetComponent<Button>();

        if(image != null)
        {
            button.image.sprite = image;
        }

        if (text != null)
        {
            buttonText.text = text;
        }

        if(gameObject != null)
        {
            objectToSpawn = gameObject;
        }

        button.onClick.AddListener(InitBuilding);
    }

    public void InitBuilding()
    {
        if (objectToSpawn != null)
        {
            GridBuildingSystem.instance.InitBuilding(objectToSpawn, this);
        }
    }

    public void EnableDisableButton()
    {
        button.enabled = !button.enabled;
    }

}
