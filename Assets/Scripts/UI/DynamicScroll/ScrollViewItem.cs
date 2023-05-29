using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ScrollViewItem : MonoBehaviour
{
    [SerializeField]
    private Image buttonImage;

    [SerializeField]
    private TextMeshProUGUI buttonText;

    public void InitItem(Sprite image, string text)
    {
        if(image != null)
        {
            buttonImage.sprite = image;
        }

        if (text != null)
        {
            buttonText.text = text;
        }
    }

}
