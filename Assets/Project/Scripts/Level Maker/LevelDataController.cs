using System.Collections;
using System.Collections.Generic;
using System.IO;
using Kit.Common;
using NaughtyAttributes;
using UnityEngine;

public class LevelDataController : Singleton<LevelDataController>
{
    private bool AlwaysFalse() => false;

    [SerializeField, ShowIf("AlwaysFalse")] OrderDataManager m_OrderDataManager;
    public OrderDataManager OrderDataManager => m_OrderDataManager;
    [SerializeField, ShowIf("AlwaysFalse")] ShelfDataElement m_ShelfDataElementPrefab;

    [SerializeField, ShowIf("AlwaysFalse")] Transform m_ShelfHolder;
    [HorizontalLine(1, EColor.Blue)]
    public List<ShelfDataElement> shelfs = new List<ShelfDataElement>();

    [HorizontalLine(1, EColor.Blue)]
    [SerializeField] Vector3 m_SpawnPosition = Vector3.zero;
    [SerializeField] int m_ShelfAtIndex = 1;
    [Button("Spawn Shelf")]
    public void SpawnShelf()
    {
        ShelfDataElement shelf = Instantiate(m_ShelfDataElementPrefab);
        shelf.transform.SetParent(m_ShelfHolder);
        shelf.transform.localPosition = m_SpawnPosition;
        shelf.name = "ShelfDataElement_" + shelfs.Count;
      
        shelfs.Add(shelf);
    }



    [Button("Spawn Shelf At Index")]
    public void SpawnShelfAtIndex()
    {
        if (m_ShelfAtIndex < 0 || m_ShelfAtIndex > shelfs.Count)
        {
            Debug.LogError("Invalid index for spawning shelf: " + m_ShelfAtIndex);
            return;
        }
        ShelfDataElement shelf = Instantiate(m_ShelfDataElementPrefab);
        shelf.transform.SetParent(m_ShelfHolder);
        shelf.transform.localPosition = m_SpawnPosition;
        shelf.name = "ShelfDataElement_" + m_ShelfAtIndex;
        
        shelfs.Insert(m_ShelfAtIndex, shelf);
        shelf.transform.SetSiblingIndex(m_ShelfAtIndex); 

        for (int i = m_ShelfAtIndex + 1; i < shelfs.Count; i++)
        {
            shelfs[i].name = "ShelfDataElement_" + i;
            shelfs[i].transform.SetSiblingIndex(i);
        }
    }

    [Button("Delete Shelf At Index")]
    public void DeleteShelfAtIndex()
    {
        if (m_ShelfAtIndex < 0 || m_ShelfAtIndex >= shelfs.Count)
        {
            Debug.LogError("Invalid index for deleting shelf: " + m_ShelfAtIndex);
            return;
        }
        ShelfDataElement shelfToDelete = shelfs[m_ShelfAtIndex];
        if (shelfToDelete != null)
        {
            DestroyImmediate(shelfToDelete.gameObject);
            shelfs.RemoveAt(m_ShelfAtIndex);
        }
        for (int i = m_ShelfAtIndex; i < shelfs.Count; i++)
        {
            shelfs[i].name = "ShelfDataElement_" + i;
            shelfs[i].transform.SetSiblingIndex(i);
        }
    }


    [Button("Delete All Shelfs")]
    public void DeleteAllShelfs()
    {
        foreach (ShelfDataElement shelf in shelfs)
        {
            if (shelf == null) continue;
            DestroyImmediate(shelf.gameObject);
        }
        shelfs.Clear();
    }
    public void DeleteShelf(ShelfDataElement shelf)
    {
        if (shelf == null) return;
        shelfs.Remove(shelf);
        DestroyImmediate(shelf.gameObject);
    }

    public void CreateLevelDataFile(string levelName)
    {
        //CHUYỂN SANG JSON VÀ LƯU VÀO FILE Resources/LevelData/Lv_n.txt
        LevelData levelData = new LevelData();

        // Convert Order Data
        foreach (var order in OrderDataManager.orderDataElements)
        {
            OrderData orderData = new OrderData();

            foreach (var selection in order.itemSelections)
            {
                if (selection.itemIndex >= 0 && selection.itemIndex < OrderDataManager.items.Count)
                {
                    var item = OrderDataManager.items[selection.itemIndex];
                    if (item != null)
                    {
                        var itemData = item.GetComponent<ItemDataElement>();
                        if (itemData != null)
                        {
                            orderData.orderItemDatas.Add(new ItemData { itemId = itemData.ItemId });
                        }
                    }
                }
            }

            levelData.orderDatas.Add(orderData);
        }

        // Convert Shelf Data
        foreach (var shelf in shelfs)
        {
            ShelfData shelfData = new ShelfData
            {
                spawnPosition = new Vector2(shelf.transform.position.x, shelf.transform.position.y)
            };

            foreach (var row in shelf.rowDataElements)
            {
                RowData rowData = new RowData();

                foreach (var itemData in row.ItemDataElements)
                {
                    if (itemData != null)
                    {
                        rowData.itemDatas.Add(new ItemData { itemId = itemData.ItemId });
                    }
                }

                shelfData.rowDatas.Add(rowData);
            }

            levelData.shelfDatas.Add(shelfData);
        }

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(levelData, true);

        // Create directory if not exists
        string directoryPath = Path.Combine(Application.dataPath, "Resources/LevelData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save to file
        string filePath = Path.Combine(directoryPath, levelName + ".txt");
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Level data saved to: " + filePath);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void LoadLevelFromTextAsset(TextAsset ts)
    {
        if (ts == null)
        {
            Debug.LogError("Level text asset is null.");
            return;
        }

        string json = ts.text;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Level data is empty.");
            return;
        }

        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        if (levelData == null)
        {
            Debug.LogError("Failed to parse level data.");
            return;
        }

        // Clear current level
        DeleteAllShelfs();
        OrderDataManager.orderDataElements.Clear();

        // Rebuild Orders
        foreach (var order in levelData.orderDatas)
        {
            OrderDataElement orderElement = new OrderDataElement();
            foreach (var item in order.orderItemDatas)
            {
                var selection = new OrderDataElement.ItemSelection();

                // Tìm itemIndex dựa trên itemId
                int itemIndex = OrderDataManager.items.FindIndex(x =>
                {
                    var itemData = x.GetComponent<ItemDataElement>();
                    return itemData != null && itemData.ItemId == item.itemId;
                });

                if (itemIndex != -1)
                {
                    selection.itemId = item.itemId;
                    selection.itemIndex = itemIndex;
                    orderElement.itemSelections.Add(selection);
                }
            }

            OrderDataManager.orderDataElements.Add(orderElement);
        }

        // Rebuild Shelfs
        foreach (var shelfData in levelData.shelfDatas)
        {
            ShelfDataElement shelf = Instantiate(m_ShelfDataElementPrefab, m_ShelfHolder);
            shelf.transform.localPosition = new Vector3(shelfData.spawnPosition.x, shelfData.spawnPosition.y, 0f);
            shelf.name = "ShelfDataElement_" + shelfs.Count;
            shelfs.Add(shelf);

            foreach (var rowData in shelfData.rowDatas)
            {
                RowDataElement row = Instantiate(shelf.RowDataElementPrefab, shelf.transform);
                row.transform.SetParent(shelf.Holder); 
                row.transform.localPosition = shelf.StartRowPosition.localPosition +
                                              new Vector3(0, 0, shelf.RowSpacing * shelf.rowDataElements.Count);

                row.name = "RowDataElement_" + shelf.rowDataElements.Count;
                row.SetShelfDataElement(shelf);
                row.SetUp();

                // Spawn items theo từng slot
                for (int i = 0; i < rowData.itemDatas.Count; i++)
                {
                    string itemId = rowData.itemDatas[i].itemId;

                    GameObject prefab = OrderDataManager.items.Find(x =>
                    {
                        var itemData = x.GetComponent<ItemDataElement>();
                        return itemData != null && itemData.ItemId == itemId;
                    });

                    if (prefab != null)
                    {
                        row.OnAddToSlot(prefab, i + 1);
                    }
                    else
                    {
                        Debug.LogWarning($"Item with id {itemId} not found in item list.");
                    }
                }

                shelf.rowDataElements.Add(row);
            }
        }

        Debug.Log("Level loaded from " + ts.name);
    }

}
