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

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMouseWorldPosition(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            //Left mouse button press
            startPosition = mouseWorldPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Left mouse button release
            //Debug.Log(mouseWorldPosition + " " + startPosition);
            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, mouseWorldPosition);

            foreach(Collider2D collider2D in collider2DArray)
            {
                Debug.Log(collider2D.name);
            }
        }
    }

    private void CalculateMouseWorldPosition(Vector3 mouseScreenPosition)
    {
        //get mouse position out of cliping plane of camera
        //mouseScreenPosition.z = mainCam.nearClipPlane + 1;
        mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
    }
}
