using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsentScript : MonoBehaviour
{

	public string PrivacyPolicyLink;
	public string TermConditionsLink;
	public string SplashScreen = "SplashScreen";

	private GameObject dialog;


	public IEnumerator Start ()
	{
		yield return new WaitForSeconds(0.5f);
		dialog = transform.GetChild (0).gameObject;
		if (PlayerPrefs.GetInt ("IsFirsttime", 0) == 0)
		{
			dialog.SetActive (true);
		} else
		{
			No ();
		}
	}


	public void openPrivacyPolicy ()
	{
		Application.OpenURL (PrivacyPolicyLink);
	}

	public void openTermPolicy ()
	{
		Application.OpenURL (TermConditionsLink);
	}




	public void Yes ()
	{
		PlayerPrefs.SetInt ("IsFirsttime", 1);
		dialog.SetActive (false);
		Application.LoadLevel (SplashScreen);
	}

	public void No ()
	{
		dialog.SetActive (false);
		Application.LoadLevel (SplashScreen);
	}

}