using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DailyClaimables : MonoBehaviour
{



    IEnumerator GetTournamentData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            //tournamentData = JsonConvert.DeserializeObject<MainTournament>(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            // OnLastdataRecieved(false, null);
        }
    }
}
