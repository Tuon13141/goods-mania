using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderDataManager : MonoBehaviour
{
    [Header("Item Management")]
    [ShowAssetPreview]
    public List<GameObject> items = new List<GameObject>();
    [Header("Order")]
    public List<OrderDataElement> orderDataElements = new List<OrderDataElement>();
}

[System.Serializable]
public class OrderDataElement
{
    public List<ItemSelection> itemSelections = new List<ItemSelection>();

    [System.Serializable]
    public class ItemSelection
    {
        [SerializeField, HideInInspector]
        public string itemId;

        [SerializeField]
        public int itemIndex = 0;
    }
}