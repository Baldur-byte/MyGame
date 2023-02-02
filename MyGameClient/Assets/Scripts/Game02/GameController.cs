using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game02
{
    public partial class GameController : MonoBehaviour
    {
        private Transform FruitRoot;

        private List<List<ItemBase>> items;

        private List<GameObject> itemObjectPool;

        private List<ItemBase> itemsToEliminate;

        private float eliminatedTime = 0.2f;

        private float movedownTime = 0.6f;

        private void Awake()
        {
            FruitRoot = transform.Find("FruitRoot").GetComponent<Transform>();
        }

        void Start()
        {
            InitMap();
        }

        
        void Update()
        {

        }

        private void InitMap()
        {
            items = new List<List<ItemBase>>();
            for (int rowIndex = 0; rowIndex < ConfigData.ROW_COUNT; rowIndex++)
            {
                List<ItemBase> temp = new List<ItemBase> ();
                for (int columIndex = 0; columIndex < ConfigData.COLUMN_COUNT; columIndex++)
                {
                    FruitItem item = createRandomCellItem(rowIndex, columIndex);
                    setCellPositonWithAni(item);
                    temp.Add(item);
                }
                items.Add(temp);
            }
            StartCoroutine(autoCheckMap());
        }

        private FruitItem createRandomCellItem(int row, int colum)
        {
            //TODO:待优化
            FruitItem item = Instantiate(Resources.Load<FruitItem>(ConfigData.PREFABS_PATH + "Fruit"), FruitRoot);
            item.InitItem(row, colum);
            return item;
        }

        private void setCellPositonWithAni(ItemBase item, bool isWithAni = false)
        {
            float x = (item.columnIndex - ConfigData.COLUMN_COUNT / 2) * ConfigData.CELL_SIZE + ConfigData.CELL_SIZE / 2;
            float y = (item.rowIndex - ConfigData.ROW_COUNT / 2) * ConfigData.CELL_SIZE + ConfigData.CELL_SIZE / 2;
            //float x = (item.colIndex - ConfigData.COLUMN_COUNT / 2) * ConfigData.CELL_OFFSET + ConfigData.CELL_OFFSET / 2;
            //float y = (item.rowIndex - ConfigData.ROW_COUNT / 2) * ConfigData.CELL_OFFSET + ConfigData.CELL_OFFSET / 2;
            if (isWithAni)
            {
                item.transform.DOLocalMove(new Vector2(x, y), movedownTime);
            }
            else
            {
                item.transform.localPosition = new Vector2(x, y);
            }
        }

        private IEnumerator autoCheckMap()
        {
            InputLockManager.Instance.Lock();
            if (checkHorizontalMatch() || checkVerticalMatch())
            {
                // 消除匹配的
                RemoveMatchCell();
                yield return new WaitForSeconds(eliminatedTime);

                //上面的掉落下来，
                DropDownOtherCell();

                itemsToEliminate.Clear();

                yield return new WaitForSeconds(movedownTime);
                yield return autoCheckMap();
            }
            yield return null;
            InputLockManager.Instance.UnLock();
        }

        private void RemoveMatchCell()
        {
            for (int i = 0; i < itemsToEliminate.Count; i++)
            {
                itemsToEliminate[i].Eliminated();
            }
        }

        private void DropDownOtherCell()
        {
            for (int i = 0; i < itemsToEliminate.Count; i++)
            {
                if (itemsToEliminate[i].ItemType == ItemType.EliminatedCell)
                {
                    var fruit = itemsToEliminate[i] as FruitItem;
                    for (int j = fruit.rowIndex + 1; j < ConfigData.ROW_COUNT; ++j)
                    {
                        var item = GetFruitItem(j, fruit.columnIndex);
                        item.rowIndex--;
                        refreshItem(item);
                    }
                    ReuseRemovedCell(fruit);
                }
            }
        }

        private void ReuseRemovedCell(FruitItem item)
        {
            Destroy(item.gameObject);
            item = createRandomCellItem(ConfigData.ROW_COUNT, item.columnIndex);
            setCellPositonWithAni(item);
            item.rowIndex--; 
            refreshItem(item);
        }

        private bool checkHorizontalMatch()
        {
            bool isMatch = false;
            for (int rowIndex = 0; rowIndex < ConfigData.ROW_COUNT; rowIndex++)
            {
                for (int columIndex = 0; columIndex < ConfigData.COLUMN_COUNT - 2; columIndex++)
                {
                    var item1 = GetFruitItem(rowIndex, columIndex);
                    var item2 = GetFruitItem(rowIndex, columIndex + 1);
                    var item3 = GetFruitItem(rowIndex, columIndex + 2);
                    if (item1.FruitType == item2.FruitType && item2.FruitType == item3.FruitType)
                    {
                        isMatch = true;
                        AddMatchItem(item1);
                        AddMatchItem(item2);
                        AddMatchItem(item3);
                    }
                }
            }
            return isMatch;
        }

        private bool checkVerticalMatch()
        {
            bool isMatch = false;
            for (int columIndex = 0; columIndex < ConfigData.COLUMN_COUNT; columIndex++)
            {
                for (int rowIndex = 0; rowIndex < ConfigData.ROW_COUNT - 2; rowIndex++)
                {
                    var item1 = GetFruitItem(rowIndex, columIndex);
                    var item2 = GetFruitItem(rowIndex + 1, columIndex);
                    var item3 = GetFruitItem(rowIndex + 2, columIndex);
                    if (item1.FruitType == item2.FruitType && item2.FruitType == item3.FruitType)
                    {
                        isMatch = true;
                        AddMatchItem(item1);
                        AddMatchItem(item2);
                        AddMatchItem(item3);
                    }
                }
            }
            return isMatch;
        }

        private FruitItem GetFruitItem(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= items.Count) return null;
            var temp = items[rowIndex];
            if (columnIndex < 0 || columnIndex >= temp.Count) return null;
            return temp[columnIndex] as FruitItem;
        }

        private void AddMatchItem(FruitItem fruit)
        {
            if(null == itemsToEliminate)
            {
                itemsToEliminate = new List<ItemBase>();
            }
            if (!itemsToEliminate.Contains(fruit))
            {
                itemsToEliminate.Add(fruit);
            }
        }

        private void refreshItem(ItemBase item)
        {
            items[item.rowIndex][item.columnIndex] = item;
            item.gameObject.name = $"item({item.rowIndex},{item.columnIndex})";
            setCellPositonWithAni(item, true);
        }
    }
}