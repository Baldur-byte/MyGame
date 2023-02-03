using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game02
{
    public class FruitItem : ItemBase
    {
        public int FruitType;
        private ItemState ItemState;

        public Image ShowImg;
        public Image RowCleanImg;
        public Image ColumnCleanImg;
        public Image TypeCleanImg;
        public Image FrozenImg;
        public Image BlockImg;

        public Image SelectedImg;

        public override void Eliminated()
        {
            if(ItemState == ItemState.Normal)
            {
                this.ItemType = ItemType.EliminatedCell;
            }
        }

        public void InitItem(int row, int col, int fruitId = 0, ItemState state = ItemState.Normal)
        {
            base.SetItemPos(row, col);
            this.ItemType = ItemType.NormalCell;
            FruitType = fruitId == 0 ? Random.Range(1, 7) : fruitId;
            Sprite sprite = Resources.Load<Sprite>(ConfigData.FRUITS_PATH + FruitType);
            ShowImg.sprite = sprite;

            ItemState = state;
            UpdateItemState();
            UnSelected();
        }

        public void Selected()
        {
            SelectedImg.gameObject.SetActive(true);
        }

        public void UnSelected()
        {
            SelectedImg.gameObject.SetActive(false);
        }

        public void UpdateItemState()
        {
            RowCleanImg.gameObject.SetActive(false);
            ColumnCleanImg.gameObject.SetActive(false);
            TypeCleanImg.gameObject.SetActive(false);
            FrozenImg.gameObject.SetActive(false);
            BlockImg.gameObject.SetActive(false);
            switch (ItemState)
            {
                case ItemState.RowClean:
                    RowCleanImg.gameObject.SetActive(true);
                    break;
                case ItemState.ColumnClean:
                    ColumnCleanImg.gameObject.SetActive(true);
                    break;
                case ItemState.TypeClean:
                    TypeCleanImg.gameObject.SetActive(true);
                    break;
                case ItemState.Frozen:
                    FrozenImg.gameObject.SetActive(true);
                    break;
                case ItemState.Block:
                    BlockImg.gameObject.SetActive(true);
                    break;
            }
        }

        public void ChangeItemState(ItemState state)
        {
            ItemState = state;
            UpdateItemState();
        }
    }

    public enum ItemState
    {
        Normal,
        RowClean,
        ColumnClean,
        TypeClean,
        Frozen,//需要多次消除,不能移动
        Block,//不能移动
    }
}