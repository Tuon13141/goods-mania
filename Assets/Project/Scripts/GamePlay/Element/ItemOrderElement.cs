using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemOrderElement : MonoBehaviour
{
    [Header("Data")]
    ItemData _itemData;
    int _totalCount;
    public string itemId => _itemData != null ? _itemData.itemId : string.Empty;

    ItemDataElement _itemDataElement;
    [SerializeField] TextMeshPro m_CountText;

    OrderElement _orderElement;

    public void SetUp(ItemData itemData, int total, OrderElement orderElement)
    {
        _itemData = itemData;
        _totalCount = total;
        _orderElement = orderElement;

        _itemDataElement = ItemPoolingManager.instance.GetItem(itemData.itemId);
        _itemDataElement.transform.SetParent(transform);
        _itemDataElement.transform.localPosition = Vector3.zero;
        _itemDataElement.transform.localScale = Vector3.one;    

        UpdateText();
    }

    public void AddToOrder(ItemMergeElement itemMergeElement)
    {
        if (itemMergeElement.itemId != _itemData.itemId) return;
        _totalCount--;
        Action action = UpdateText;
        if(_totalCount <= 0)
        {
            action += OnCompletedOrder;
        }

        itemMergeElement.OnBeginAnim(_itemDataElement.transform, Vector3.zero, action);
    }

    public void OnReset()
    {
        ItemPoolingManager.instance.ReturnItem(_itemDataElement);
        _itemData = null;
        _itemDataElement = null;
        _totalCount = 0;
    }

    void UpdateText()
    {
        m_CountText.text = _totalCount.ToString();
    }

    void OnCompletedOrder()
    {
        _orderElement.OnCompletedOrder(this);
    }
}
