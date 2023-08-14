using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterials : MonoBehaviour
{
    public MaterialType materialType;
    public Material Mat;
    public Axis moveAxis;
    public float Speed;
    public enum Axis
    {
        X, Y
    }

    public enum MaterialType
    {
        Standard,LWRP
    }

    void Update()
    {
        switch (materialType)
        {
            case MaterialType.Standard:
                switch (moveAxis)
                {
                    case Axis.X:
                        Mat.SetTextureOffset("_MainTex", new Vector2(Time.time * Speed, 0));
                        break;
                    case Axis.Y:
                        Mat.SetTextureOffset("_MainTex", new Vector2(0, Time.time * Speed));
                        break;
                }
                break;
            case MaterialType.LWRP:
                switch (moveAxis)
                {
                    case Axis.X:
                        Mat.SetTextureOffset("_BaseMap", new Vector2(Time.time * Speed, 0));
                        break;
                    case Axis.Y:
                        Mat.SetTextureOffset("_BaseMap", new Vector2(0, Time.time * Speed));
                        break;
                }
                break;
        }
    }
}
