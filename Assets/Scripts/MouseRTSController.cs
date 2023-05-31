using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRTSController : MonoBehaviour
{

    public static MouseRTSController instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of MouseRTSController found!");
            return;
        }
        instance = this;
    }

    public Vector3 mouseWorldPosition;

    private Camera mainCam;

    [SerializeField]
    private Vector3 startPosition;

    public List<Interactable> interactableList;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMouseWorldPosition(Input.mousePosition);

        //Left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            //Stop focusing any objects
            RemoveFocus();
            startPosition = mouseWorldPosition;
        }

        //Left mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, mouseWorldPosition);

            //Check if hit interactable
            foreach(Collider2D collider2D in collider2DArray)
            {
                //Debug.Log(collider2D.name);
                if(collider2D.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    interactableList.Add(interactable);
                }
            }
            SetFocus(interactableList);
        }
    }

    private void CalculateMouseWorldPosition(Vector3 mouseScreenPosition)
    {
        mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void SetFocus(List<Interactable> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].OnFocused();
        }
    }

    void RemoveFocus()
    {
        for (int i = 0; i < interactableList.Count; i++)
        {
            interactableList[i].OnDefocused();
            interactableList.RemoveAt(i);
        }
    }
}
