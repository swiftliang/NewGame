using UnityEngine;
using System.Collections;

public class DestroyObjWithTime : MonoBehaviour {

    public void startDestroy(float time)
    {
        StartCoroutine(destroy(time));
    }

	IEnumerator destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
