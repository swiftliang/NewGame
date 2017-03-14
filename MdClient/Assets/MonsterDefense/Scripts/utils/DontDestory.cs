using UnityEngine;
using System.Collections;

public class DontDestory : MonoBehaviour 
{

	// Use this for initialization
	void Awake ()
    {
        DontDestroyOnLoad(gameObject);
	}
	
}
