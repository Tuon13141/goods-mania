using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotElement : MonoBehaviour
{
    [Header("Data")]
    ItemMergeElement _itemMergeElement;
    public bool isLocked = false;

    [SerializeField] Transform m_Holder;

    public void SetUp()
    {

    }

    public bool CanAddToSlot()
    {
        if (isLocked) return false;
        if (_itemMergeElement == null) return true;
        return false;
    }

    public void AddToSlot(ItemMergeElement itemMergeElement)
    {
        if (isLocked) return;
        if (_itemMergeElement != null) return;
        _itemMergeElement = itemMergeElement;
        _itemMergeElement.transform.SetParent(m_Holder);


        _itemMergeElement.OnBeginAnim(m_Holder, new Vector3(0, 0, 0));
    }

    public void OnReset()
    {
        _itemMergeElement = null;
        isLocked = false;
    }
}
