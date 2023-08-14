using UnityEngine;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables instance;

    public static int minAddTokenValue = 100;
    public static int minRedeemValue = 1;

    [Header("Wager Mode")]
    public List<int> roomEntryAmt = new List<int>();

    private void Awake()
    {
        instance = this;
        //$$$ Debug.LogError(gameObject.name); //
        roomEntryAmt.Clear();
        roomEntryAmt.Add(10000);
        roomEntryAmt.Add(20000);
    }
}