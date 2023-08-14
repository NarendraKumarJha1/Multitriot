using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class ERC721BalanceOfExample : MonoBehaviour
{
    public string chain = "ethereum";
    public string network = "mainnet";
    public string contract = "0x60f80121c31a0d46b5279700f9df786054aa5ee5";
    public string account = "0x6b2be2106a7e883f282e2ea8e203f516ec5b77f7";
    async void Start()
    {


        int balance = await ERC721.BalanceOf(chain, network, contract, account);
        print(balance);
    }
}
