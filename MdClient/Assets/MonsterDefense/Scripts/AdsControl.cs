using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

#if ADS_PLUGIN
using UnityEngine.SocialPlatforms;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
#endif

public class AdsControl : MonoBehaviour
{
	
	
	protected AdsControl ()
	{
	}

	private static AdsControl _instance;

	#if ADS_PLUGIN
	ShowOptions options;
	InterstitialAd interstitial;
	#endif

	public string AdmobID_Android, AdmobID_IOS, UnityID_Android, UnityID_IOS, UnityZoneID;

	public static AdsControl Instance { get { return _instance; } }

	void Awake ()
	{
		
		if (FindObjectsOfType (typeof(AdsControl)).Length > 1) {
			Destroy (gameObject);
			return;
		}
		
		_instance = this;
		MakeNewInterstial ();

		
		DontDestroyOnLoad (gameObject); //Already done by CBManager

		#if ADS_PLUGIN

		if (Advertisement.isSupported) { // If the platform is supported,
			#if UNITY_IOS
			Advertisement.Initialize (UnityID_IOS); // initialize Unity Ads.
			#endif

			#if UNITY_ANDROID
			Advertisement.Initialize (UnityID_Android); // initialize Unity Ads.
			#endif
		}
		options = new ShowOptions ();
		options.resultCallback = HandleShowResult;

		#endif

	}


	public void HandleInterstialAdClosed (object sender, EventArgs args)
	{
	
		#if ADS_PLUGIN

		if (interstitial != null)
			interstitial.Destroy ();
		MakeNewInterstial ();
	
		#endif
		
	}

	void MakeNewInterstial ()
	{
		#if ADS_PLUGIN

#if UNITY_ANDROID
		interstitial = new InterstitialAd (AdmobID_Android);
#endif
#if UNITY_IPHONE
		interstitial = new InterstitialAd (AdmobID_IOS);
#endif
		interstitial.OnAdClosed += HandleInterstialAdClosed;
		AdRequest request = new AdRequest.Builder ().Build ();
		interstitial.LoadAd (request);

		#endif
	}


	public void showAds ()
	{
		
		#if ADS_PLUGIN
		interstitial.Show ();
		#endif
	

	}


	public bool GetRewardAvailable ()
	{
		bool avaiable = false;

		return avaiable;
	}

	public void ShowRewardVideo ()
	{
		#if ADS_PLUGIN
		Advertisement.Show (UnityZoneID, options);
	
		#endif
		
	}

	public void HideBannerAds ()
	{
	}

	public void ShowBannerAds ()
	{
	}
	#if ADS_PLUGIN
	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			int money = PlayerPrefs.GetInt (GameSetting.MONEY_KEY);
			SoundManager.instance.playSoundBuy ();
			money += 10;
			PlayerPrefs.SetInt (GameSetting.MONEY_KEY, money);
				//PlayerPrefs.SetInt(keySave, GameSetting.TRUE_RESULT);
				//setupItem();
			FindObjectOfType<Map> ().updateMoney ();
			break;
		case ShowResult.Skipped:
			break;
		case ShowResult.Failed:
			break;
		}
	}
	#endif
}

