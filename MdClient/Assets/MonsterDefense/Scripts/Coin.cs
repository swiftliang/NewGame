using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    private GameManager _gameManager;
    public int bonus;
	// Use this for initialization
	void Start () {
        _gameManager = GameSetting._gameManger;
	}

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("arrow"))
        {
            _gameManager.updateMoney(bonus);
            Destroy(gameObject);
        }
    }
}
