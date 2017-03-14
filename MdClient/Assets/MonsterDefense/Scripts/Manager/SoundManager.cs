using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;
    [SerializeField]
    public AudioClip shootPlayer;
    [SerializeField]
    public AudioClip lightSkill;
    [SerializeField]
    public AudioClip iceSkill;
    [SerializeField]
    public AudioClip enemyAttack;
    [SerializeField]
    public AudioClip click;
    [SerializeField]
    public AudioClip buy;
    [SerializeField]
    public AudioSource audioSource;
    [SerializeField]
    public AudioClip bombSkill;
    [SerializeField]
    public AudioClip win;
    [SerializeField]
    public AudioClip over;
    [SerializeField]
    public AudioClip hit;

    public float volSFX;

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

	}

    void Start()
    {
        updateVol();
    }

    public void updateVol()
    {
        volSFX = PlayerPrefs.GetFloat(GameSetting.SFX_KEY);
    }

    public void playSoundShoot()
    {
        playAudioClip(shootPlayer, volSFX);
    }

    public void playSoundLightSkill()
    {
        playAudioClip(lightSkill,volSFX);
    }

    public void playSoundIceSkill()
    {
        playAudioClip(iceSkill, volSFX);
    }

    public void playSoundEnemyAttack()
    {
        playAudioClip(enemyAttack, volSFX);
    }

    public void playSoundClick()
    {
        playAudioClip(click, volSFX);
    }

    public void playSoundBuy()
    {
        playAudioClip(buy, volSFX);
    }

    public void playSoundBombSkill()
    {
        playAudioClip(bombSkill, volSFX);
    }

    public void playSoundHit()
    {
        playAudioClip(hit, volSFX);
    }

    public void playSoundOver()
    {
        playAudioClip(over, volSFX);
    }

    public void playSoundWin()
    {
        playAudioClip(win, volSFX);
    }

    void playAudioClip(AudioClip audio,float vol)
    {
        audioSource.PlayOneShot(audio, vol);
    }

}
