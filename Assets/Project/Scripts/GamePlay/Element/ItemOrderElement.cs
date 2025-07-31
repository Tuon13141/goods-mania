using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemOrderElement : MonoBehaviour
{
    [Header("Data")]
    ItemData _itemData;
    int _totalCount;

    ItemDataElement _itemDataElement;
    [SerializeField] TextMeshPro m_CountText;

    public void SetUp(ItemData itemData, int total)
    {
        _itemData = itemData;
        _totalCount = total;

        _itemDataElement = ItemPoolingManager.instance.GetItem(itemData.itemId);
        _itemDataElement.transform.SetParent(transform);
        _itemDataElement.transform.localPosition = Vector3.zero;
        _itemDataElement.transform.localScale = Vector3.one;    

        UpdateText();
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
}
