using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RedeemTokenApi : ApiBase
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

    public void Download(double count)
    {
        WWWForm formData = new WWWForm();
        Debug.LogError("redeem amount is " + count);
        formData.AddField("count", string.Format("{0:0}", count));
        formData.AddField("address", BlockchainDataManager.instance.WalletData.walletAddress);

        StartCoroutine(AddTokenData(URL, formData));
    }

    IEnumerator AddTokenData(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

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
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            OnLastdataRecieved(false, lastdataRecieved);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        ApiStatus status = GetApiStatus(json);

        OnLastdataRecieved(true, lastdataRecieved);
    }
}