using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sliders : MonoBehaviour
{
    FastMobileDOF fmDof;
    void Start()
    {
        fmDof = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FastMobileDOF>();
    }

    void Update()
    {

    }
    public void Focus(Slider a)
    {
        fmDof.Focus = a.value;
    }
}
