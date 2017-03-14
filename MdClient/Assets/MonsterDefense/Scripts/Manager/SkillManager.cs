using UnityEngine;
using System.Collections;

public enum Skill { ice,boom,lighting};

public class SkillManager : MonoBehaviour {

    public GameObject effect;

    public float timeStopWithIce = 3;
    public float dame;
    public Skill skill;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("enemy"))
        {
            StopCoroutine(genEffect(effect, col.transform, timeStopWithIce));
            StartCoroutine(genEffect(effect, col.transform, timeStopWithIce));
            col.GetComponent<EnemyController>().takeHit(dame, timeStopWithIce);
        }
    }

    IEnumerator genEffect(GameObject effect,Transform pointGen,float timeStop)
    {
        if (effect != null)
        {
            switch (skill)
            {
                case Skill.ice:
                    GameObject ice = (GameObject)Instantiate(effect, pointGen.position, pointGen.rotation);
                    ice.GetComponent<SpriteRenderer>().sortingOrder = pointGen.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    ice.GetComponent<DestroyObjWithTime>().startDestroy(timeStop);
                    break;
                case Skill.boom:
                    GameObject boom = (GameObject)Instantiate(effect, pointGen.position, pointGen.rotation);
                    boom.GetComponent<SpriteRenderer>().sortingOrder = pointGen.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    boom.GetComponent<DestroyObjWithTime>().startDestroy(timeStop);
                    break;
                case Skill.lighting:
                    GameObject lighting = (GameObject)Instantiate(effect, pointGen.position, pointGen.rotation);
                    pointGen.GetComponent<EnemyController>().hitLighting(timeStop);
                    lighting.GetComponent<SpriteRenderer>().sortingOrder = pointGen.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    lighting.GetComponent<DestroyObjWithTime>().startDestroy(timeStop);
                    break;
            }
            
        }
        yield return new WaitForSeconds(timeStop);
        Destroy(gameObject);
    }

}
