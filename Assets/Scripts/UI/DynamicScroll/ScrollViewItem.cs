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

    BuildingObject buildingObject;

    public void InitItemButton(BuildingObject building)
    {
        this.buildingObject = building;

        button = GetComponent<Button>();

        if (building.icon != null)
        {
            button.image.sprite = building.icon;
        }

        if (building.name != null)
        {
            buttonText.text = building.name;
        }

        button.onClick.AddListener(InitBuilding);
    }

    private void InitBuilding()
    {
        BuildingGhost.instance.SetButton(this);
        BuildingGhost.instance.SetBuilding(buildingObject);
        BuildingGhost.instance.CreateVisual();
    }

    public void EnableDisableButton()
    {
        button.enabled = !button.enabled;
    }

}
