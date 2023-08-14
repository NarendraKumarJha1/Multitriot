using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class AllErc1155Example : MonoBehaviour
{

    [System.Serializable]
    public class NFTs
    {
        public string contract;
        public string tokenId;
        public string uri;
        public string balance;
    }

    public List<NFTs> erc = new List<NFTs>();
    async void Start()
    {
        string chain = "Binance";
        string network = "Testnet"; // mainnet ropsten kovan rinkeby goerli
        string account = "0x33A4B0217DDFaBa14f56023635DC7F49A191D0d6";
        string contract = "0x49b0FC84EFa788023e35d0A6EaF83Fcb09072EB9";
        int first = 500;
        int skip = 0;
        string response = await EVM.AllErc1155(chain, network, account, contract, first, skip);
        try
        {
            erc = JsonConvert.DeserializeObject<List<NFTs>>(response);
            print(erc[0].contract);
            print(erc[0].tokenId);
            print(erc[0].uri);
            print(erc[0].balance);
        }
        catch
        {
            print("Error: " + response);
        }
    }
}
