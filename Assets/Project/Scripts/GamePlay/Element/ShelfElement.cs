using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShelfElement : MonoBehaviour
{
    [Header("Data")]
    public List<RowData> rowDatas = new List<RowData>();
    List<ItemElement> _allItemElements = new List<ItemElement>();

    List<ItemElement> _row_1_Items = new List<ItemElement>();
    List<ItemElement> _row_2_Items = new List<ItemElement>();


    [Header("Row Position List")]
    [SerializeField] List<Transform> m_Row_1_Positions = new List<Transform>();
    [SerializeField] List<Transform> m_Row_2_Positions = new List<Transform>();

    Dictionary<Transform, ItemElement> _itemElementByPosition = new Dictionary<Transform, ItemElement>();

    ShelfManager _shelfManager;

    [SerializeField] Transform m_ItemHolder;
    [SerializeField] DropOnableObject m_DropOnableObject;
    private void Awake()
    {
        m_DropOnableObject.OnDragEnterEvent += OnDragEnter;
        m_DropOnableObject.OnDragExitEvent += OnDragExit;
        m_DropOnableObject.OnDropEvent += OnDrop;
    }
    public void SetUp(ShelfData shelfData, ShelfManager shelfManager)
    {
        this._shelfManager = shelfManager;
        transform.localPosition = shelfData.spawnPosition;

        foreach (Transform t in m_Row_1_Positions)
        {
            //Debug.Log("Adding position: " + t.position + " to " + name);
            _itemElementByPosition.Add(t, null);
        }

        rowDatas = shelfData.rowDatas;
        for (int i = 0; i < Mathf.Min(2, shelfData.rowDatas.Count); i++)
        {
            var itemList = shelfData.rowDatas[i].itemDatas;
            for (int j = 0; j < itemList.Count; j++)
            {
                ItemData itemData = itemList[j];
                if (itemData == null) continue;

                ItemElement itemElement = PoolingManager.instance.Get(typeof(ItemElement)) as ItemElement;
                itemElement.transform.SetParent(m_ItemHolder);
                itemElement.SetUp(itemData, this);

                if(i == 0)
                {
                    itemElement.transform.position = m_Row_1_Positions[j].position;
                    _itemElementByPosition[m_Row_1_Positions[j]] = itemElement;
                    _row_1_Items.Add(itemElement);

                    itemElement.isLocked = false;
                }
                else
                {
                    itemElement.transform.position = m_Row_2_Positions[j].position;
                    _row_2_Items.Add(itemElement);
                }

                _allItemElements.Add(itemElement);
             
            }
        }
    }

    public void OnDragEnter(DraggableObject draggable)
    {
        
    }

    public void OnDragExit(DraggableObject draggable)
    {
       
    }

    public void OnDrop(DraggableObject draggable)
    {
        ItemElement itemElement = draggable.GetComponent<ItemElement>();

        if(itemElement == null || itemElement.isLocked)
        {
            draggable.OnDropedFail();
            return;
        }

        Transform nearestEmptyPosition = null;
        float minDistance = float.MaxValue;
        bool check = false;

        foreach (var key in _itemElementByPosition.Keys)
        {
            //Debug.Log("Checking position: " + key.position + " for item: " + itemElement.gameObject.name);
            if (_itemElementByPosition[key] == null)
            {
                check = true;
                float distance = Vector3.Distance(itemElement.transform.position, key.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEmptyPosition = key;
                }
            }
        }

        if(!check)
        {
            Debug.LogWarning("No empty position found for item: " + itemElement.gameObject.name);
            draggable.OnDropedFail();
            return;
        }

        Debug.Log("Nearest empty position found at: " + nearestEmptyPosition);
        itemElement.transform.SetParent(m_ItemHolder);
        TweenManager.tweens.Add(itemElement.transform.DOMove(nearestEmptyPosition.position, 0.25f)
            .SetEase(Ease.OutQuad));

        _itemElementByPosition[nearestEmptyPosition] = itemElement;

        if (m_Row_1_Positions.Exists(t => t == nearestEmptyPosition))
        {
            _row_1_Items.Add(itemElement);
        }

        _allItemElements.Add(itemElement);

        draggable.OnDropedOn(m_DropOnableObject);
    }


    public void RemoveItem(ItemElement itemElement)
    {
        if (itemElement == null) return;
        foreach (var key in _itemElementByPosition.Keys.ToList())
        {
            if (_itemElementByPosition[key] == itemElement)
            {
                _itemElementByPosition[key] = null;
                break;
            }
        }
        if (_row_1_Items.Contains(itemElement))
        {
            _row_1_Items.Remove(itemElement);
        }

        _allItemElements.Remove(itemElement);
    }

    public Transform GetItemPosition(ItemElement itemElement)
    {
        foreach (var key in _itemElementByPosition.Keys)
        {
            if (_itemElementByPosition[key] == itemElement)
            {
                return key;
            }
        }
        return null;
    }

    public void OnReset()
    {
        rowDatas.Clear();
        foreach(ItemElement i in _allItemElements)
        {
            i.OnReset();
            PoolingManager.instance.Release(i);  
        }

        
        _itemElementByPosition.Clear();
        _row_1_Items.Clear();
        _row_2_Items.Clear();
        _allItemElements.Clear();
    }
}
