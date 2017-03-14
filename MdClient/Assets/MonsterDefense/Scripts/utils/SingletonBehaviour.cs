using UnityEngine;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T inst;

    public static T Instance
    {
        get
        {
            if (inst == null)
                inst = FindObjectOfType<T>();

            return inst;
        }
    }

	void Awake () 
    {
        inst = this as T;
        Init();
	}

    protected virtual void Init()
    {

    }
}
