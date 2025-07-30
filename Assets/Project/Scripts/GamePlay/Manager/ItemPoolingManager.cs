using Kit.Common;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolingManager : Singleton<ItemPoolingManager>
{
    [SerializeField] private List<ItemDataElement> m_ItemPrefabs = new();

    private Dictionary<string, Queue<ItemDataElement>> poolDict = new();
    private Dictionary<string, ItemDataElement> prefabDict = new();

    private void Start()
    {
        foreach (var prefab in m_ItemPrefabs)
        {
            if (prefab == null || string.IsNullOrEmpty(prefab.ItemId)) continue;

            if (!prefabDict.ContainsKey(prefab.ItemId))
            {
                prefabDict[prefab.ItemId] = prefab;
                poolDict[prefab.ItemId] = new Queue<ItemDataElement>();
            }
        }
    }

    public ItemDataElement GetItem(string itemId, Vector3 position = default, Quaternion rotation = default)
    {
        // Nếu chưa từng thấy itemId => cố tìm trong danh sách prefab
        if (!prefabDict.TryGetValue(itemId, out var prefab))
        {
            prefab = m_ItemPrefabs.Find(p => p.ItemId == itemId);
            if (prefab == null)
            {
                Debug.LogError($"Không tìm thấy prefab với ItemId = {itemId}");
                return null;
            }
            prefabDict[itemId] = prefab;
            poolDict[itemId] = new Queue<ItemDataElement>();
        }

        var queue = poolDict[itemId];

        ItemDataElement item;
        if (queue.Count > 0)
        {
            item = queue.Dequeue();
        }
        else
        {
            item = Instantiate(prefab);
        }

        item.transform.SetPositionAndRotation(position, rotation);
        item.gameObject.SetActive(true);
        return item;
    }

    public void ReturnItem(ItemDataElement item)
    {
        if (item == null) return;

        string itemId = item.ItemId;
        item.gameObject.SetActive(false);

        if (!poolDict.ContainsKey(itemId))
        {
            poolDict[itemId] = new Queue<ItemDataElement>();
        }

        poolDict[itemId].Enqueue(item);
    }
}
