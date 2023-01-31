using UnityEngine;

namespace Game02
{
    public abstract class ItemBase : MonoBehaviour
    {
        public int rowIndex { get; set; }
        public int colIndex { get; set; }

        public ItemType ItemType { get; set; }

        public abstract void Eliminated();

        public void SetItem(ItemType itemType, int row, int col)
        {
            this.ItemType = itemType;
            this.rowIndex = row;
            this.colIndex = col;
        }

        protected void attacked()
        {

        }
    }
}
