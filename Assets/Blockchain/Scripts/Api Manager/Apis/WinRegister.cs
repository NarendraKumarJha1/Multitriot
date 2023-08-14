using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WinRegister : ApiBase
{
    [SerializeField]
    public class Winner
    {
        public string roomId;
    }

    public void Download(string Roomid)
    {
        Debug.Log("winner registered with room id=" + Roomid + " Player id " + UserDatabase.Instance.localUserData.id);
        

        WWWForm formData = new WWWForm();
        formData.AddField("roomId", Roomid);
        formData.AddField("id", UserDatabase.Instance.localUserData.id);

        StartCoroutine(WinnerRegisterData(URL, formData));
        RewardDistribution(PlayerPrefs.GetString("WinnerIndex"));
    }

    private void RewardDistribution(string _winnerIndex)
    {
        //Send Winner data
        string _postUrl = "http://3.236.113.97/escrow/distribueEscrow/";

        //Retreiving id
        Debug.Log("Checking Escrow Data " + PlayerPrefs.GetString("EscrowId"));
        string _dataRec = PlayerPrefs.GetString("EscrowId").Replace(" ", "");
        int _id = 0;
        try
        {
            _id = int.Parse(_dataRec);
            Debug.Log("Converted EscrowData " + _id);
        }
        catch (Exception)
        {
            Debug.Log("Failed to convert data rec");
        }
        Debug.Log("Data" + _dataRec);


        // Create JSON object
        var json = new
        {
            escrowid = _id,
            winnerindex = _winnerIndex,
            password = "5b7d0c83146141dac52cd845554130b4db4291a0ca9ef846fb73bc79072736eb@test.com",
            keystore = "{\"encSeed\":{\"encStr\":\"oTeqhGKAbGUmwS91WTk1G049n78oANk6ohmDGOkHEhkECRB6o3EDwftYgv2zkot3QkgNVltvnJ7VbDD5p9+eUAmHOH5jiy4tIruicDbBSwy4TKh0C6zZRWl92SFaLzZYE7Um4QzXoQTO8PABuLetZDQqywDwC2jMsUojddcYgxQ54M8vxSQ1mA==\",\"nonce\":\"5RWt9n3Hlg9cjHh4PoiY4MRciOEpj4tQ\"},\"encHdRootPriv\":{\"encStr\":\"aH2F+Z0ADpa0OYK9+GhHblR9ipzGntbD03+fni/c122XfzYbaAzTsNQIGBKkbFQ4PnbqfYGq5+iVO58vKqKneEdQt129qgrWq9jkWmqHsDtz/eeFshG1SmhKRze4RUYwuvZ5+WDpuhEl7SJsytEJR2OgOb5zo3I8idPTCWl1Uw==\",\"nonce\":\"y7fb/OLEBnl3SwPJ+HY4ysBncsb076sq\"},\"addresses\":[\"ce3db11e9f521f1b3da0b5b6c07ee8f7a5a0f94d\"],\"encPrivKeys\":{\"ce3db11e9f521f1b3da0b5b6c07ee8f7a5a0f94d\":{\"key\":\"JzlVp7P381qKDrsBuzXSVsbCz4/vzgMGJqGlWPRwwgCgIDqlyMIto1O6g7YS3dyL\",\"nonce\":\"c88NINY/p3Iv9R/3joa0hXEDJmX4XLA+\"}},\"hdPathString\":\"m/44'/60'/0'/0\",\"salt\":\"t0aNdyv2UfBrsZ4PVc7EtGHcnkM7ztKehFWd3j1SrUs=\",\"hdIndex\":1,\"version\":3}"
        };

        // Convert JSON object to string
        string postData = JsonConvert.SerializeObject(json);
        StartCoroutine(PostRequest(_postUrl, postData));
    }
    IEnumerator PostRequest(string url, string json)
    {
        Debug.Log(" JSON " + json);
        Debug.LogWarning(" JSON " + json);
        Debug.LogError(" JSON " + json);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Request failed: {www.error}");
            yield break;
        }

        string responseText = www.downloadHandler.text;
        Debug.LogError($"Response: {responseText}");
        Debug.Log($"Response: {responseText}");
        Debug.LogWarning($"Response: {responseText}");
    }
    IEnumerator WinnerRegisterData(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Winner Register Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            // OnLastdataRecieved(false, null);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        //  lastdataRecieved = GetApiStatus(json);

        //   OnLastdataRecieved(true, lastdataRecieved);
    }
}
