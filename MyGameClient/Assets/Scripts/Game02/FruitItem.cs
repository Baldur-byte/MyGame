using UnityEngine;

namespace Game02
{
    public class FruitItem : ItemBase
    {
        public override void Eliminated()
        {
            ItemType = ItemType.Eliminated;
        }
    }
}