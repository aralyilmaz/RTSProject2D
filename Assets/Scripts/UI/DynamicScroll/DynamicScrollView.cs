using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{
    private ScrollRect scrollRect;
    public List<GameObject> objectPool;
    public GameObject objectToPool;
    public int poolSize;

    public Transform content;

    public List<BuildingObject> buildingList;

    private int topItemIndex;
    private int botItemIndex;
    private int topNextItemPos;
    private int botNextItemPos;

    public Vector3 spacing = Vector3.down * 50;

    [SerializeField]
    private Transform topLimit;

    [SerializeField]
    private Transform botLimit;

    private bool initComplete = false;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        poolSize = buildingList.Count;

        InitScrollView();
    }

    private void Update()
    {
        if (initComplete) 
        {
            if (objectPool[botItemIndex].transform.position.y < botLimit.position.y)
            {
                //Scrolling Up
                InfiniteView(true);
            }
            else if (objectPool[topItemIndex].transform.position.y > topLimit.position.y)
            {
                //Scrolling Down
                InfiniteView(false);
            }
        }
    }

    public void InitScrollView()
    {
        objectPool = new List<GameObject>();
        GameObject tmp;
        ScrollViewItem item;
        for (int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(objectToPool, content);
            if(tmp.TryGetComponent<ScrollViewItem>(out item))
            {
                item.InitItemButton(buildingList[i]);
            }
            //tmp.SetActive(false);
            objectPool.Add(tmp);
            
        }

        topItemIndex = 0;
        botItemIndex = poolSize - 1;
        topNextItemPos = 1;
        botNextItemPos = -poolSize;
        OrderListItems();
        initComplete = true;
    }

    private void OrderListItems()
    {
        for (int i = 0; i < poolSize ; i++)
        {
            if (objectPool[i] != null)
            {
                objectPool[i].transform.localPosition = (spacing * i);
            }
        }
        topLimit.position = objectPool[0].transform.position - spacing;
        botLimit.position = objectPool[poolSize - 1].transform.position + spacing;
    }

    private GameObject GetPooledObject(int objectIndex)
    {
        if (objectIndex >= 0 && objectIndex < poolSize)
        {
            return objectPool[objectIndex];
        }
        return null;
    }

    private void InfiniteView(bool goingUp)
    {
        // Check to make sure the pool is not empty
        if (goingUp)
        {
            //if we go up then put the bottom object in the pool to top
            GameObject newObj = GetPooledObject(botItemIndex);
            if (newObj != null)
            {
                newObj.transform.localPosition = Vector3.up * 50 * topNextItemPos;
                topItemIndex = botItemIndex; //bot item is now at the top

                if (botItemIndex == 0)//check to stay in range of object list
                {
                    botItemIndex = poolSize - 1;
                }
                else
                {
                    botItemIndex--;
                }
                //increase next positions because go up by 1
                topNextItemPos++;
                botNextItemPos++;
            }
        }

        if (!goingUp)
        {
            //if we go down then put the top object in the pool to bot
            GameObject newObj = GetPooledObject(topItemIndex);
            if (newObj != null)
            {
                newObj.transform.localPosition = Vector3.up * 50 * botNextItemPos;
                botItemIndex = topItemIndex;

                if (topItemIndex == poolSize - 1)//check to stay in range of object list
                {
                    topItemIndex = 0;
                }
                else
                {
                    topItemIndex++;
                }
                //decrease next positions because go down by 1
                botNextItemPos--;
                topNextItemPos--;
            }
        }
    }

    public void EnableDisableAllButtons(bool enabled)
    {
        foreach(GameObject viewItemObject in objectPool)
        {
            if (viewItemObject.TryGetComponent<ScrollViewItem>(out ScrollViewItem item))
            {
                item.EnableDisableButton(enabled);
            }
        }
    }
}
