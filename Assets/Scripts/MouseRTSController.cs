using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

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

    public Vector3 mouseWorldPosition { get; private set; }

    private Camera mainCam;

    private Vector3 startPosition;

    public List<Interactable> interactableList;

    [SerializeField]
    private Transform selectionAreaTransform;

    public event EventHandler<OnInteractableSelectedEventArgs> OnInteractableSelected;
    public class OnInteractableSelectedEventArgs : EventArgs
    {
        public Interactable interactable;
    }

    void Start()
    {
        mainCam = Camera.main;
        interactableList = new List<Interactable>();
        selectionAreaTransform.gameObject.SetActive(false);
    }

    void Update()
    {
        CalculateMouseWorldPosition(Input.mousePosition);

        //return if mouse on ui
        if (EventSystem.current.IsPointerOverGameObject())
        {
            selectionAreaTransform.gameObject.SetActive(false);
            return;
        }

        //Left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            //Stop focusing any objects
            startPosition = mouseWorldPosition;
            selectionAreaTransform.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            AdjustSelectedArea();
        }

        //Left mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            selectionAreaTransform.gameObject.SetActive(false);

            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, mouseWorldPosition);

            //Deselect all
            RemoveFocus();

            //Check if hit interactable within selection area
            foreach (Collider2D collider2D in collider2DArray)
            {
                //Debug.Log(collider2D.name);
                if(collider2D.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    interactableList.Add(interactable);
                }
            }

            //Select the interactables
            if (interactableList.Count != 0)
            {
                OnInteractableSelected?.Invoke(this, new OnInteractableSelectedEventArgs { interactable = interactableList[0] });
                SetFocus(interactableList);
            }
            else
            {
                OnInteractableSelected?.Invoke(this, new OnInteractableSelectedEventArgs { interactable = null });
            }
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

    private void RemoveFocus()
    {
        for (int i = 0; i < interactableList.Count; i++)
        {
            interactableList[i].OnDefocused();
        }

        interactableList.Clear();
    }

    private void AdjustSelectedArea()
    {
        Vector3 currentMousePosition = mouseWorldPosition;
        Vector3 lowerLeftCorner = new Vector3(Mathf.Min(startPosition.x, currentMousePosition.x), Mathf.Min(startPosition.y, currentMousePosition.y));
        Vector3 upperRightCorner = new Vector3(Mathf.Max(startPosition.x, currentMousePosition.x), Mathf.Max(startPosition.y, currentMousePosition.y));

        selectionAreaTransform.position = lowerLeftCorner;
        selectionAreaTransform.localScale = upperRightCorner - lowerLeftCorner;
    }
}
