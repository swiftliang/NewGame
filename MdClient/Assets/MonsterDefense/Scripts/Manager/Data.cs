using UnityEngine;
using System.Collections.Generic;

public class Data : MonoBehaviour {

    public static Data instance;
    [HideInInspector]
    public Sprite backGround;
    [HideInInspector]
    public List<Enemy> listEnemy;
    [HideInInspector]
    public int level;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

    }

    public void setData(List<Enemy> data, Sprite backGround)
    {
        listEnemy = data;
        this.backGround = backGround;
    }

}
