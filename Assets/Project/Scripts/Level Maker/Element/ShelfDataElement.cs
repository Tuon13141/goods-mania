using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfDataElement : MonoBehaviour
{
    [Header("Row List")]
    public List<RowDataElement> rowDataElements = new List<RowDataElement>();

    [SerializeField, ShowIf("AlwaysFalse")] Transform m_Holder;
    public Transform Holder => m_Holder;    
    [SerializeField, ShowIf("AlwaysFalse")] Transform m_StartRowPosition;
    public Transform StartRowPosition => m_StartRowPosition;
    [SerializeField, ShowIf("AlwaysFalse")] float m_RowSpacing;
    public float RowSpacing => m_RowSpacing;
    [SerializeField, ShowIf("AlwaysFalse")] RowDataElement m_RowDataElementPrefab;
    public RowDataElement RowDataElementPrefab => m_RowDataElementPrefab;
    private bool AlwaysFalse() => false;

    [Button("Spawn Rows")]
    public void SpawnRows()
    {
        RowDataElement row = Instantiate(m_RowDataElementPrefab);
        row.transform.SetParent(m_Holder);
        row.transform.localPosition = m_StartRowPosition.localPosition + new Vector3(0, 0, (m_RowSpacing * rowDataElements.Count));
        row.name = "RowDataElement_" + rowDataElements.Count;

        row.SetShelfDataElement(this);
        row.SetUp();

        rowDataElements.Add(row);
    }

 
    [HorizontalLine(1, EColor.Blue)]
    [SerializeField] int m_RowAtIndex = 0;
    [Button("Spawn Row At Position")]
    public void SpawnRowAtPosition()
    {
        if (m_RowAtIndex < 0 || m_RowAtIndex > rowDataElements.Count)
        {
            Debug.LogError("Invalid index for spawning row: " + m_RowAtIndex);
            return;
        }

        RowDataElement row = Instantiate(m_RowDataElementPrefab);
        row.transform.SetParent(m_Holder);
        Vector3 position = m_StartRowPosition.localPosition + new Vector3(0, 0, (m_RowSpacing * m_RowAtIndex));
        row.transform.localPosition = position;
        row.name = "RowDataElement_" + m_RowAtIndex;
        rowDataElements.Insert(m_RowAtIndex, row);
        row.transform.SetSiblingIndex(m_RowAtIndex + 2);

        row.SetShelfDataElement(this);
        row.SetUp();

        for (int i = m_RowAtIndex + 1; i < rowDataElements.Count; i++)
        {
            rowDataElements[i].transform.localPosition = m_StartRowPosition.localPosition +
                                                        new Vector3(0, 0, (m_RowSpacing * i));
            rowDataElements[i].name = "RowDataElement_" + i;
            rowDataElements[i].transform.SetSiblingIndex(i + 2);
        }
    }

    [Button("Delete Row At Index")]
    public void DeleteRowAtIndex()
    {
        if (m_RowAtIndex < 0 || m_RowAtIndex >= rowDataElements.Count)
        {
            Debug.LogError("Invalid index for clearing row: " + m_RowAtIndex);
            return;
        }
        RowDataElement rowToClear = rowDataElements[m_RowAtIndex];
        if (rowToClear != null)
        {
            DestroyImmediate(rowToClear.gameObject);
            rowDataElements.RemoveAt(m_RowAtIndex);
        }

        for (int i = m_RowAtIndex; i < rowDataElements.Count; i++)
        {
            rowDataElements[i].transform.localPosition = m_StartRowPosition.localPosition +
                                                        new Vector3(0, 0, (m_RowSpacing * i));
            rowDataElements[i].name = "RowDataElement_" + i;
            rowDataElements[i].transform.SetSiblingIndex(i + 2);
        }
    }

    [Button("Delete All Rows")]
    public void DeleteAllRows()
    {
        foreach (RowDataElement row in rowDataElements)
        {
            if (row == null) continue;
            DestroyImmediate(row.gameObject);
        }
        rowDataElements.Clear();
    }
    [Button("Delete Shelf")]
    public void DeleteShelf()
    {
        LevelDataController.instance.DeleteShelf(this);
    }

    public void DeleteRow(RowDataElement row)
    {
        if (row == null) return;
        rowDataElements.Remove(row);
        DestroyImmediate(row.gameObject);
    }
}
