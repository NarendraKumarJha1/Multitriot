using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCustomizer : MonoBehaviour {
    public Color defaultBodyColor;
    public Color defaultRimEmissionColor;
    public Color[] bodyPaintColors;
    public Color[] rimEmissionColors;
    // private int currentBodyColorIndex = -1;
    // public int CurrentBodyColorIndex { get { return currentBodyColorIndex; } set { currentBodyColorIndex = value; } }
    // private int currentRimEmissionColorIndex = -1;
    // public int CurrentRimEmissionColorIndex { get { return currentRimEmissionColorIndex; } set { currentRimEmissionColorIndex = value; } }

    private List<Material> bodyMats = new List<Material> ();
    private List<Material> rimEmissionMats = new List<Material> ();

    private void Awake () {
        CacheNecessaryMaterials ();
    }

    private void OnEnable () {
        if (SaveData.Instance != null)
        {
            ApplyBodyColor (SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.bodyColorIndex);
            ApplyRimEmissionColor (SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.rimEmissionIndex);
        }
    }

    void CacheNecessaryMaterials () {
        // body mat
        foreach (Renderer _renderer in GetComponentsInChildren<Renderer> ()) {
            foreach (Material mat in _renderer.materials) {
                if (mat.name.ToLower ().Contains ("body")) {
                    bodyMats.Add (mat);
                }
                if (mat.name.ToLower ().Contains ("rim_emission")) {
                    rimEmissionMats.Add (mat);
                }
            }
        }
    }

    public void ApplyBodyColor (int index) {
        foreach (Material mat in bodyMats) {
            if (index >= 0 && index < bodyPaintColors.Length) {
                mat.color = bodyPaintColors[index];
            } else {
                mat.color = defaultBodyColor;
            }
        }

    }

    public void ApplyRimEmissionColor (int index) {
        foreach (Material mat in rimEmissionMats) {
            if (index >= 0 && index < rimEmissionColors.Length) {
                mat.color = rimEmissionColors[index];
                mat.SetColor ("_EmissionColor", rimEmissionColors[index]);
            } else {
                mat.color = defaultRimEmissionColor;
                mat.SetColor ("_EmissionColor", defaultRimEmissionColor);
            }
        }
    }
}