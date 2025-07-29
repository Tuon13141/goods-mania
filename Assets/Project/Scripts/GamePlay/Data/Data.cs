using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public List<OrderData> orderDatas = new List<OrderData>();
    public List<ShelfData> shelfDatas = new List<ShelfData>();  
}
[Serializable]
public class ItemData
{
    public string itemId;
}
[Serializable]
public class OrderData
{
    public List<ItemData> orderItemDatas = new List<ItemData>();
}
[Serializable]
public class ShelfData
{
    public List<RowData> rowDatas = new List<RowData>();
    public Vector2 spawnPosition;
}
[Serializable]
public class RowData
{
    public List<ItemData> itemDatas = new List<ItemData>();
}