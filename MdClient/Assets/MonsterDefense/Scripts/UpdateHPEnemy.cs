using UnityEngine;
using System.Collections;

public class UpdateHPEnemy : MonoBehaviour {

    public float timeShowHp = 1;
    private float saveScaleX;
	// Use this for initialization
	void Start () {
        saveScaleX = transform.localScale.x;
        GetComponent<SpriteRenderer>().sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
	}

	// Update is called once per frame
	public void updateHP (float pre) {
        if (pre >= 0)
            transform.localScale = new Vector2(pre * saveScaleX, transform.localScale.y);
        else
            transform.localScale = new Vector2(0, transform.localScale.y);
    }

}
