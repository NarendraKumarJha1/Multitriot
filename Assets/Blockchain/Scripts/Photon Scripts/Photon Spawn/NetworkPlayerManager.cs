using Photon.Pun;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using RGSK;
using System;

[System.Serializable]
public class Racers
{
    public string playerName;
    public int actorNo;
    public bool isSetup;
    public bool isReady;
    public RGSK.Statistics statistics;
    public PhotonView photonView;
}

public class NetworkPlayerManager : MonoBehaviour
{
    public static NetworkPlayerManager instance;

    [SerializeField] GameObject rskStuffPrefab;
    [SerializeField] GameObject rskStuff;
    public List<Racers> racers = new List<Racers>();

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        PhotonManager.PlayerEnterExit += OnPlayerLeftGame;
    }
    private void OnDisable()
    {
        PhotonManager.PlayerEnterExit -= OnPlayerLeftGame;
    }

    public void InitPlayers()
    {
        racers.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Racers _r = new Racers()
            {
                playerName = p.NickName,
                actorNo = p.ActorNumber,
                isSetup = false,
                isReady = false,
            };
            racers.Add(_r);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            Debug.LogError("Level loaded");

            if (!rskStuff)
                rskStuff = Instantiate(rskStuffPrefab);
            rskStuff.SetActive(true);

            SendLevelLoadComplete();
        }
    }

    void SendLevelLoadComplete()
    {
        object[] _data = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
        PhotonManager.PhotonRaiseEventsSender_MasterClient(PhotonManager.LevelLoaded, _data);
    }

    public void LevelLoaded(int actNo)
    {
        Racers _r = racers.Find(x => x.actorNo == actNo);
        if (_r != null)
        {
            Debug.Log("Racer Name1: " + _r.playerName);
            _r.isSetup = true;

            bool canSetup = true;

            foreach (Racers r in racers)
            {
                if (!r.isSetup)
                {
                    Debug.Log("Player fault1: " + r.playerName);
                    canSetup = false;
                    break;
                }
            }

            if (canSetup && PhotonNetwork.IsMasterClient)
            {
                object[] _data = new object[] { };
                PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.SetupRace, _data);
            }
        }
    }
    public IEnumerator SetUp()
    {
        if (!rskStuff)
        {
            Debug.LogError("Gameobject is not assigned");
            yield break;
        }

        rskStuff.SetActive(true);

        yield return new WaitForSeconds(1f);

        Debug.Log("SetUp done");
        object[] _data = new object[] { true };
        PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.AllRacersReady, _data);
    }

    public IEnumerator SetPlayerReady()
    {
        yield return new WaitForSeconds(3f);

        RGSK.RankManager.instance.allRacersReady = true;
    }

    public void SetPlayerReadyStatus(int actNo, bool status)
    {
        Racers _r = racers.Find(x => x.actorNo == actNo);
        Debug.Log("Racer Name: " + _r.playerName);
        _r.isReady = status;

        List<PhotonView> pv = FindObjectsOfType<PhotonView>().ToList();
        PhotonView rPv = pv.Find(x => x.Controller.ActorNumber == actNo);

        _r.photonView = rPv;
        _r.statistics = rPv.GetComponent<RGSK.Statistics>();

        CheckAllPlayerReady();
    }

    void CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        bool canStart = true;

        foreach (Racers _r in racers)
        {
            if (!_r.isReady || !_r.isSetup)
            {
                Debug.Log("Player fault: " + _r.playerName);
                canStart = false;
                break;
            }
        }

        if (canStart && GameController.instance.gameState == GameState.None)
        {
            object[] _data = new object[] { };
            PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.SetRacerPreferences, _data);
        }
    }

    public void UpdatePlayerStats(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps[PhotonManager.pCar] != null)
        {
            PhotonView[] pvs = FindObjectsOfType<PhotonView>();

            Statistics playerStatistics = pvs.ToList().Find(x => x.Controller == targetPlayer).GetComponent<Statistics>();

            Statistics.RacerDetails rd = new Statistics.RacerDetails()
            {
                racerName = targetPlayer.NickName,
                vehicleName = changedProps[PhotonManager.pCar].ToString()
            };
            playerStatistics.rank = int.Parse(changedProps[PhotonManager.pSr].ToString());
            playerStatistics.RacerDetail = rd;
        }
    }

    public void NodeChanged(int cn, int actNo)
    {
        Statistics _s = racers.Find(x => x.photonView.Controller.ActorNumber == actNo).statistics;

        if (!_s) return;

        _s.CurrentNodeNumber = cn;
    }

    public void RacerFinished(int actNo)
    {
        Statistics _s = racers.Find(x => x.photonView.Controller.ActorNumber == actNo).statistics;

        if (!_s) return;

        _s.finishedRace = true;

        RaceUI.instance.ShowRaceResults();
    }

    void OnPlayerLeftGame(Player _p, bool state)
    {
        if (!state)
        {
            try
            {
                Racers _r = racers.Find(x => x.photonView.Controller.ActorNumber == _p.ActorNumber);

                if (_r != null)
                {
                    Statistics _s = RankManager.instance.racersStats.Find(x => x == _r.statistics);
                    RankManager.instance.racersStats.Remove(_s);

                    RankManager.instance.TotalRacers = RankManager.instance.racersStats.Count;
                }
            }
            catch { }
        }
/*        try
        {
            MainControllerManager._instance.ShowMessageCoroutine();
        }
        catch (Exception e)
        {
            Debug.Log("Custom room leave function exception cuz" + e);
        }*/
    }

}