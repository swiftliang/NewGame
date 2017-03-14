using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public float speedMove;
    public float hpMax;
    public float dame;
    public int bonus;
    [Range(0,1)]
    public float speedAttack;
    public Color colorGetHit;
    public Color colorNormal;

    [SerializeField]
    public GameObject skillIce;
    [SerializeField]
    public GameObject skillLighting;
    [SerializeField]
    public GameObject skillBoom;

    private float hp;
    private bool canMove;
    private bool isAttack;
    private bool isDie;
    private SpriteRenderer spRederer;
    private Animator ani;
    private GameManager _gameManager;
    private UpdateHPEnemy hpEnemy;

	// Use this for initialization
	void Start () {
        hp = hpMax;
        _gameManager = GameSetting._gameManger;
        spRederer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        ani.speed = speedAttack;
        hpEnemy = transform.GetChild(0).GetComponent<UpdateHPEnemy>();
        canMove = true;
        isDie = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (canMove && !isAttack && !isDie)
        {
            float offset = speedMove * Time.deltaTime;
            transform.position = new Vector2(transform.position.x + offset, transform.position.y);
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.name.Equals("Tower"))
        {
            isAttack = true;
            ani.SetBool("attack",true);
        }

    }

    public void takeHit(float dame, float timeStop)
    {
        SoundManager.instance.playSoundHit();
        hp -= dame;

        // show hpEnemy
        hpEnemy.updateHP(hp/hpMax);

        if (hp <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            isDie = true;
            ani.SetTrigger("die");
        }
        StartCoroutine(getHit(timeStop));
    }

    // enemy cant walk with timeStop
    IEnumerator getHit(float timeStop)
    {
        if (canMove)
        {
            canMove = false;
            spRederer.color = colorGetHit;
            ani.speed = 0;
            yield return new WaitForSeconds(timeStop);
            canMove = true;
            ani.speed = 1;
            spRederer.color = colorNormal;

            switch (_gameManager.skillState)
            {
                case SkillState.ice:
                    SoundManager.instance.playSoundIceSkill();
                    _gameManager.skillState = SkillState.none;
                    if (skillIce != null)
                        Instantiate(skillIce, transform.position, transform.rotation);
                    break;
                case SkillState.lighting:
                    SoundManager.instance.playSoundLightSkill();
                    _gameManager.skillState = SkillState.none;
                    if (skillLighting != null)
                        Instantiate(skillLighting, transform.position, transform.rotation);
                    break;
                case SkillState.boom:
                    SoundManager.instance.playSoundBombSkill();
                    _gameManager.skillState = SkillState.none;
                    if (skillBoom != null)
                        Instantiate(skillBoom, transform.position, transform.rotation);
                    break;
            }
        }
    }

    // hide enemy when use lighting skill
    public void hitLighting(float time)
    {
        StartCoroutine(getHitLighting(time));
    }

    IEnumerator getHitLighting(float time)
    {
        spRederer.enabled = false;
        yield return new WaitForSeconds(time);
        spRederer.enabled = true;
    }

}
