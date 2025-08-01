using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Data")]
    List<OrderData> _orderDatas = new List<OrderData>();
    List<OrderElement> _allOrderElements = new List<OrderElement>();
    List<OrderElement> _currentOrderElements = new List<OrderElement>();

    [SerializeField] Transform m_OrderHolder;


    [Header("Order Display Settings")]
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;
    float _spacing;
    Vector3 _startPos;
    [SerializeField] float m_MaxSpacing = 6f;
    [SerializeField] int m_MaxOrder = 3;

    List<Vector3> _listPos = new List<Vector3>();

    int _currentOrderIndex = 0;
    private void Start()
    {
        (_startPos, _spacing) = CommonTransformCalculator.GetStartPointAndSpacingReverse(m_EndPoint, m_StartPoint, m_MaxOrder, m_MaxSpacing);
    }

    public void SetUp(List<OrderData> orderDatas)
    {
        _orderDatas = orderDatas;

        for (int i = 0; i < Mathf.Min(_orderDatas.Count, m_MaxOrder); i++)
        {
            OrderElement order = CreateOrderElement(i);
            _currentOrderIndex++;

            _listPos.Add(order.transform.position);
        }
    }
    
    OrderElement CreateOrderElement(int i)
    {
        var order = PoolingManager.instance.Get(typeof(OrderElement)) as OrderElement;
        order.SetUp(_orderDatas[i], this);

        order.transform.SetParent(m_OrderHolder);
        order.gameObject.name = $"OrderElement_{i}";

        order.isLocked = true;

        Vector3 pos = new Vector3(_startPos.x + _spacing * Mathf.Min(i, m_MaxOrder - 1), _startPos.y, 0);
        order.transform.position = pos;
        order.transform.localScale = Vector3.one;

        _allOrderElements.Add(order);
        _currentOrderElements.Add(order);

        return order;

    }

    public bool AddToOrder(ItemMergeElement itemMergeElement)
    {
        foreach(OrderElement orderElement in _allOrderElements)
        {
            if (orderElement.CanAddToOrder(itemMergeElement))
            {
                orderElement.AddToOrder(itemMergeElement);
                return true;
            }
        }
        return false;
    }

    public async void OnCompletedOrder(OrderElement orderElement)
    {
        _currentOrderElements.Remove(orderElement);
        _currentOrderIndex++;
        await UniTask.Delay(500);

        Action action = () =>
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _currentOrderElements.Count; i++)
            {
                Tween moveTween = CommonGameplayAnimation.MoveTo(
                    _currentOrderElements[i].transform,
                    _listPos[i],
                    0.25f
                );
                sequence.Join(moveTween);
            }

            sequence.OnComplete(() =>
            {
                if (_currentOrderIndex >= _orderDatas.Count) return;
                OrderElement order = CreateOrderElement(_currentOrderIndex);
                order.transform.localScale = Vector3.zero;

                Tween scaleTween = CommonGameplayAnimation.ScaleToOneBouncy(
                    order.transform,
                    Vector3.one * 1.3f,
                    0.5f
                );
                //TweenManager.tweens.Add(scaleTween);
            });

            TweenManager.sequences.Add(sequence);
        };

        Tween tween = CommonGameplayAnimation.ScaleToZeroBouncy(orderElement.Center, Vector3.one * 1.3f, 0.5f, action);
        TweenManager.tweens.Add(tween);
    }
    public void OnReset()
    {
        foreach(OrderElement orderElement in _allOrderElements)
        {
            orderElement.OnReset();
            PoolingManager.instance.Release(orderElement);
        }

        _allOrderElements.Clear();
        _currentOrderIndex = 0;
        _orderDatas.Clear();
        _listPos.Clear();
    }
}
