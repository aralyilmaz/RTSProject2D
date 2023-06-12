using UnityEngine;
using System.Collections.Generic;
using System;

/*	
	This component is for all objects that the player can
	interact with such as enemies, buildings etc. It is meant
	to be used as a base class.
*/

public abstract class Interactable : MonoBehaviour
{
    //interactable currently being focused?
    public bool isFocus = false;

    //already interacted with the object?
    bool hasInteracted = false;

    public bool placed = false;

    public int width = 1;
    public int height = 1;

    public abstract void Interact();

    public abstract Vector2Int GetGridPosition();

    public abstract List<Vector2Int> GetNeighbors();

    public abstract void TakeDamage(float damage);

    void Update()
    {
        // If we are currently being focused
        // and we haven't already interacted with the object
        if (isFocus && !hasInteracted)
        {
            // Interact with the object
            //Interact();
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