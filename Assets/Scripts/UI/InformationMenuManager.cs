using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField]
    private TextMeshProUGUI menuNameText;

    [SerializeField]
    private Image menuImage;

    [SerializeField]
    private GameObject InformationMenu;

    public void SetInformationMenu()
    {
        InformationMenu.SetActive(true);
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
