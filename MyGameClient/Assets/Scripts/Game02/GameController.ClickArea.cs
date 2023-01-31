using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

namespace Game02
{
    public partial class GameController : IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private ItemBase CurSelectCell = null;
        private ItemBase LastSelectCell = null;
        private int CurPointerId;

        private bool isMouseDown = false;

        public void OnPointerDown(PointerEventData InEventData)
        {
            if (Input.touchCount > 1) return;
            CurPointerId = InEventData.pointerId;
            if (InEventData.pointerEnter != null)
            {
                var temp = InEventData.pointerEnter.GetComponentInParent<ItemBase>();
                if (temp)
                {
                    LastSelectCell = temp;
                    OnMouseDown(temp);
                }
            }
        }

        public void OnDrag(PointerEventData InEventData)
        {
            if (Input.touchCount > 1) return;
            if (LastSelectCell == null) return;

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

                //滑动量太大，取消选择
                if (Mathf.Abs(m_fingerMoveX) > 0.5 || Mathf.Abs(m_fingerMoveY) > 0.5)
                {
                    CurSelectCell = null;
                    return;
                }
                if (Mathf.Abs(m_fingerMoveX) > Mathf.Abs(m_fingerMoveY))
                {
                    CurSelectCell = GetFruitItem(LastSelectCell.rowIndex, LastSelectCell.colIndex + (m_fingerMoveX > 0 ? 1 : -1));
                }
                else if (Mathf.Abs(m_fingerMoveX) < Mathf.Abs(m_fingerMoveY))
                {
                    CurSelectCell = GetFruitItem(LastSelectCell.rowIndex + (m_fingerMoveY > 0 ? 1 : -1), LastSelectCell.colIndex);
                }
            }
        }

        public void OnPointerUp(PointerEventData InEventData)
        {
            if (isMouseDown)
            {
                OnMouseUp();
            }

            LastSelectCell = null;
            CurSelectCell = null;
            CurPointerId = 0;
        }

        public void OnMouseDown(ItemBase item)
        {
            isMouseDown = true;
            if(item.ItemType != ItemType.Eliminated)
            {
                
            }
        }

        public void OnMouseUp()
        {
            isMouseDown = false;
            if(CurSelectCell != null && LastSelectCell != null)
            {
                StartCoroutine(ExchangeAndMatch(CurSelectCell, LastSelectCell));
            }
        }

        IEnumerator ExchangeAndMatch(ItemBase item1, ItemBase item2)
        {
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
        }

        IEnumerator Exchange(ItemBase item1, ItemBase item2)
        {
            items[item1.rowIndex][item1.colIndex] = item2;
            items[item2.rowIndex][item2.colIndex] = item1;

            int tmp = 0;
            tmp = item1.rowIndex;
            item1.rowIndex = item2.rowIndex;
            item2.rowIndex = tmp;

            tmp = item1.colIndex;
            item1.colIndex = item2.colIndex;
            item2.colIndex = tmp;

            item1.gameObject.name = $"item({item1.rowIndex},{item1.colIndex})";
            item2.gameObject.name = $"item({item2.rowIndex},{item2.colIndex})";
            setCellPositonWithAni(item1, true);
            setCellPositonWithAni(item2, true);
            yield return new WaitForSeconds(movedownTime);
        }
    }
}
