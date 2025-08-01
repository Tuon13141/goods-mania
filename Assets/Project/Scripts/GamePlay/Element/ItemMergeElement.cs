using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMergeElement : MonoBehaviour
{
    [Header("Data")]
    List<ItemElement> _itemElements = new List<ItemElement>();
    public string itemId => _itemElements.Count > 0 ? _itemElements[0].itemId : string.Empty;

    [SerializeField] Transform m_Holder;

    public void SetUp(List<ItemElement> itemElements)
    {
        _itemElements.AddRange(itemElements);

        foreach(ItemElement itemElement in _itemElements)
        {
            Vector3 savedPosition = itemElement.transform.position;
            itemElement.transform.SetParent(m_Holder, true);
            itemElement.transform.position = savedPosition;
        }

    }

    public void OnBeginAnim(Transform destination, Vector3 offSet, Action onFinishAction = null)
    {
        //Dùng dotween để chia anim thành các đoạn nhỏ sau
        //1. Di chuyển các item đến localMove các item về Vector3.zero và localScale các item về 1f trong khoảng thời gian 0.25s 
        //2. Khi xong 1 thì làm hiệu ứng thu bé lại thành 1 r phóng to ra 1.3f trong 0.25s  
        //3. Sau khi hoàn thành 2 thì di chuyển gameObject.transform về destination - offSet với vận tốc là nào đó và tăng tốc dần 
        //4. Sau khi hoàn thành 3 thì sẽ đổi parent cho các item -> destination và di chuyển từng item về local Vector3.zero trong 0.25s và localScale dần về 1f
        Sequence mainSequence = DOTween.Sequence();

        Sequence stage1 = DOTween.Sequence();
        foreach (ItemElement item in _itemElements)
        {
            if (item != null)
            {
                stage1.Join(item.transform.DOLocalMove(Vector3.zero, 0.125f).SetEase(Ease.Linear));
                stage1.Join(item.transform.DOScale(Vector3.one, 0.125f).SetEase(Ease.Linear));
            }
        }
        mainSequence.Append(stage1);

        Sequence stage2 = DOTween.Sequence();
        stage2.Append(transform.DOScale(0.95f, 0.125f).SetEase(Ease.Linear))
              .Append(transform.DOScale(1.3f, 0.125f).SetEase(Ease.Linear));
        mainSequence.Append(stage2);

        Tween stage3 = transform.DOMove(destination.position - offSet, 0.25f).SetEase(Ease.Linear);
        mainSequence.Append(stage3);

        mainSequence.AppendCallback(() =>
        {
            Sequence stage4 = DOTween.Sequence();
            foreach (ItemElement item in _itemElements)
            {
                if (item != null)
                {
                    item.transform.SetParent(destination);
                    stage4.Join(item.transform.DOLocalMove(Vector3.zero, 0.125f).SetEase(Ease.Linear));
                    stage4.Join(item.transform.DOScale(Vector3.one, 0.125f).SetEase(Ease.Linear));
                }
            }
            stage4.Play();
        });

        mainSequence.OnComplete(() => onFinishAction?.Invoke());
    }

    public void OnReset()
    {
        _itemElements.Clear();
    }
}
