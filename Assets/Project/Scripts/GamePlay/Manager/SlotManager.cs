using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    List<SlotElement> _allSlotElements = new List<SlotElement>();
    List<SlotElement> _currentSlotElements = new List<SlotElement>();

    [SerializeField] Transform m_SlotParent;

    [Header("Slot Display Settings")]
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;
    float _spacing;
    Vector3 _startPos;
    [SerializeField] float m_MaxSpacing = 6f;
    [SerializeField] int m_MaxSlot = 5;

    private void Start()
    {
        (_startPos, _spacing) = CommonTransformCalculator.GetStartPointAndSpacingReverse(m_EndPoint, m_StartPoint, m_MaxSlot, m_MaxSpacing);

        for (int i = 0; i < m_MaxSlot; i++)
        {
            SlotElement slotElement = PoolingManager.instance.Get(typeof(SlotElement)) as SlotElement;

            slotElement.transform.SetParent(m_SlotParent);
            slotElement.SetUp();
            slotElement.gameObject.name = $"SlotElement";

            Vector3 pos = new Vector3(_startPos.x + _spacing * i, _startPos.y, 0);
            slotElement.transform.position = pos;
            slotElement.transform.localScale = Vector3.one;

            _allSlotElements.Add(slotElement);
            _currentSlotElements.Add(slotElement);
        }
    }

    public void SetUp()
    {
        
    }
}
