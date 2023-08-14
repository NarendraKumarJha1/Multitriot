using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FastMobileDOF : MonoBehaviour {
	[Range(0, 10)]
	public float BlurAmount = 1f;
	public float Focus = 0f;
	public float Aperture = 1f;
	static readonly int scrWidth=Screen.width/4;
	static readonly int scrHeight=Screen.height/4;
	static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
	static readonly int blurTexString = Shader.PropertyToID("_BlurTex");
	static readonly int focusAmountString = Shader.PropertyToID("_Focus");
	static readonly int apertureAmountString = Shader.PropertyToID("_Aperture");
	public Material material=null;
	void Start()
	{
		
	}
	void  OnRenderImage (RenderTexture source ,   RenderTexture destination){
		Shader.SetGlobalFloat(focusAmountString, Focus);
		Shader.SetGlobalFloat(apertureAmountString, Aperture);
		material.SetFloat(blurAmountString, BlurAmount);
		RenderTexture buffer = RenderTexture.GetTemporary(scrWidth, scrHeight, 0,source.format);
		Graphics.Blit(source, buffer, material,0);

		RenderTexture temp = RenderTexture.GetTemporary(scrWidth/2, scrHeight/2, 0, source.format);
		Graphics.Blit(buffer, temp, material, 0);
		RenderTexture.ReleaseTemporary(buffer);

		RenderTexture temp2 = RenderTexture.GetTemporary(scrWidth, scrHeight, 0, source.format);
		Graphics.Blit(temp, temp2, material, 0);
		RenderTexture.ReleaseTemporary(temp);

		material.SetTexture(blurTexString, temp2);
		Graphics.Blit(source, destination, material,1);
		RenderTexture.ReleaseTemporary(temp2);

	}

}
