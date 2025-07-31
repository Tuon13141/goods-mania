using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Data")]
    List<OrderData> _orderDatas = new List<OrderData>();
    List<OrderElement> _orderElements = new List<OrderElement>();

    [SerializeField] Transform m_OrderHolder;


    [Header("Order Display Settings")]
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;
    float _spacing;
    Vector3 _startPos;
    [SerializeField] float m_MaxSpacing = 6f;
    [SerializeField] int m_MaxSlot = 5;

    private void Start()
    {
        (_startPos, _spacing) = CommonTransformCalculator.GetStartPointAndSpacingReverse(m_EndPoint, m_StartPoint, m_MaxSlot, m_MaxSpacing);
    }

    public void SetUp(List<OrderData> orderDatas)
    {
        _orderDatas = orderDatas;

        for (int i = 0; i < orderDatas.Count; i++)
        {
            var order = PoolingManager.instance.Get(typeof(OrderElement)) as OrderElement;
            order.SetUp(orderDatas[i]); 

            order.transform.SetParent(m_OrderHolder);
            order.gameObject.name = $"OrderElement_{i}";

            float offSet = 0;
            if(i >= m_MaxSlot)
            {
                offSet = 1f; // Adjust the offset for more than max slots
            }

            Vector3 pos = new Vector3(_startPos.x + _spacing * i + offSet, _startPos.y, 0);
            order.transform.position = pos;
            order.transform.localScale = Vector3.one;

            _orderElements.Add(order);
        }
    }

    public void OnReset()
    {
        foreach(OrderElement orderElement in _orderElements)
        {
            orderElement.OnReset();
            PoolingManager.instance.Release(orderElement);
        }

        _orderElements.Clear();

        _orderDatas.Clear();
    }
}
