using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
public class FogLayer : MonoBehaviour {
    public bool revertFogState = false;

    private void Start () {
        RenderSettings.fog = revertFogState;
    }

    // void OnPreRender () {
    //     revertFogState = RenderSettings.fog;
    //     RenderSettings.fog = enabled;
    // }

    // void OnPostRender () {
    //     RenderSettings.fog = revertFogState;
    // }
}