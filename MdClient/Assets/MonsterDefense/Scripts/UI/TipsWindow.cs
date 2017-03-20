using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsWindow : DestroyObjWithTime
{

    public Text txtTips;
	// Use this for initialization
	void Start () {
		
	}

    public void SetTip(string strTip)
    {
        txtTips.text = strTip;
    }
}
