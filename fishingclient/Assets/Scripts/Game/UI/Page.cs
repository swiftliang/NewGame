using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fishing.Net;
namespace Fishing.UI
{
    public class Page : MonoBehaviour
    {
        public RectTransform rectTrans;
        private List<Table> _tableList = new List<Table>();
        private HallWindow _hallWindow;
        public void init(HallWindow hallWindow)
        {
            rectTrans = GetComponent<RectTransform>();
            _hallWindow = hallWindow;
            int i;
            for (i = 0; i < transform.childCount; i++)
            {
                Table table = transform.GetChild(i).gameObject.AddComponent<Table>();
                table.Init(hallWindow);
                _tableList.Add(table);
            }
        }
        public void FillTables(List<TableInfo> tableInfoList)
        {
            int i;
            for (i = 0; i < _hallWindow.NUMBER_OF_TABLE_PER_PAGE; i++)
            {
                if (i < tableInfoList.Count)
                    _tableList[i].FillTable(tableInfoList[i]);
                else
                    _tableList[i].SetActive(false);
            }
        }
    }
}
