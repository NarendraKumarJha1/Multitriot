using Newtonsoft.Json;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ProfileEdit : ApiBase
{
    

    public void Download(string nickname, int avatarId)
    {
        

        WWWForm formData = new WWWForm();
        formData.AddField("nickname", nickname);
        formData.AddField("avtarId", avatarId.ToString());

        StartCoroutine(EditProfileNow(URL, formData));
    }

    IEnumerator EditProfileNow(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("profile update Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
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
