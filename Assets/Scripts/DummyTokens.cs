using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DummyTokens : MonoBehaviour
{

    public DummyTokenss tokens;
    private string baseURL = "http://45.79.126.10:8082";


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        GetTokens();
    //    }

    //    if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        AddTokenss("12");
    //    }

    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        RemoveTokenss("10");
    //    }
    //}
    public void GetTokens()
    {
        StartCoroutine(GetDummyTokens());
    }
    IEnumerator GetDummyTokens()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/getdummytoken");

        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));


            tokens = JsonConvert.DeserializeObject<DummyTokenss>(www.downloadHandler.text);


        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));

        }
    }

    public void AddTokenss(string count)
    {



        WWWForm formData = new WWWForm();

        formData.AddField("dummytokens", count);

        StartCoroutine(AddDummyTokens(formData, baseURL + "/adddummytokens"));

    }
    IEnumerator AddDummyTokens(WWWForm formData, string URL)
    {
        UnityWebRequest www = UnityWebRequest.Post(URL, formData);

        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));


            // tokens = JsonConvert.DeserializeObject<DummyTokenss>(www.downloadHandler.text);


        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));

        }
    }

    public void RemoveTokenss(string count)
    {
        WWWForm formData = new WWWForm();

        formData.AddField("dummytokens", count);

        StartCoroutine(RemoveDummyTokens(formData, baseURL + "/removedummytokens"));

    }
    IEnumerator RemoveDummyTokens(WWWForm formData, string URL)
    {
        UnityWebRequest www = UnityWebRequest.Post(URL, formData);

        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));


            // tokens = JsonConvert.DeserializeObject<DummyTokenss>(www.downloadHandler.text);


        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));

        }
    }
}


[Serializable]
public class DummyTokensss
{
    public string _id;
    public string userId;
    public string dummytokens;
    public DateTime createdAt;
    public DateTime updatedAt;
}

