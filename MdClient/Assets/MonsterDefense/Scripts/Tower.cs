using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

    public float hpMax;
    public float hpPre;
    public Sprite[] spriteTower; 
    private GameManager _gameManager;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        hpPre = hpMax;
        _gameManager = GameSetting._gameManger;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	
	public void getHit (float hp) {
        if (_gameManager.gameState == GameState.play)
        {
            hpPre -= hp;
            _gameManager._uiManager.updateHp(hpPre, hpMax);

            if (hpPre < hpMax * 1 / 2)
            {
                spriteRenderer.sprite = spriteTower[0];
            }

            if (hpPre < hpMax * 1 / 4)
            {
                spriteRenderer.sprite = spriteTower[1];
            }

            if (hpPre < 0)
            {
                spriteRenderer.sprite = spriteTower[2];
                _gameManager.gameOver();
            }
        }
	}

}
