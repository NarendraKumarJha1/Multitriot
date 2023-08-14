using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerVehicleVFX : MonoBehaviour {
    private List<Material> allMats = new List<Material> ();
    public AudioClip dissolveFwdClip;
    public AudioClip dissolveBackwardClip;
    private AudioSource thisAudioSource;

    private Projector shadowPorjector;
    private float shadowPorjectorFarClipPlane;
    private void Start () {
        if (GetComponent<AudioSource> ())
            thisAudioSource = GetComponent<AudioSource> ();
        else {
            this.gameObject.AddComponent (typeof (AudioSource));
            thisAudioSource = GetComponent<AudioSource> ();
            thisAudioSource.playOnAwake = false;
            thisAudioSource.loop = false;
            thisAudioSource.volume = 0.6f;
        }
        shadowPorjector = GetComponentInChildren<Projector> ();
        shadowPorjectorFarClipPlane = shadowPorjector.farClipPlane;
        CacheAllMaterials ();
        SetDissolveAmount (1);

        // PlayDissolveBackward ();
    }
    public void CacheAllMaterials () {
        foreach (Renderer _renderer in GetComponentsInChildren<Renderer> ()) {
            foreach (Material mat in _renderer.materials) {
                if (mat.shader.name.ToLower ().Contains ("dissolve") && !allMats.Contains (mat))
                    allMats.Add (mat);
            }
            // allMats.AddRange (_renderer.materials);
        }
    }

    public void SetDissolveAmount (float disAmount) {
        foreach (Material mat in allMats) {
            mat.SetFloat ("_SliceAmount", disAmount);
        }
        if (disAmount >= 1)
            shadowPorjector.farClipPlane = 0;

    }

    public void PlayDissolveBackward (float duration = 1f) {

        foreach (Material mat in allMats) {

            mat.SetFloat ("_SliceAmount", 1);
            mat.DOFloat (0, "_SliceAmount", duration);
            // Debug.Log ("In PlayDissolveBackward====================================");
            // shadowPorjector.farClipPlane = 
        }
        DOTween.To (x => shadowPorjector.farClipPlane = x, 0, shadowPorjectorFarClipPlane, duration);
        if (thisAudioSource && dissolveBackwardClip)
            thisAudioSource.PlayOneShot (dissolveBackwardClip);
    }

    public void PlayDissolveForward (float duration = 1f) {
        foreach (Material mat in allMats) {
            mat.SetFloat ("_SliceAmount", 0);
            mat.DOFloat (1, "_SliceAmount", duration);
        }
        DOTween.To (x => shadowPorjector.farClipPlane = x, shadowPorjectorFarClipPlane, 0, duration);
        if (thisAudioSource && dissolveFwdClip)
            thisAudioSource.PlayOneShot (dissolveFwdClip);
    }
}