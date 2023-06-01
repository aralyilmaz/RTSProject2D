using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class InformationMenuManager : MonoBehaviour
{
    public static InformationMenuManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InformationMenuManager found!");
            return;
        }
        instance = this;
    }

    MouseRTSController rtsController;

    [SerializeField]
    private TextMeshProUGUI menuNameText;

    [SerializeField]
    private Image menuImage;

    [SerializeField]
    private GameObject informationMenu;

    [SerializeField]
    private GameObject productionMenu;

    [SerializeField]
    private Button[] productionButtons;

    private Building building;

    private void Start()
    {
        //rtsController = MouseRTSController.instance;
        //rtsController.onInteractableClickedCallBack += UpdateInformationMenu;

        informationMenu = this.gameObject;
        informationMenu.SetActive(false);
    }

    public void CloseInformationMenu()
    {
        informationMenu.SetActive(false);
    }

    public void UpdateInformationMenu()
    {
        //If any interactable selected activate information menu
        if(rtsController.interactableList.Count != 0 && rtsController.interactableList[0].placed)
        {
            informationMenu.SetActive(true);
            if (rtsController.interactableList[0] is Building)
            {
                SetBuildingInformation(rtsController.interactableList[0] as Building);
            }

            if (rtsController.interactableList[0] is Soldier)
            {
                SetSoldierInformation(rtsController.interactableList[0] as Soldier);
            }
        }
        else
        {
            informationMenu.SetActive(false);
        }
    }

    public void UpdateInformationMenu(Interactable interactable)
    {
        //If any interactable selected activate information menu
        if (interactable.placed)
        {
            informationMenu.SetActive(true);
            if (interactable is Building)
            {
                SetBuildingInformation(interactable as Building);
            }

            if (interactable is Soldier)
            {
                SetSoldierInformation(interactable as Soldier);
            }
        }
        else
        {
            informationMenu.SetActive(false);
        }
    }

    private void SetBuildingInformation(Building building)
    {
        if (building != null)
        {
            this.building = building;
            menuNameText.text = building.buildingObject.name;
            menuImage.sprite = building.buildingObject.icon;

            if (building.buildingObject.haveProducts)
            {
                productionMenu.SetActive(true);
                SetProductionButtons(building.buildingObject.productCount);
            }
            else
            {
                SetProductionButtons(0);
                productionMenu.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Information menu building is null");
        }
    }

    private void SetProductionButtons(int buttonCount)
    {
        for (int i = 0; i < productionButtons.Length; i++)
        {
            productionButtons[i].enabled = false;
            productionButtons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < buttonCount; i++)
        {
            productionButtons[i].enabled = true;
            InitProductionButton(productionButtons[i], i);
        }
    }

    private void InitProductionButton(Button button, int buttonNo)
    {
        //if (building.buildingObject.productionIcon != null)
        //{
        //    button.image.sprite = building.icon;
        //}

        //if (building.name != null)
        //{
        //    buttonText.text = building.name;
        //}

        button.onClick.AddListener(() =>ProductionButton(buttonNo));
    }

    private void ProductionButton(int buttonNo)
    {
        building.ProductionButtons(buttonNo);
    }

    private void SetSoldierInformation(Soldier soldier)
    {

    }

}
