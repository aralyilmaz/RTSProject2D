using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(Button))]
public class ScrollViewItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonText;

    private Button button;

    public GameObject objectToSpawn;

    BuildingObject building;

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

    public void InitItemButton(BuildingObject building)
    {
        {
            this.building = building;

            button = GetComponent<Button>();

            if (building.icon != null)
            {
                button.image.sprite = building.icon;
            }

            if (building.name != null)
            {
                buttonText.text = building.name;
            }

            //if (gameObject != null)
            //{
            //    objectToSpawn = gameObject;
            //}

            button.onClick.AddListener(InitBuilding);
        }
    }

    private void InitBuilding()
    {
        //if (objectToSpawn != null)
        //{
        //    //GridBuildingSystem.instance.InitBuilding(objectToSpawn, this);
        //}
        BuildingGhost.instance.SetButton(this);
        BuildingGhost.instance.SetBuilding(building);
        BuildingGhost.instance.CreateVisual();
    }

    public void EnableDisableButton()
    {
        button.enabled = !button.enabled;
    }

}
