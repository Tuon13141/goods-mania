using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RowDataElement : MonoBehaviour
{
    List<ItemDataElement> _itemDataElements = new List<ItemDataElement>();
    public List<ItemDataElement> ItemDataElements => _itemDataElements;
    private bool AlwaysFalse() => false;

    [SerializeField, ShowIf("AlwaysFalse")] Transform m_ItemPosition1;
    [SerializeField, ShowIf("AlwaysFalse")] Transform m_ItemPosition2;
    [SerializeField, ShowIf("AlwaysFalse")] Transform m_ItemPosition3;

    ShelfDataElement _shelfDataElement;

    [Header("Item Picked")]
    [ShowAssetPreview]
    public List<GameObject> choiceItems = new List<GameObject>();


    [Header("Item Management")]
    [ShowAssetPreviewWithButton(64, 64, "Add to Slot 1", "Add to Slot 2", "Add to Slot 3")]
    public List<GameObject> items = new List<GameObject>();


    public void OnAddToSlot(GameObject itemPrefab, int slotIndex)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("Cannot add null item to slot");
            return;
        }

        // Validate slot index
        slotIndex = Mathf.Clamp(slotIndex, 1, 3);
        Debug.Log($"Adding {itemPrefab.name} to slot {slotIndex}");

        // Get or create ItemDataElement
        ItemDataElement itemData = itemPrefab.GetComponent<ItemDataElement>();
        if (itemData == null)
        {
            Debug.LogWarning("Item prefab doesn't have ItemDataElement component");
            return;
        }

        // Handle choiceItems list
        EnsureListSize(ref choiceItems, slotIndex);

        // Destroy old item if exists
        if (choiceItems[slotIndex - 1] != null)
        {
            DestroyImmediate(choiceItems[slotIndex - 1]);
        }

        // Instantiate new item
        Transform slotTransform = GetSlotPosition(slotIndex);
        GameObject newItem = Instantiate(itemPrefab);
        newItem.transform.SetParent(transform);
        newItem.transform.localPosition = slotTransform.localPosition;
        choiceItems[slotIndex - 1] = newItem;

        // Update item data
        UpdateItemData(slotIndex, itemData);
    }

    private void EnsureListSize<T>(ref List<T> list, int requiredSize)
    {
        while (list.Count < requiredSize)
        {
            list.Add(default(T));
        }
    }

    private Transform GetSlotPosition(int slotIndex)
    {
        switch (slotIndex)
        {
            case 1: return m_ItemPosition1;
            case 2: return m_ItemPosition2;
            case 3: return m_ItemPosition3;
            default: return null;
        }
    }

    private void UpdateItemData(int slotIndex, ItemDataElement itemData)
    {
        EnsureListSize(ref _itemDataElements, slotIndex);

        _itemDataElements[slotIndex - 1] = itemData;
    }

    [HorizontalLine(1, EColor.Blue)]
    [SerializeField] int m_ItemAtIndex = 0;

    [Button("Delete Item Slot At")]
    public void ClearItemSlotAtIndex()
    {
        if (m_ItemAtIndex < 1 || m_ItemAtIndex > choiceItems.Count)
        {
            Debug.LogError("Invalid index for clearing item slot: " + m_ItemAtIndex);
            return;
        }
        GameObject itemToClear = choiceItems[m_ItemAtIndex - 1];
        if (itemToClear != null)
        {
            DestroyImmediate(itemToClear);
            choiceItems[m_ItemAtIndex - 1] = null;
            _itemDataElements[m_ItemAtIndex - 1] = null;
        }
    }


    [Button("Delete All Items")]
    public void ClearAllItems()
    {
        foreach (GameObject item in choiceItems)
        {
            if (item != null)
            {
                DestroyImmediate(item);
            }
        }
        choiceItems.Clear();
        _itemDataElements.Clear();
    }

    [Button("Delete Row")]
    public void DeleteRow()
    {
        _shelfDataElement.DeleteRow(this);
    }

    public void SetShelfDataElement(ShelfDataElement shelfDataElement)
    {
        _shelfDataElement = shelfDataElement;
    }

    public void SetUp()
    {
        items.Clear();

        items = LevelDataController.instance.OrderDataManager.items;
    }
}
