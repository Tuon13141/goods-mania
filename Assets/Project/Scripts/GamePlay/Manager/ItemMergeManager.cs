using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemMergeManager 
{
    static List<ItemMergeElement> _itemMergeElements = new List<ItemMergeElement>();

    public static void AddItemMergeElement(ItemMergeElement element)
    {
        _itemMergeElements.Add(element);
    }

    public static void OnReset()
    {
        foreach (var element in _itemMergeElements)
        {
            element.OnReset();
            PoolingManager.instance.Release(element);
        }
        _itemMergeElements.Clear();
    }   
}
