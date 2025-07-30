using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfElement : MonoBehaviour
{
    [Header("Data")]
    public List<RowData> rowDatas = new List<RowData>();


    [Header("Row Position List")]
    [SerializeField] List<Transform> m_Row_1_Positions = new List<Transform>();
    [SerializeField] List<Transform> m_Row_2_Positions = new List<Transform>();

    ShelfManager _shelfManager;

    [SerializeField] Transform m_ItemHolder;
    public void SetUp(ShelfData shelfData, ShelfManager shelfManager)
    {
        this._shelfManager = shelfManager;

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
                }
                else
                {
                    itemElement.transform.position = m_Row_2_Positions[j].position;
                }
            }
        }
        transform.localPosition = shelfData.spawnPosition;
    }
}
