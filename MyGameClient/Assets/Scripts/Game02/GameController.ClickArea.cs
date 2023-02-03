using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

namespace Game02
{
    public partial class GameController : IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private FruitItem CurSelectItem = null;
        private FruitItem LastSelectItem = null;
        private int CurPointerId;

        private bool isMouseDown = false;

        public void OnPointerDown(PointerEventData InEventData)
        {
            if (Input.touchCount > 1) return;
            CurPointerId = InEventData.pointerId;
            if (InEventData.pointerEnter != null)
            {
                var temp = InEventData.pointerEnter.GetComponentInParent<FruitItem>();
                if (temp)
                {
                    temp.Selected();
                    if(LastSelectItem == null)
                    {
                        LastSelectItem = temp;
                    }
                    else
                    {
                        CurSelectItem = temp;
                        if (CurSelectItem != null && LastSelectItem != null)
                        {
                            if (canExchange(CurSelectItem, LastSelectItem))
                            {
                                StartCoroutine(ExchangeAndMatch());
                            }
                            else
                            {
                                LastSelectItem.UnSelected();
                                LastSelectItem = CurSelectItem;
                                CurSelectItem = null;
                            }
                        }
                    }
                    OnMouseDown(temp);
                }
            }
        }

        public void OnDrag(PointerEventData InEventData)
        {
            if (Input.touchCount > 1) return;
            if (LastSelectItem == null) return;

            if (InEventData.pointerId == CurPointerId)
            {
                float m_fingerMoveX = 0, m_fingerMoveY = 0;
                if (Input.GetMouseButton(0))
                {
                    m_fingerMoveX = Input.GetAxis("Mouse X");
                    m_fingerMoveY = Input.GetAxis("Mouse Y");
                }
                Debug.Log("m_fingerMoveX：" + m_fingerMoveX);
                Debug.Log("m_fingerMoveY：" + m_fingerMoveY);
                // 滑动量太小，不处理
                if (Mathf.Abs(m_fingerMoveX) < 0.1f && Mathf.Abs(m_fingerMoveY) < 0.1f)
                    return;

                if (Mathf.Abs(m_fingerMoveX) > Mathf.Abs(m_fingerMoveY))
                {
                    CurSelectItem = GetFruitItem(LastSelectItem.rowIndex, LastSelectItem.columnIndex + (m_fingerMoveX > 0 ? 1 : -1));
                }
                else if (Mathf.Abs(m_fingerMoveX) < Mathf.Abs(m_fingerMoveY))
                {
                    CurSelectItem = GetFruitItem(LastSelectItem.rowIndex + (m_fingerMoveY > 0 ? 1 : -1), LastSelectItem.columnIndex);
                }

                if (CurSelectItem != null && LastSelectItem != null)
                {
                    StartCoroutine(ExchangeAndMatch());
                }
            }
        }

        public void OnPointerUp(PointerEventData InEventData)
        {
            if (isMouseDown)
            {
                OnMouseUp();
            }
        }

        public void OnMouseDown(FruitItem item)
        {
            isMouseDown = true;
            if(item.ItemType != ItemType.EliminatedCell)
            {
                
            }
        }

        public void OnMouseUp()
        {
            isMouseDown = false;
        }

        IEnumerator ExchangeAndMatch()
        {
            InputLockManager.Instance.Lock();
            FruitItem item1 = CurSelectItem;
            FruitItem item2 = LastSelectItem;
            CurSelectItem = null;
            LastSelectItem = null;

            item1.UnSelected();
            item2.UnSelected();

            yield return Exchange(item1, item2);
            if (checkHorizontalMatch() || checkVerticalMatch())
            {
                yield return autoCheckMap();
            }
            else
            {
                // 没有任何水果匹配，交换回来
                yield return Exchange(item1, item2);
            }
            InputLockManager.Instance.UnLock();
        }

        IEnumerator Exchange(ItemBase item1, ItemBase item2)
        {
            int tmp = 0;
            tmp = item1.rowIndex;
            item1.rowIndex = item2.rowIndex;
            item2.rowIndex = tmp;

            tmp = item1.columnIndex;
            item1.columnIndex = item2.columnIndex;
            item2.columnIndex = tmp;

            refreshItem(item1);
            refreshItem(item2);

            yield return new WaitForSeconds(movedownTime);
        }

        private bool canExchange(ItemBase item1, ItemBase item2)
        {
            return (Mathf.Abs(item1.rowIndex - item2.rowIndex) + Mathf.Abs(item1.columnIndex - item2.columnIndex))==1;
        }
    }
}
