using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip rightClip, leftClip;

    private void OnEnable()
    {
        this.transform.parent.transform.Find("All Audio Sources").gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        this.transform.parent.transform.Find("All Audio Sources").gameObject.SetActive(true);
    }

    public void RightStepSound()
    {
        source.PlayOneShot(rightClip);
    }

    public void LeftStepSound()
    {
        source.PlayOneShot(leftClip);
    }
}
