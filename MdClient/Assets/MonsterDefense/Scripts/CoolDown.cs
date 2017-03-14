using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoolDown : MonoBehaviour {

    public float timeCoolDown;
    private float timeCoolDownPre;
    private Image image;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
	}
	
    void OnEnable()
    {
        timeCoolDownPre = timeCoolDown;
    }

	// Update is called once per frame
	void Update () {
        timeCoolDownPre -= Time.deltaTime;
        image.fillAmount = timeCoolDownPre / timeCoolDown;
        if (timeCoolDownPre < 0)
            gameObject.SetActive(false);
	}
}
