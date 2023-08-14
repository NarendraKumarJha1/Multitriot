using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Photon.Pun;

#region Login Api Data
[Serializable]
public class LoginApiData
{
    public ApiStatus status;
    public LoginData loginData;

}
[Serializable]
public class LoginData
{
    public string id;
    public string email;
    public string username;
    public string nickname;
    public string metamaskId;
    public int avtarId;
    public UserWalletData userWalletData = new UserWalletData();
    public DummyTokenss tokens = new DummyTokenss();
    public DailyClaimable Dc = new DailyClaimable();
    public string referralCode;
    public int dummytokens;
    [Header("Login Token")]
    public string token;
    public string dummytoken;
}
[Serializable]
public class UserWalletData
{
    public string ingameToken;
    public List<WalletNFT> walletNFT = new List<WalletNFT>();
}
[Serializable]
public class WalletNFT
{
    public string contractAssress;
    public string tokenId;
    public string count;
}

[Serializable]
public class DailyClaimable
{
    public string Days;
    public int fuel;
    public int tickets;
    public int speedboost;
    public int missile;
}






[Serializable]
public class DummyTokenss
{
    public string _id;
    public string userId;
    public string dummytokens;
    public DateTime createdAt;
    public DateTime updatedAt;
}
#endregion

public class LoginApi : ApiBase
{
    [SerializeField] LoginApiData lastdataRecieved = new LoginApiData();
    public event Action<bool, LoginApiData> OnLastdataRecieved = delegate { };

    protected override void OnValidate()
    {
        if (DownloadNow)
        {
            DownloadNow = false;
            WWWForm formData = new WWWForm();
            foreach (FormFields ff in formFields)
                formData.AddField(ff.key, ff.value);

            StartCoroutine(GetLoginData(URL, formData));
        }
    }

    public void Download(string nickname, string password, bool guestUser)
    {

        WWWForm formData = new WWWForm();
        formData.AddField("nickname", nickname);
        formData.AddField("password", password);

        if (guestUser)
        {
            Bypass();
            return;
        }
        
        StartCoroutine(GetLoginData(URL, formData));
    }

    IEnumerator GetLoginData(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            if (www.downloadHandler.text.Length < 100)
            {

                OnLastdataRecieved(false, null);
            }
            else
            {
                JsonParser(www.downloadHandler.text);
            }
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            OnLastdataRecieved(false, null);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        ApiStatus status = GetApiStatus(json);

        if (status.status > 400)
        {
            lastdataRecieved = new LoginApiData()
            {
                status = status,
                loginData = null
            };
        }
        else
        {
            List<WalletNFT> _walletNFTs = new List<WalletNFT>();
            JSONArray nftArr = (JSONArray)node["data"]["wallet"]["nft"];

            if (node["status"] == "400")
            {

            }

            for (int i = 0; i < nftArr.Count; i++)
            {
                WalletNFT _nft = new WalletNFT()
                {
                    contractAssress = nftArr[i]["contractAssress"],
                    tokenId = nftArr[i]["tokenId"],
                    count = nftArr[i]["count"]
                };
                _walletNFTs.Add(_nft);
            }

            UserWalletData walletData = new UserWalletData()
            {
                ingameToken = node["data"]["wallet"]["ingametoken"],
                walletNFT = _walletNFTs
            };

            DummyTokenss dk = new DummyTokenss()
            {
               // _id = node["dummy"]["_id"],
               // userId = node["dummy"]["userId"],
                dummytokens = node["result"]["dummytoken"]
               // createdAt = DateTime.Now,
               // updatedAt = DateTime.Now

            };

            string claimText = "";

            try
            {
                claimText = node["claim"]["Days"];
            }
            catch
            {
                claimText = string.Empty;
            }




            if (!string.IsNullOrEmpty(claimText))
            {
                DailyClaimable dc = new DailyClaimable()
                {
                    fuel = node["claim"]["fuel"],
                    Days = node["claim"]["Days"],
                    tickets = node["claim"]["tickets"],
                    speedboost = node["claim"]["speedboost"],
                    missile = node["claim"]["missile"]
                };

                LoginData loginData = new LoginData()
                {
                    id = node["result"]["_id"],
                    email = node["result"]["email"],
                    // username = node["result"]["username"],
                    nickname = node["result"]["nickname"],
                    avtarId = node["result"]["avtarId"],
                    userWalletData = walletData,
                    tokens = dk,
                    dummytokens = node["dummytoken"],
                    referralCode = node["code"],
                    token = node["token"],
                    
                    Dc = dc


                };

                lastdataRecieved = new LoginApiData()
                {
                    status = status,
                    loginData = loginData

                };
            }
            else
            {
                LoginData loginData = new LoginData()
                {
                    id = node["result"]["_id"],
                    email = node["result"]["email"],
                    // username = node["result"]["username"],
                    nickname = node["result"]["nickname"],
                    avtarId = node["result"]["avtarId"],
                    metamaskId = node["result"]["metamaskId"],
                    userWalletData = walletData,
                    tokens = dk,
                    dummytokens = node["dummytoken"],
                    referralCode = node["code"],
                    token = node["token"],

                };

                lastdataRecieved = new LoginApiData()
                {
                    status = status,
                    loginData = loginData

                };
            }

            
        }

        OnLastdataRecieved(true, lastdataRecieved);

        ApiManager.instance.dumTokens.GetTokens();
/*        ApiManager.instance.inventory.download();
*/        ApiManager.instance.dumTokens.GetTokens();
    }

    void Bypass()
    {
        string _username = PlayerPrefs.GetString("username");
        ApiStatus apiStatus = new ApiStatus()
        {
            status = 102,
            msg = GenericStringKeys.LoginSuccess
        };

        UserWalletData walletData = new UserWalletData()
        {
            ingameToken = "100000"
        };

        LoginData loginData = new LoginData()
        {
            id = "1",
            email = "narendera.abhiwan@gmail.com",
            username = _username,
            nickname = _username,

            userWalletData = walletData,

            referralCode = "123456",
            token = "xyz"
        };

        lastdataRecieved = new LoginApiData()
        {
            status = apiStatus,
            loginData = loginData

        };

        OnLastdataRecieved(true, lastdataRecieved);
    }


    public IEnumerator CheckInGameToken(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {

            JSONNode node = JSONNode.Parse(www.downloadHandler.text);

            UserDatabase.Instance.localUserData.userWalletData.ingameToken = node["ingametoken"];
            //string _value = string.Format("{0:#,###0} Raze", UserDatabase.Instance.localUserData.userWalletData.ingameToken);
            //InGameCoinsText.text = _value;



        }

    }
}


