using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationMenuManager : MonoBehaviour
{
    MouseRTSController rtsController;

    [SerializeField]
    private TextMeshProUGUI menuNameText;

    [SerializeField]
    private Image menuImage;

    [SerializeField]
    private GameObject InformationMenu;

    private void Start()
    {
        rtsController = MouseRTSController.instance;
        rtsController.onInteractableClickedCallBack += UpdateInformationMenu;

        InformationMenu = this.gameObject;
    }

    public void UpdateInformationMenu()
    {


        if(rtsController.interactableList.Count != 0 && rtsController.interactableList[0].placed)
        {
            InformationMenu.SetActive(true);
        }
        else
        {
            InformationMenu.SetActive(false);
        }

    }

    private void SetBuildingInformation()
    {

    }

    private void SetSoldierInformation()
    {

    }

    private void SetName(string name)
    {
        menuNameText.text = name;
    }

    private void SetImage(Sprite image)
    {
        menuImage.sprite = image;
    }

    private void SetProduction()
    {

    }

}
