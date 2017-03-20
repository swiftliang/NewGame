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

        UpdateLevels(gif.gameInfo.levels);
        UpdateSkills(gif.gameInfo.skills);
       
    }

    public void UpdateSkills(string strSkills)
    {
        if (strSkills.Length > 0)
        {
            this.skills = strSkills.Split(',');
        }
        else
        {
            this.skills = null;
        }
    }

    public void UpdateLevels(string strlevels)
    {
        if (strlevels.Length > 0)
        {
            string[] levels = strlevels.Split(',');
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
    }

    public int GetLevelStar(int level)
    {
        if(this.stars != null && level < this.stars.Length)
        {
            return this.stars[level]-1;
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
