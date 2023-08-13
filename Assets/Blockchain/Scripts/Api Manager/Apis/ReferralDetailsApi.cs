using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[Serializable]
public class ReferralDetails
{
    public int referralCount;
}

public class ReferralDetailsApi : ApiBase
{
    public ReferralDetails lastdataRecieved = new ReferralDetails();
    public event Action<bool, ReferralDetails> OnLastdataRecieved = delegate { };

    protected override void OnValidate()
    {
        if (DownloadNow)
        {
            DownloadNow = false;
            StartCoroutine(GetReferralData(URL));
        }
    }

    public void Download()
    {
        if (byPassApi)
        {
            Bypass();
            return;
        }

        StartCoroutine(GetReferralData(URL));
    }

    IEnumerator GetReferralData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            //if (ApiManager.instance.showDebugs)
            //    Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            OnLastdataRecieved(false, lastdataRecieved);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        lastdataRecieved = new ReferralDetails()
        {
            referralCount = node["add"]["referalcount"]
        };

        OnLastdataRecieved(true, lastdataRecieved);
    }

    void Bypass()
    {
        lastdataRecieved = new ReferralDetails()
        {
            referralCount = 0
        };

        OnLastdataRecieved(true, lastdataRecieved);
    }
}