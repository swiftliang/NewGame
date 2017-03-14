using UnityEngine;
using System.Collections;

public enum TypeAttack { normal, magic};

public class EnemyAttack : StateMachineBehaviour {

    private Tower tower;
    public TypeAttack typeAttack;

	// Use this for initialization
	void OnEnable () {
        tower = GameSetting._tower;
	}

    // Update is called once per frame
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attack(animator);
    }

    void attack(Animator animator) {
        switch (typeAttack)
        {
            case TypeAttack.normal:
                SoundManager.instance.playSoundEnemyAttack();
                tower.getHit(animator.GetComponent<EnemyController>().dame);
                break;
            case TypeAttack.magic:
                SoundManager.instance.playSoundEnemyAttack();
                ArrowController[] arrow = FindObjectsOfType<ArrowController>();
                foreach(ArrowController weapon in arrow)
                {
                    weapon.canAttack = false;
                }
                break;
        }
    }
}
