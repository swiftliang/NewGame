using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenManager : MonoBehaviour {

    public static GenManager currentPool;
    [HideInInspector]
    public int levelArrow;
    private GameObject arrow;
    public ArrowUpgrade[] listArrowUpgrade;
    public int countArrow;
    public bool isGrown;
    private List<GameObject> ArrowList;
    private GameManager _gameManager;
    // Use this for initialization
    void Start () {
        currentPool = this;
        _gameManager = GameSetting._gameManger;
        listArrowUpgrade = GameSetting.instance.listArrowUpgrade;
        arrow = listArrowUpgrade[0].arrow;
        ArrowList = new List<GameObject>();

        if (countArrow != 0)
            for (int i = 0; i < countArrow; i++)
            {
                GameObject obj = (GameObject)Instantiate(arrow);
                obj.SetActive(false);
                ArrowList.Add(obj);
            }
    }

    public GameObject GetArrow()
    {
        for (int i = 0; i < ArrowList.Count; i++)
        {
            if (!ArrowList[i].activeInHierarchy)
                return ArrowList[i];
        }

        if (isGrown)
        {
            GameObject obj = (GameObject)Instantiate(arrow);
            obj.SetActive(false);
            ArrowList.Add(obj);

            for (int i = 0; i < ArrowList.Count; i++)
            {
                if (!ArrowList[i].activeInHierarchy)
                    return ArrowList[i];
            }
        }

        return null;
    }

    public void upgradeArrow()
    {
        if (levelArrow < listArrowUpgrade.Length - 1)
        {
            levelArrow++;
            arrow = listArrowUpgrade[levelArrow].arrow;
            reset(ArrowList);
        }
    }

    void reset(List<GameObject> obj)
    {
        for (int i = 0; i < obj.Count; i++)
        {
            Destroy(obj[i]);
        }
        obj.Clear();
    }
}
[System.Serializable]
public class ArrowUpgrade
{
    public GameObject arrow;
    public int cost;
}