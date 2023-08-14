using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class Games{
	public string Name;
	public Sprite Icon;
	public string BundleID;
	public string AppID;

	public Games(string name_c,Sprite icon_c, string bundleid_c, string appid_c){
		Name = name_c;
		Icon = icon_c;
		BundleID = bundleid_c;
		AppID = appid_c;
	}
}

public enum Developers{
	BrilliantGamez,
	Fazbro,
	GamerzStudio,
	AmazingGamez,
	RogueGamez
}

public enum Promo{
	Banner,
	Frame
}

public class GSF_CrossPromotion : MonoBehaviour {

	[Header("Promotion Settings")]
	[Space]
	public bool EnabePromotion = false;
	[Space]
	public Developers Developr;
	public Promo PromoType;
	public float RepeatRate = 5f;

	[Space]
	[Header("UI Panels")]
	public GameObject PromotionPanel;
	public GameObject BannerPanel;
	public GameObject FramePanel;
	public Image Icon_banner;
	public Image Icon_frame;
	public Text Name_banner;

	[Space]
	TextAsset DeveloperFile;
	static bool Initialized;
	static List<Games> PromoGames = new List<Games> ();
	string promoLink = null;
	string promoName = null;
	int index = 0;

	void Start () {
		if (!Initialized){
			GetGames ();
		}
		if (EnabePromotion){
			InvokeRepeating("RepeatPromo", 1, RepeatRate);
			PromotionPanel.SetActive (true);
			if (PromoType == Promo.Banner){
				BannerPanel.SetActive (true);
				FramePanel.SetActive (false);
			} else if (PromoType == Promo.Frame){
				FramePanel.SetActive (true);
				BannerPanel.SetActive (false);
			}
		} else{
			PromotionPanel.SetActive (false);
		}
	}

	void GetGames(){
		DeveloperFile = (TextAsset)Resources.Load ("CrossPromotion/Developers/"+Developr.ToString ());
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (DeveloperFile.text);
		XmlNode gameslist = doc.LastChild;

		for (int i = 0; i < gameslist.ChildNodes.Count; i++) {
			XmlNode game = gameslist.ChildNodes [i];
			string name = game["name"].InnerText;
			string bundleid = game["bundleid"].InnerText;
			string appid = game["appid"].InnerText;
			PromoGames.Add (new Games (name, Resources.Load <Sprite> ("CrossPromotion/Icons/"+Developr.ToString ()+"/"+ bundleid), bundleid, appid));
		}
		Initialized = true;
	}

	void RepeatPromo(){

		if (PromoType == Promo.Frame){
			Icon_frame.sprite = PromoGames[index].Icon;
			promoName = PromoGames[index].Name;
		} else if (PromoType == Promo.Banner){
			Icon_banner.sprite = PromoGames[index].Icon;
			promoName = PromoGames[index].Name;
			Name_banner.text = promoName;
		}

		#if UNITY_ANDROID
		promoLink = "https://play.google.com/store/apps/details?id="+PromoGames[index].BundleID;
		#endif
		#if UNITY_IOS
		promoLink = "https://itunes.apple.com/us/app/id"+PromoGames[index].AppID;
		#endif


		if (index == PromoGames.Count - 1){
			index = 0;
		} else{
			index++;
		}
	}

	public void OpenPromoGame(){
		//ConsoliAds.Instance.LogScreen ("Promo Click "+promoName);
		Application.OpenURL(promoLink);
	}

}
