using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneVoiceOver : MonoBehaviour {
    private AudioSource thisAudioSource;
    public float playDelay = 1f;
    IEnumerator Start () {
        thisAudioSource = GetComponent<AudioSource> ();

        yield return new WaitForSecondsRealtime (playDelay);
        thisAudioSource.Play ();

    }

}