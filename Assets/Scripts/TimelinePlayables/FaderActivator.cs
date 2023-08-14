using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderActivator : MonoBehaviour {
	

	private void OnEnable() {
		ScreenFader.Instance.FadeInOut(1,0.5f);
	}
}
