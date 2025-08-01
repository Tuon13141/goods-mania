using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemElement : MonoBehaviour
{
    [SerializeField] Transform m_Holder;
    [SerializeField] DraggableObject m_DraggableObject;

    ItemData _itemData;
    public string itemId => _itemData?.itemId ?? string.Empty;

    ItemDataElement _itemDataElement;
    ShelfElement _shelfElement;
    public ShelfElement ShelfElement => _shelfElement;

    public bool isLocked = true; 

    private void Awake()
    {
        m_DraggableObject.OnTouchEvent += OnTouch;
        m_DraggableObject.OnDragEvent += OnDrag;
        m_DraggableObject.OnDropEvent += OnDrop;
        m_DraggableObject.OnDropedOnEvent += OnDropedOn;
        m_DraggableObject.OnDropedFailEvent += OnDropedFail;
    }

    public void SetUp(ItemData itemData, ShelfElement shelfElement)
    {
        _shelfElement = shelfElement;
        _itemData = itemData;

        _itemDataElement = ItemPoolingManager.instance.GetItem(itemData.itemId);
        _itemDataElement.transform.SetParent(m_Holder);
        _itemDataElement.transform.localPosition = Vector3.zero;
        _itemDataElement.transform.localScale = Vector3.one;

        _itemDataElement.gameObject.name = "ItemDataElement_" + itemData.itemId;

        gameObject.name = "ItemElement_" + itemData.itemId;
    }

    public void OnTouch()
    {
        //Debug.Log(gameObject.name + " touched.");
        if(isLocked) return;

        TweenManager.tweens.Add(m_Holder.DOLocalMoveZ(-0.5f, 0.25f));
    }

    public void OnDrag(Vector3 targetPos)
    {
        if (isLocked) return;

        transform.position = targetPos;
    }

    public void OnDrop()
    {
        if (isLocked) return;

        TweenManager.tweens.Add(m_Holder.DOLocalMoveZ(0f, 0.25f).OnComplete(() =>
        {
            // Handle drop logic here, e.g., check if the item is placed correctly
            Debug.Log(gameObject.name + " dropped.");
        }));
    }

    private void OnDropedOn(DropOnableObject dropOnableObject)
    {
        if (isLocked) return;

        ShelfElement shelfElement = dropOnableObject.GetComponent<ShelfElement>();
        if (shelfElement == null)
        {
            return;
        }

        _shelfElement.RemoveItem(this);

        this._shelfElement = shelfElement;
    }

    public void OnDropedFail()
    {
        if (isLocked) return;

        TweenManager.tweens.Add(transform.DOMove(_shelfElement.GetItemPosition(this).position , 0.25f).OnComplete(() =>
        {
            //Debug.Log(gameObject.name + " drop failed, returning to original position.");
            OnDrop();
        }));
    }
    public void OnReset()
    {
        _itemData = null;
        _shelfElement = null;
        isLocked = true;

        ItemPoolingManager.instance.ReturnItem(_itemDataElement);
        _itemDataElement = null;

        m_Holder.transform.localPosition = Vector3.zero;
    }
}
