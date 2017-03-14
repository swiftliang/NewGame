using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public Transform[] pointGenEnemy;
    [HideInInspector]
    public List<Enemy> listEnemy;

    public static int countAllEnemy;

    private float delayGen;
    private GameObject prefab;
    private int countEnemy;
    private float timeDelay;
    void Start () {
        listEnemy = new List<Enemy>();
        getEnemy(Data.instance.listEnemy);
        StartCoroutine(genEnemy());
	}

    void callEnemy()
    {
        // gen random enemy
        int countEnemy = listEnemy.Count;

        if (countEnemy > 0)
        {
            int a = Random.Range(0, countEnemy);
            if (listEnemy[a].count > 0)
            {
                listEnemy[a].count--;
                // gen enemy with random pointGenEnemy
                int random = Random.Range(0, pointGenEnemy.Length);
                delayGen = listEnemy[a].timeGen;
                GameObject enemy = (GameObject)Instantiate(listEnemy[a].enemy, pointGenEnemy[random].position, pointGenEnemy[random].rotation);
                enemy.GetComponent<SpriteRenderer>().sortingOrder = 5 + pointGenEnemy[random].GetComponent<SpriteRenderer>().sortingOrder * 2;

            }
            else
            {
                delayGen = 0;
                listEnemy.RemoveAt(a);
                callEnemy();
            }
        }

    }

    IEnumerator genEnemy()
    {
        while (true)
        {
            callEnemy();
            yield return new WaitForSeconds(delayGen);
        }
    }

    void getEnemy(List<Enemy> list)
    {
        countAllEnemy = 0;
        foreach (Enemy enemy in list)
        {
            Enemy obj = new Enemy();
            obj.enemy = enemy.enemy;
            obj.count = enemy.count;
            obj.timeGen = enemy.timeGen;
            listEnemy.Add(obj);
            // count enemy
            countAllEnemy += enemy.count;
        }
    }
}

[System.Serializable]
public class Enemy
{
    //prefabs enemy
    public GameObject enemy;
    // count enemy in level
    public int count;
    //time delay
    public float timeGen = 1;
}