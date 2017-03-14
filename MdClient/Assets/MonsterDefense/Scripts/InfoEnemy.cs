using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoEnemy : MonoBehaviour {

    public Image icon;
    public Text txtname;
    public Text describe;

    public Info[] info;
    private int index;

    void OnEnable()
    {
        setInfo(info[index].icon, info[index].name, info[index].describe);
    }
    // Use this for initialization
    public void next()
    {
        index++;
        if (index >= info.Length)
        {
            index = 0;
        }
        setInfo(info[index].icon, info[index].name, info[index].describe);
    }
	
	// Update is called once per frame
	public void pre () {
        index--;
        if (index < 0)
        {
            index = info.Length-1;
            Debug.Log(index + "");
        }
        setInfo(info[index].icon, info[index].name, info[index].describe);
    }

    void setInfo(Sprite icon,string name,string describe)
    {
        this.icon.sprite = icon;
        this.icon.SetNativeSize();
        this.txtname.text = name;
        this.describe.text = describe;
    }
}

[System.Serializable]
public class Info
{
    public string name;
    public Sprite icon;
    public string describe;
}
