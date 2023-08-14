using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof (UnityEngine.UI.Image))]
public class ScreenFader : MonoBehaviour {
    public static ScreenFader Instance { get; set; }

    private UnityEngine.UI.Image _image;

    private void Awake () {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad (this.transform.parent.gameObject);
        } else {
            DestroyImmediate (this.transform.parent.gameObject);
            return;
        }

        _image = GetComponent<UnityEngine.UI.Image> ();
        _image.enabled = false;
    }

    private void Start () {

    }

    public void DoFade (float from, float to, float duration, Action onEnd = null, bool disableFaderOnEnd = true) {
        _image.enabled = true;
        _image.color = new Color (_image.color.r, _image.color.g, _image.color.b, from);
        _image.DOFade (to, duration).OnComplete (() => {
            if (disableFaderOnEnd)
                _image.enabled = false;

            if (onEnd != null)
                onEnd.Invoke ();
        });
    }

    public void FadeIn (float duration, Action onEnd = null, bool disableFaderOnEnd = true) {
        DoFade (0f, 1f, duration, onEnd, disableFaderOnEnd);
    }

    public void FadeOut (float duration, Action onEnd = null, bool disableFaderOnEnd = true) {
        DoFade (1f, 0f, duration, onEnd, disableFaderOnEnd);
    }

    public void FadeInOut (float duration, float inBwDelay, Action inBwAction = null, Action onEnd = null) {
        StartCoroutine (FadeInOutCoroutine (duration, inBwDelay, inBwAction, onEnd));
    }

    private IEnumerator FadeInOutCoroutine (float duration, float inBwDelay, Action inBwAction = null, Action onEnd = null) {
        float d = duration / 2;
        FadeIn (d, () => {
            _image.enabled = true;
        });

        yield return new WaitForSeconds (inBwDelay + d);
        if (inBwAction != null)
            inBwAction.Invoke ();

        FadeOut (d, onEnd);
    }
}