using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InformationMenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI menuNameText;

    [SerializeField]
    private TextMeshProUGUI statText;

    [SerializeField]
    private Image menuImage;

    [SerializeField]
    private GameObject informationMenu;

    [SerializeField]
    private GameObject productionMenu;

    [SerializeField]
    private Button destroyButton;

    [SerializeField]
    private Button[] productionButtons;

    private void Start()
    {
        informationMenu = this.gameObject;
        informationMenu.SetActive(false);

        MouseRTSController.instance.OnInteractableSelected += HandleInteractableSelected;
    }

    public void CloseInformationMenu()
    {
        destroyButton.onClick.RemoveAllListeners();
        informationMenu.SetActive(false);
    }

    public void HandleInteractableSelected(object sender, MouseRTSController.OnInteractableSelectedEventArgs eventArgs)
    {
        if (eventArgs.interactable != null)
        {
            UpdateInformationMenu(eventArgs.interactable);
        }
        else
        {
            CloseInformationMenu();
        }
    }

    public void UpdateInformationMenu(Interactable interactable)
    {
        //If any interactable selected activate information menu
        destroyButton.onClick.RemoveAllListeners();
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
        productionMenu.SetActive(false);

        menuNameText.text = building.buildingObject.name;
        menuImage.sprite = building.buildingObject.icon;

        //statText.text = "HP: " + building.buildingObject.health.ToString() + "/" + building.health.ToString();
        statText.text = "HP: " + building.health.ToString() + "/" + building.buildingObject.health.ToString();

        InitDestroyBuildingButton(destroyButton, building);

        if (building.buildingObject.haveProducts)
        {
            productionMenu.SetActive(true);
            SetProductionButtons(building.buildingObject.productCount, building);
        }
        else
        {
            SetProductionButtons(0, building);
            productionMenu.SetActive(false);
        }
    }

    private void SetProductionButtons(int buttonCount, Building building)
    {
        for (int i = 0; i < productionButtons.Length; i++)
        {
            productionButtons[i].enabled = false;
            productionButtons[i].gameObject.SetActive(false);
            productionButtons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < buttonCount; i++)
        {
            productionButtons[i].enabled = true;
            productionButtons[i].gameObject.SetActive(true);
            InitProductionButton(productionButtons[i], i, building);
        }
    }

    private void InitProductionButton(Button button, int buttonNo, Building building)
    {
        if (building.buildingObject.products[buttonNo].icon != null)
        {
            button.image.sprite = building.buildingObject.products[buttonNo].icon;
        }

        if (building.buildingObject.products[buttonNo].name != null)
        {
            TextMeshProUGUI text;
            text = button.GetComponentInChildren<TextMeshProUGUI>();
            if(text != null)
            {
                text.text = building.buildingObject.products[buttonNo].name;
            }
        }

        button.onClick.AddListener(() =>ProductionButton(buttonNo, building));
    }

    private void ProductionButton(int buttonNo, Building building)
    {
        building.ProductionButtons(buttonNo);
    }

    private void InitDestroyBuildingButton(Button button, Building building)
    {
        destroyButton.onClick.AddListener(() => DestroyBuildingButton(building));
    }

    private void InitDestroySoldierButton(Button button, Soldier soldier)
    {
        destroyButton.onClick.AddListener(() => DestroySoldierButton(soldier));
    }

    private void DestroyBuildingButton(Building building)
    {
        building.Die();
        CloseInformationMenu();
    }

    private void DestroySoldierButton(Soldier soldier)
    {
        soldier.Die();
        CloseInformationMenu();
    }

    private void SetSoldierInformation(Soldier soldier)
    {
        destroyButton.onClick.RemoveAllListeners();
        productionMenu.SetActive(false);

        menuNameText.text = soldier.soldierObject.name;
        menuImage.sprite = soldier.soldierObject.icon;

        string stats = "HP: " + soldier.soldierObject.health.ToString() + "/" + soldier.health.ToString() + "\n" +
            "DMG: " + soldier.soldierObject.damage.ToString();

        statText.text = stats;

        InitDestroySoldierButton(destroyButton, soldier);
    }

}
