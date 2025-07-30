using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    List<ShelfElement> _allShelfElements = new List<ShelfElement>();

    [SerializeField] Transform m_Holder;
    public void SetUp(List<ShelfData> shelfDatas)
    {
        foreach (var shelfData in shelfDatas)
        {
            var shelf = PoolingManager.instance.Get(typeof(ShelfElement)) as ShelfElement;
            shelf.transform.SetParent(m_Holder);
            shelf.SetUp(shelfData, this);


            _allShelfElements.Add(shelf);
        }

        LevelController.instance.cameraController.FitChildrenToBottomPartOfCamera(m_Holder.gameObject);
    }
}
