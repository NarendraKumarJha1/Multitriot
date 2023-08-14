using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Onhover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public bool _enable = true;
    // The original scale of the button's RectTransform component
    private Vector3 originalScale;

    // The amount to scale the button when the mouse cursor is over it
    public float hoverScale = 1.01f;

    // The duration of the scaling animation
    public float animationDuration = 0.1f;

    // A flag indicating whether the button is currently being scaled up
    private bool scalingUp = false;

    // The LeanTween ID for the scaling animation
    private int tweenId = -1;

    // Initialize the original scale of the button
    public GameObject Locked;
    private void Awake()
    {
        if (_enable)
        {

            originalScale = transform.localScale;
        }
    }

    // Handle the OnPointerEnter event
    public void OnPointerEnter(PointerEventData eventData)
    {
        // If the button is already being scaled up, don't start a new animation
        if (scalingUp)
        {
            return;
        }

        // Start the scaling animation
        if (_enable)
        {
            tweenId = LeanTween.scale(gameObject, new Vector3(hoverScale, hoverScale, hoverScale), animationDuration)
                .setEase(LeanTweenType.easeOutSine)
                .setOnComplete(() => scalingUp = true)
                .id;
            FindObjectOfType<MMSceneManager>().PlayTempClip("Button");

        }
    }

    // Handle the OnPointerExit event
    public void OnPointerExit(PointerEventData eventData)
    {
        // If the button is not being scaled up, don't start a new animation
        if (!scalingUp)
        {
            return;
        }

        // Start the scaling animation
        if (_enable)
        {
            tweenId = LeanTween.scale(gameObject, originalScale, animationDuration)
                .setEase(LeanTweenType.easeOutSine)
                .setOnComplete(() => scalingUp = false)
                .id;

        }
    }
}
