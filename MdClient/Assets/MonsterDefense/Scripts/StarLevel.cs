using UnityEngine;
using System.Collections;

public class StarLevel : MonoBehaviour {
	
	public GameObject oneStar;
	public GameObject twoStar;
	public GameObject threeStar;

	private int countStar;

	// Use this for initialization
	void Start () {
	
		//countStar = PlayerPrefs.GetInt (GameSetting.STAR_KEY+GetComponent<Unlock>().numberlevel);
        countStar = GameData.Instance.GetLevelStar(GetComponent<Unlock>().numberlevel);
		if (countStar == 1) {
			oneStar.SetActive(true);
		}

		if (countStar == 2) {
			oneStar.SetActive(true);
			twoStar.SetActive(true);
		}

		if (countStar == 3) {
			oneStar.SetActive(true);
			twoStar.SetActive(true);
			threeStar.SetActive(true);
		}
	}
	
}
