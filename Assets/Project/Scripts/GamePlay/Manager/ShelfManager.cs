using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    List<ShelfElement> _allShelfElements = new List<ShelfElement>();

    [SerializeField] Transform m_Holder;
    public void SetUp(List<ShelfData> shelfDatas)
    {
        for (int i = 0; i < shelfDatas.Count; i++)
        {
            var shelfData = shelfDatas[i];
            var shelf = PoolingManager.instance.Get(typeof(ShelfElement)) as ShelfElement;
            shelf.transform.SetParent(m_Holder);
            shelf.gameObject.name = "ShelfElement_" + i;
            shelf.SetUp(shelfData, this);
            _allShelfElements.Add(shelf);
        }

        LevelController.instance.cameraController.FitChildrenToBottomPartOfCamera(m_Holder.gameObject);
    }

    public void OnReset()
    {
        m_Holder.transform.localPosition = Vector3.zero;
        m_Holder.transform.localScale = Vector3.one;

        foreach (var shelf in _allShelfElements)
        {
            shelf.OnReset();
            PoolingManager.instance.Release(shelf);
        }
        _allShelfElements.Clear();
    }
}
