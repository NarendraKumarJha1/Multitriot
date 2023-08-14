using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public class Tournament : MonoBehaviour
{

    public MainTournament tournamentData = new MainTournament();

    private void Start()
    {
       // InvokeRepeating("Download", 1, 15);    Download();

       // SetWinner("6423", "1484521298", "63280fbc2454822ecc8a6ae6");
    }

    #region Tournament Data
    public void Download()
    {
        StartCoroutine(GetTournamentData(Constants.BaseUrl + "/gettournament"));
    }
    IEnumerator GetTournamentData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            //if (ApiManager.instance.showDebugs)
            //    Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            tournamentData = JsonConvert.DeserializeObject<MainTournament> (www.downloadHandler.text);
        }
        else
        {
            //if (ApiManager.instance.showDebugs)
            //    Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
           // OnLastdataRecieved(false, null);
        }
    }

    #endregion

    #region JoinTournament

    public void JoinTrnmnt(string Tid,string RoomName,bool isInveUsed)
    {
        StartCoroutine(JoinTournament(Constants.BaseUrl + "/jointournament", Tid, RoomName,isInveUsed));
    }
    IEnumerator JoinTournament(string url, string TId, string RoomName,bool isInventryUsed)
    {
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", TId);
        form.AddField("roomId", RoomName);
        form.AddField("isUsingInventry", isInventryUsed.ToString());
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            // tournamentData = JsonConvert.DeserializeObject<List<TournamentClass>>(www.downloadHandler.text);
            Debug.LogError("Tournament joined with Tid " + TId + " Roomname " + RoomName + " is inventry used" +isInventryUsed);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            // OnLastdataRecieved(false, null);
        }
    }
    #endregion

    #region Winner
    public void SetWinner(string TournamentID,string RoomId,string UserId)
    {
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", TournamentID);
        form.AddField("roomId", RoomId);
        form.AddField("userId", UserId);
        Debug.Log("Winner set");
        StartCoroutine(setTournamentWinner(Constants.BaseUrl + "/addwinner", form));
    }

    IEnumerator setTournamentWinner(string url,WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
      //  www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("TWinner Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

           // tournamentData = JsonConvert.DeserializeObject<List<TournamentClass>>(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            // OnLastdataRecieved(false, null);
        }
    }
    #endregion
}



#region Tournament Data class

[Serializable]
public class TournamentPlayer
{
    public string userId;
    public string metamaskId;
    public string _id;
}

[Serializable]
public class TournamentRoom
{
    public Winner winner;
    public string roomId;
    public string createrId;
    public List<TournamentPlayer> players;
    public string _id;
}

[Serializable]
public class MainTournament
{
    public List<TournamentData> Tournament;
}

[Serializable]
public class Ticket
{
    public string userId;
    public int quantity;
    public string _id;
}


[Serializable]
public class TournamentData
{
    public string _id;
    public string tournamentmap;
    public string FuelAmount;
    public string nameoftournament;
    public string status;
    public string tournamentId;
    public string numberofplayers;
    public int TicketsAmount;
    public string starttiming;
    public string endingTime;
    public List<TournamentRoom> Rooms;
    public List<Ticket> tickets;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[Serializable]
public class Winner
{
    public string userId;
    public string metamaskId;
}





#endregion tournament data

#region Register Tournament

#endregion 

#region setWinner

#endregion
