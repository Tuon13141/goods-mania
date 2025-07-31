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

    Dictionary<string, int> _itemDict = new Dictionary<string, int>();

    [Header("Order Display Settings")]
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;
    float _spacing;
    Vector3 _startPos;
    [SerializeField] float m_MaxSpacing = 6f;
    [SerializeField] Transform m_Holder;

    public void SetUp(OrderData data)
    {
        int avaIndex = Random.Range(0, m_AvaSprites.Count);
        m_AvaSpriteRenderer.sprite = m_AvaSprites[avaIndex];    

        _orderData = data;

        foreach(ItemData item in data.orderItemDatas)
        {
            if(!_itemDict.ContainsKey(item.itemId)) _itemDict.Add(item.itemId, 0);
            
            _itemDict[item.itemId]+=3;
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

            itemOrderElement.SetUp(itemData, totalCount);
            _itemOrderElements.Add(itemOrderElement);
        }
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
        // Reset any other properties or states if necessary
    }
}
