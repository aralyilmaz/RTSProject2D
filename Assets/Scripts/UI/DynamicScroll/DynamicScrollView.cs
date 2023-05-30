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

    public List<Sprite> spriteList;
    public List<string> textList;

    private int topItemIndex;
    private int botItemIndex;
    private int topNextItemPos;
    private int botNextItemPos;

    public Vector3 spacing = Vector3.down * 50;

    [SerializeField]
    private Transform topLimit;

    [SerializeField]
    private Transform botLimit;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        //if (spriteList.Count != poolSize && textList.Count != poolSize)
        //{
        //    Debug.Log("not enough sprite");
        //    return;
        //}
        InitScrollView();
    }

    private void Update()
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
                item.InitItem(spriteList[0], textList[i]);
            }
            //tmp.SetActive(false);
            objectPool.Add(tmp);
            
        }

        topItemIndex = 0;
        botItemIndex = poolSize - 1;
        topNextItemPos = 1;
        botNextItemPos = -poolSize;
        OrderListItems();
        //StartCoroutine(InfiniteViewCoroutine(0.1f));
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
            GameObject newObj = GetPooledObject(botItemIndex);
            if (newObj != null)
            {
                //newObj.transform.SetParent(content);
                //newObj.transform.SetAsFirstSibling();
                newObj.transform.localPosition = Vector3.up * 50 * topNextItemPos;
                topItemIndex = botItemIndex;

                if (botItemIndex == 0)
                {
                    botItemIndex = poolSize - 1;
                }
                else
                {
                    botItemIndex--;
                }
                topNextItemPos++;
                botNextItemPos++;
            }
        }

        if (!goingUp)
        {
            GameObject newObj = GetPooledObject(topItemIndex);
            if (newObj != null)
            {
                //newObj.transform.SetParent(content);
                //newObj.transform.SetAsFirstSibling();
                newObj.transform.localPosition = Vector3.up * 50 * botNextItemPos;
                botItemIndex = topItemIndex;

                if (topItemIndex == poolSize - 1)
                {
                    topItemIndex = 0;
                }
                else
                {
                    topItemIndex++;
                }
                botNextItemPos--;
                topNextItemPos--;
            }
        }
    }

    private IEnumerator InfiniteViewCoroutine(float time)
    {
        //infinite loop in
        for (; ; )
        {
            if (objectPool[botItemIndex].transform.position.y < -15f)
            {
                //Scrolling Up
                InfiniteView(true);
            }
            else if (objectPool[topItemIndex].transform.position.y > 450f)
            {
                //Scrolling Down
                InfiniteView(false);
            }
            yield return new WaitForSeconds(time);
        }
    }
}
