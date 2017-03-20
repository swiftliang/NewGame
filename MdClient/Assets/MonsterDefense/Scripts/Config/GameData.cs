using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NW;

public class GameData : SingletonBehaviour<GameData>
{
    protected int coin;
    protected int[] stars;
    protected string[] skills;

    public void SetGameData(GameInfo gif)
    {
        Coin = gif.gameInfo.coin;
        if (gif.gameInfo.levels.Length > 0)
        {
            string[] levels = gif.gameInfo.levels.Split(',');
            this.stars = new int[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                this.stars[i] = int.Parse(levels[i]);
            }
        }
        else
        {
            this.stars = null;
        }
        if (gif.gameInfo.skills.Length > 0)
        {
            this.skills = gif.gameInfo.skills.Split(',');
        }
        else
        {
            this.skills = null;
        }
    }

    public int GetLevelStar(int level)
    {
        if(this.stars != null && level < this.stars.Length)
        {
            return this.stars[level];
        }
        return 0;
    }


    public int Coin
    {
        get
        {
            return this.coin;
        }
        set
        {
            this.coin = value;
        }
    }
}
