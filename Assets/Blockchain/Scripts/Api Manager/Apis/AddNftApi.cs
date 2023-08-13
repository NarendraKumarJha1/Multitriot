using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AddNftApi : ApiBase
{
    public ApiStatus lastdataRecieved = new ApiStatus();
    public event Action<bool, ApiStatus> OnLastdataRecieved = delegate { };

    protected override void OnValidate()
    {
        if (DownloadNow)
        {
            DownloadNow = false;
            WWWForm formData = new WWWForm();
            foreach (FormFields ff in formFields)
                formData.AddField(ff.key, ff.value);

            StartCoroutine(AddTokenData(URL, formData));
        }
    }

    public void Download(string hash, string count)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("hash", hash);
        formData.AddField("address", BlockchainDataManager.instance.WalletData.walletAddress);
        formData.AddField("count", count);

        StartCoroutine(AddTokenData(URL, formData));
    }

    IEnumerator AddTokenData(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            OnLastdataRecieved(false, lastdataRecieved);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        lastdataRecieved = new ApiStatus()
        {
            status = node["status"]
        };

        OnLastdataRecieved(true, lastdataRecieved);
    }
}