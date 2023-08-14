using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    Text FPS;

    internal float frequency = 0.5f;
    internal int framesPerSec;

    private void Start()
    {
        FPS = GetComponent<Text>();
        StartCoroutine(updateFPS());
    }

    private IEnumerator updateFPS()
    {
        while (true)
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it
            this.framesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            this.FPS.text = this.framesPerSec.ToString();
        }
    }
}