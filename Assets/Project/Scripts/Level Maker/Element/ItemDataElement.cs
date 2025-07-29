using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataElement : MonoBehaviour
{
    [SerializeField] private string m_ItemId;
    public string ItemId => m_ItemId;
}
