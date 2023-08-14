using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetBillboards : MonoBehaviour
{
    public static SetBillboards instance;

    private void Awake()
    {
        instance = this;
    }

    public List<string> LavaMap = new List<string>();
    public string LavaMapV;
    public List<string> NeonMap = new List<string>();
    public string NeonMapV;
    public List<string> FuturisticMap = new List<string>();
    public string FuturisticMapV;
    public string BaseURL;
    public List<Billboards> futuristicMapBoards = new List<Billboards>();
    public List<Billboards> NeonMapBoards = new List<Billboards>();
    public List<Billboards> LavaMapBoards = new List<Billboards>();
    private void Start()
    {
        StartCoroutine(GetBillBoardsForLavaMap());
        StartCoroutine(GetBillBoardsForFuturistic());
        StartCoroutine(GetBillBoardsForNeonMap());
    }

    IEnumerator GetBillBoardsForFuturistic()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://45.79.126.10:8082/getlogo/FuturisticMap");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            //                                                                                                                    JsonParser(www.downloadHandler.text);
            // JSONNode node = JSON.Parse(www.downloadHandler.text);

            futuristicMapBoards = JsonConvert.DeserializeObject<List<Billboards>>(www.downloadHandler.text);
            //Debug.Log(node);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            //  OnLastdataRecieved(false, null);
        }

        if (futuristicMapBoards.Count > 0)
        {
            //SaperateForRoom(FuturisticMap, futuristicMapBoards, FuturisticMapV);
            SaperateFuturistic();
        }
    }

    IEnumerator GetBillBoardsForNeonMap()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://45.79.126.10:8082/getlogo/NeonMap");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            //                                                                                                                    JsonParser(www.downloadHandler.text);
            // JSONNode node = JSON.Parse(www.downloadHandler.text);

            NeonMapBoards = JsonConvert.DeserializeObject<List<Billboards>>(www.downloadHandler.text);
            //Debug.Log(node);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            //  OnLastdataRecieved(false, null);
        }

        if (NeonMapBoards.Count > 0)
        {
            //  SaperateForRoom(NeonMap, NeonMapBoards, NeonMapV);
            SaperateNeon();
        }
    }

    IEnumerator GetBillBoardsForLavaMap()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://45.79.126.10:8082/getlogo/Lavamap");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            //                                                                                                                    
            // JSONNode node = JSON.Parse(www.downloadHandler.text);

            LavaMapBoards = JsonConvert.DeserializeObject<List<Billboards>>(www.downloadHandler.text);
            //Debug.Log(node);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            //  OnLastdataRecieved(false, null);

          
        }

        if (LavaMapBoards.Count > 0)
        {
            //  SaperateForRoom(LavaMap, LavaMapBoards, LavaMapV);
            SaperateLava();
        }
    }
    //void JsonParser(string json)
    //{
    //    JSONNode node = JSON.Parse(json);


    //}

    void SaperateFuturistic()
    {



        foreach (var b in futuristicMapBoards)
        {
            if (b.imagetype.Contains('h'))
            {
                FuturisticMap.Add(b.image);
            }
            else if (b.imagetype.Contains('v'))
            {

                FuturisticMapV = b.image;
            }


        }
    }

    void SaperateLava()
    {
        
        foreach (var b in LavaMapBoards)
        {
            if (b.imagetype.Contains('h'))
            {
               
                LavaMap.Add(b.image);
            }
            else if (b.imagetype.Contains('v'))
            {

                LavaMapV = b.image;
            }


        }
    }

    void SaperateNeon()
    {



        foreach (var b in NeonMapBoards)
        {
            if (b.imagetype.Contains('h'))
            {
                NeonMap.Add(b.image);
            }
            else if (b.imagetype.Contains('v'))
            {

                NeonMapV = b.image;
            }


        }
    }
}

[Serializable]
public class Billboards
{
    public string _id;
    public string maptype;
    public string image;
    public string imagetype;
    public DateTime createdAt;
    public DateTime updatedAt;
}


