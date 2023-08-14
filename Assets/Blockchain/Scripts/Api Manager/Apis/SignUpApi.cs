using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SignUpApi : ApiBase
{
    [SerializeField] ApiStatus lastdataRecieved = new ApiStatus();
    public event Action<bool, ApiStatus> OnLastdataRecieved = delegate { };

    protected override void OnValidate()
    {
        if (DownloadNow)
        {
            DownloadNow = false;
            WWWForm formData = new WWWForm();
            foreach (FormFields ff in formFields)
                formData.AddField(ff.key, ff.value);

            StartCoroutine(RegisterData(URL, formData));
        }
    }

    public void Download(string username, string nickname, string email, string password, string referalcode)
    {
        WWWForm formData = new WWWForm();
       // formData.AddField("username", username);
        formData.AddField("nickname", nickname);
        formData.AddField("email", email);
        formData.AddField("password", password);
        if (string.IsNullOrEmpty(referalcode))
            formData.AddField("referalcode", "000000");
        else
            formData.AddField("referalcode", referalcode);

        StartCoroutine(RegisterData(URL, formData));
    }

    IEnumerator RegisterData(string url, WWWForm form)
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
            OnLastdataRecieved(false, null);
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        lastdataRecieved = GetApiStatus(json);

        OnLastdataRecieved(true, lastdataRecieved);
    }
}