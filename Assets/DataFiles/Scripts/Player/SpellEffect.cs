using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class SpellEffect : MonoBehaviour
{
    #region Freeze spell variables
    [Header("Car Mesh")]
    [SerializeField]
    public MeshRenderer[] childMeshRenderers;


    [Header("Car Freeze Material")]
    [SerializeField]
    public Material _freezeMaterial;
    [SerializeField] Texture _freeze;

    [Header("Car Original Material")]
    [SerializeField]
    private Material[] _originalMaterial;
    #endregion

    #region Trans spell variables
    [Header("Translucent Material")]
    [SerializeField]
    public Material _transMaterial;
    #endregion

    #region Inverse spell variables
    [Header("Inverse Material")]
    [SerializeField]
    public Material _inverseMaterial;
    [SerializeField] Texture _inverse;
    #endregion


    CarShoot _car = null;
    [SerializeField]
    public GameObject _target;
    private List<Material[]> _originalMaterials = new List<Material[]>();

    private void Start()
    {
        childMeshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Save the original materials
        foreach (MeshRenderer mesh in childMeshRenderers)
        {
            Material[] originalMaterials = mesh.materials;
            _originalMaterials.Add(originalMaterials);
        }
    }


    public void SpellFunc(SpellType _spellType)
    {
        Debug.Log("Spell Function called with spell type In Spell Func " + _spellType);

        switch (_spellType)
        {
            case SpellType.Freeze:
                Debug.Log("Freeze called");
                AssignFreezeTexture();
                StartCoroutine(Freeze(3f, -3f, 1f)); // Interpolate "_CHANGER" value from -3 to 3 over 5 seconds
                StartCoroutine(RetreiveColor(-3f, 3f, 1f, 3f));
                break;
            case SpellType.Inverse:
                Debug.Log("Inverse called");
                AssignInverseTexture();
                StartCoroutine(Freeze(3f, -3f, 1f));
                StartCoroutine(RetreiveColor(-3f, 3f, 1f, 3f));
                break;
            case SpellType.Invisible:
                Debug.Log("Invisible called");

                foreach (MeshRenderer mesh in childMeshRenderers)
                {
                    int l = mesh.materials.Length;
                    Material[] newMaterials = new Material[l];
                    for (int j = 0; j < l; j++)
                    {
                        newMaterials[j] = _transMaterial;
                        Debug.LogWarning("Making invisible " + mesh + " Current material " + mesh.materials[j]);
                    }
                    mesh.materials = newMaterials;

                }
                StartCoroutine(InvisibleEffect());
                StartCoroutine(RetreiveOriginalMaterials(8f));
                break;
        }
    }

    IEnumerator RetreiveOriginalMaterials(float _dur)
    {
        yield return new WaitForSeconds(_dur);
        // Retrieve and assign original materials back to the MeshRenderer
        // Retrieve and assign original materials back to the MeshRenderer
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            MeshRenderer mesh = childMeshRenderers[i];
            Material[] originalMaterials = _originalMaterials[i];
            mesh.materials = originalMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //On hit Try to get carshoot script to analyze spell type
        try
        {
            _car = other.gameObject.GetComponent<CarShoot>();
        }catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        if (_car != null)
        {
            if(_car._spellType == SpellType.Invisible){
                SpellFunc(_car._spellType);
            }
        }

    }

    IEnumerator InvisibleEffect()
    {
        Physics.IgnoreLayerCollision(8, 8, true);
        yield return new WaitForSeconds(8f);
        Physics.IgnoreLayerCollision(8, 8, false);
    }


    #region Freeze
    public void AssignFreezeTexture()
    {
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            int l = childMeshRenderers[i].materials.Length;
            for (int j = 0; j < l; j++)
            {
                try
                {
                    childMeshRenderers[i].material.SetTexture("_EMISSION_MAP_2", _freeze);
                }catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
    IEnumerator Freeze(float startValue, float endValue, float duration)
    {
        // Get all child MeshRenderers

        // Set the initial value of the "_CHANGER" property for all materials
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            Material[] materials = childMeshRenderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetFloat("_CHANGER", startValue);
            }
        }

        // Calculate the amount to change the value each frame based on the duration
        float valueChangePerFrame = (endValue - startValue) / duration;

        // Gradually change the value of the "_CHANGER" property over time
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Calculate the new value of the "_CHANGER" property
            float newValue = startValue + (t * valueChangePerFrame);

            // Set the new value of the "_CHANGER" property for all materials
            for (int i = 0; i < childMeshRenderers.Length; i++)
            {
                Material[] materials = childMeshRenderers[i].materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SetFloat("_CHANGER", newValue);
                }
            }

            yield return null;
        }

        // Set the final value of the "_CHANGER" property to ensure it reaches the correct end value for all materials
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            Material[] materials = childMeshRenderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetFloat("_CHANGER", endValue);
            }
        }
    }
    IEnumerator RetreiveColor(float startValue, float endValue, float duration, float waitdur)
    {
        // Get all child MeshRenderers

        // Set the initial value of the "_CHANGER" property for all materials
        yield return new WaitForSeconds(waitdur);
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            Material[] materials = childMeshRenderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetFloat("_CHANGER", startValue);
            }
        }

        // Calculate the amount to change the value each frame based on the duration
        float valueChangePerFrame = (endValue - startValue) / duration;

        // Gradually change the value of the "_CHANGER" property over time
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Calculate the new value of the "_CHANGER" property
            float newValue = startValue + (t * valueChangePerFrame);

            // Set the new value of the "_CHANGER" property for all materials
            for (int i = 0; i < childMeshRenderers.Length; i++)
            {
                Material[] materials = childMeshRenderers[i].materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SetFloat("_CHANGER", newValue);
                }
            }

            yield return null;
        }

        // Set the final value of the "_CHANGER" property to ensure it reaches the correct end value for all materials
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            Material[] materials = childMeshRenderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetFloat("_CHANGER", endValue);
            }
        }
    }
    #endregion


    #region



    public void AssignInverseTexture()
    {
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            int l = childMeshRenderers[i].materials.Length;
            for (int j = 0; j < l; j++)
            {
                try
                {
                    childMeshRenderers[i].material.SetTexture("_EMISSION_MAP_2", _inverse);
                }catch (Exception e) { 
                    Debug.LogError(e.Message);
                }
            }
        }
    }

    #endregion
}
