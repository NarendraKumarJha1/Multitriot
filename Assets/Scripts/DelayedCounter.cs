using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedCounter : MonoBehaviour
{

    public float countingDuration = 2f;
    public float countingDelay = 0.1f;

    private UnityEngine.UI.Text textUi;
    private Animator animator;
    private AudioSource audioSfx;

    void Start()
    {
        textUi = GetComponent<UnityEngine.UI.Text>();
        animator = GetComponent<Animator>();
        audioSfx = GetComponent<AudioSource>();
    }

    public void StartCounting(int start, int end)
    {

        if (!textUi)
            Start();

        animator.speed = 2f;
        StopCoroutine("Count");
        StartCoroutine(Count(start, end));
    }

    public IEnumerator Count(int start, int end)
    {

        // Debug.Log ("start " + start + " end" + end);
        // countingDelay = Mathf.Clamp (countingDuration / (end - start), 0.0001f, 0.1f);
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(countingDelay);

        if (audioSfx)
        {
            audioSfx.Play();
            audioSfx.loop = true;
        }

        float loopIncrement = ((float)(end - start) / 50f);
        if (loopIncrement < 1)
            loopIncrement = 1;

        // Debug.Log ("loopIncrement = " + loopIncrement);
        for (int i = start; i <= end; i += (int)loopIncrement)
        {
            //Debug.Log("C="+i);
            textUi.text = "" + i;
            if (animator)
                animator.Play("BLINK");
            yield return delay;
        }

        textUi.text = "" + end;

        if (audioSfx)
            audioSfx.Stop();

    }
}