using UnityEngine;

namespace Game02
{
    public abstract class ItemBase : MonoBehaviour
    {
        public int rowIndex { get; set; }
        public int columnIndex { get; set; }

        public ItemType ItemType { get; set; }

        public abstract void Eliminated();

        protected void SetItemPos(int row, int col)
        {
            this.rowIndex = row;
            this.columnIndex = col;
        }
    }

    public enum ItemType
    {
        EliminatedCell,

        NormalCell,

        IceCell,//会被附近的消除影响

        BlockCell,//不会被附近的消除影响
    }
}
