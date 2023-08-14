using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class GameRegister : ApiBase
{


    public void Download(string entryFee, string RoomId, string matchType)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("entryfees", entryFee);
        formData.AddField("roomId", RoomId);
        formData.AddField("matchtype", matchType);
        formData.AddField("status", "0");
        //Debug.LogError("game registered with room id=" + RoomId
        //    + " :entryfee= " + entryFee
        //    + " :matchtype= " + matchType);
        StartCoroutine(GameRegisterData(URL, formData));
    }

    IEnumerator GameRegisterData(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("GameRegister Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("GameRegister Response Code : {0}\nError:{1}", www.responseCode, www.error));
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
