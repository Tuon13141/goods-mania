using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OrderElement : MonoBehaviour
{
    [Header("Data")]
    OrderData _orderData;
    [SerializeField] List<Sprite> m_AvaSprites = new List<Sprite>();
    List<ItemOrderElement> _itemOrderElements = new List<ItemOrderElement>();  

    [SerializeField] SpriteRenderer m_AvaSpriteRenderer;

    public bool isLocked = true;

    Dictionary<string, int> _itemDict = new Dictionary<string, int>();

    OrderManager _orderManager;

    [Header("Order Display Settings")]
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;
    float _spacing;
    Vector3 _startPos;
    [SerializeField] float m_MaxSpacing = 6f;
    [SerializeField] Transform m_Holder;
    [SerializeField] Transform m_Center;
    public Transform Center => m_Center;

    public void SetUp(OrderData data, OrderManager orderManager)
    {
        _orderManager = orderManager;
        int avaIndex = Random.Range(0, m_AvaSprites.Count);
        m_AvaSpriteRenderer.sprite = m_AvaSprites[avaIndex];    

        _orderData = data;

        foreach(ItemData item in data.orderItemDatas)
        {
            if(!_itemDict.ContainsKey(item.itemId)) _itemDict.Add(item.itemId, 0);
            
            _itemDict[item.itemId]++;
        }

        //Debug.Log("StartPoint " + m_StartPoint.position + " EndPoint " + m_EndPoint.position);

        (_startPos, _spacing) = CommonTransformCalculator.GetStartPointAndSpacingReverse(m_EndPoint, m_StartPoint, _itemDict.Keys.Count, m_MaxSpacing);

        //Debug.Log("StartPos " + _startPos + " Spacing " + _spacing);

        for (int i = 0; i < Mathf.Min(2, _itemDict.Keys.Count); i++)
        {
            string itemId = _itemDict.Keys.ElementAt(i);

            ItemData itemData = _orderData.orderItemDatas.FirstOrDefault(item => item.itemId == itemId);

            int totalCount = _itemDict[itemId];

            ItemOrderElement itemOrderElement = PoolingManager.instance.Get(typeof(ItemOrderElement)) as ItemOrderElement;

            itemOrderElement.transform.SetParent(m_Holder);
            itemOrderElement.transform.localScale = Vector3.one;
            itemOrderElement.transform.position = _startPos + Vector3.right * _spacing * i;

            itemOrderElement.SetUp(itemData, totalCount, this);
            _itemOrderElements.Add(itemOrderElement);
        }
    }

    public bool CanAddToOrder(ItemMergeElement itemMergeElement)
    {
        if(!isLocked) return false;
        if (_itemDict.ContainsKey(itemMergeElement.itemId))
        {
            if(_itemDict[itemMergeElement.itemId] - 1 < 0) return false;

            _itemDict[itemMergeElement.itemId]--;
            return true;
        }
        return false;
    }

    public void AddToOrder(ItemMergeElement ItemMergeElement)
    {
        _itemDict[ItemMergeElement.itemId]--;
        ItemOrderElement itemOrderElement = _itemOrderElements.FirstOrDefault(item => item.itemId == ItemMergeElement.itemId);
        if (itemOrderElement != null)
        {
            itemOrderElement.AddToOrder(ItemMergeElement);
        }
    }

    public void OnCompletedOrder(ItemOrderElement orderElement)
    {
        foreach(int value in _itemDict.Values)
        {
            if (value > 0) return;
        }   

        _orderManager.OnCompletedOrder(this);
    }


    public void OnReset()
    {
        _orderData = null;

        _itemDict.Clear();

        foreach (ItemOrderElement item in _itemOrderElements)
        {
            item.OnReset();
            PoolingManager.instance.Release(item);
        }

        _itemOrderElements.Clear();

        isLocked = true;
        // Reset any other properties or states if necessary
    }
}
