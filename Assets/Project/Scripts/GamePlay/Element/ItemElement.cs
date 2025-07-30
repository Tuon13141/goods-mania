using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemElement : MonoBehaviour
{
    [SerializeField] Transform m_Holder;
    [SerializeField] DraggableObject m_DraggableObject;

    ItemData _itemData;

    ItemDataElement _itemDataElement;
    ShelfElement _shelfElement;

    private void Start()
    {
        m_DraggableObject.OnTouchEvent += OnTouch;
        m_DraggableObject.OnDragEvent += OnDrag;
        m_DraggableObject.OnDropEvent += OnDrop;
    }

    public void SetUp(ItemData itemData, ShelfElement shelfElement)
    {
        _shelfElement = shelfElement;
        _itemData = itemData;

        _itemDataElement = ItemPoolingManager.instance.GetItem(itemData.itemId);
        _itemDataElement.transform.SetParent(m_Holder);
        _itemDataElement.transform.localPosition = Vector3.zero;

        _itemDataElement.gameObject.name = "ItemDataElement_" + itemData.itemId;

        gameObject.name = "ItemElement_" + itemData.itemId;
    }

    public void OnTouch()
    {
        Debug.Log(gameObject.name + " touched.");

        m_Holder.DOLocalMoveZ(-0.5f, 0.25f);
    }

    public void OnDrag()
    {
        
    }

    public void OnDrop()
    {
        m_Holder.DOLocalMoveZ(0f, 0.25f).OnComplete(() =>
        {
            // Handle drop logic here, e.g., check if the item is placed correctly
            Debug.Log(gameObject.name + " dropped.");
        });
    }
}
