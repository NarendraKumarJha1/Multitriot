using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConversionHandler : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        public string name;
        public MeshRenderer carMesh;
        public MeshRenderer robotMesh;
        public GameObject carToRobotObject;
        public GameObject robotToCarObject;
        public float waitTime;
    }
    public Data[] allData = null;

}
