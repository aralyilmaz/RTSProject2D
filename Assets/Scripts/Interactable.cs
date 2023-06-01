using UnityEngine;

/*	
	This component is for all objects that the player can
	interact with such as enemies, buildings etc. It is meant
	to be used as a base class.
*/

public class Interactable : MonoBehaviour
{
    //interactable currently being focused?
    public bool isFocus = false;

    //already interacted with the object?
    bool hasInteracted = false;

    public bool placed = false;

    public GridMapManager grid { get; private set; }

    void Start()
    {
        grid = GridMapManager.instance;
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with: " + this.name);
        // This method is meant to be overwritten
    }

    void Update()
    {
        // If we are currently being focused
        // and we haven't already interacted with the object
        if (isFocus && !hasInteracted)
        {
            // Interact with the object
            Interact();
            hasInteracted = true;
        }
    }

    // Called when the object starts being focused
    public void OnFocused()
    {
        isFocus = true;
        hasInteracted = false;
    }

    // Called when the object is no longer focused
    public void OnDefocused()
    {
        isFocus = false;
        hasInteracted = false;
    }

}