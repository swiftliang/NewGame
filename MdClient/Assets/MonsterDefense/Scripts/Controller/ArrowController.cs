using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowController : MonoBehaviour {

    public float speedMove;
    private Rigidbody2D rigi;
    public float dame;
    [HideInInspector]
    public bool canAttack;
    // Use this for initialization
    void Awake () {
        rigi = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        canAttack = true;
        rigi.velocity = transform.up.normalized * speedMove;
    }

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D col) {
        if (col.name.Equals("Background"))
        {
            gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("enemy"))
        {
            gameObject.SetActive(false);
            if (canAttack)
                col.GetComponent<EnemyController>().takeHit(dame, 0.2f);
        }
    }

}
