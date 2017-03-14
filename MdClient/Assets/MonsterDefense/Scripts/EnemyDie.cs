using UnityEngine;
using System.Collections;

public class EnemyDie : StateMachineBehaviour {

    private GameManager _gameManager;
    [SerializeField]
    public GameObject coin;

    void OnEnable()
    {
        _gameManager = GameSetting._gameManger;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _gameManager.checkWin();
        GameObject obj = (GameObject)Instantiate(coin, animator.transform.position, coin.transform.rotation);
        obj.GetComponent<Coin>().bonus = animator.GetComponent<EnemyController>().bonus;
        obj.GetComponent<SpriteRenderer>().sortingOrder = animator.GetComponent<SpriteRenderer>().sortingOrder;
        Destroy(animator.gameObject);
    }
}
