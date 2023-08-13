using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public enum CallbackStatus { None, Success, Failed };

public class PhotonManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static PhotonManager instance;
    public static bool IsGuest;
    public static bool FreeToPlay;
    public static bool DummyTokens;
    public static bool HostQuit;
    public static int Rank;
    public static string TournamentRoomId;
    public bool IsInventryItemUsed;
    #region Events
    public static event Action<CallbackStatus> PhotonServerJoiningStatus = delegate { };
    public static event Action<CallbackStatus> LobbyJoiningStatus = delegate { };
    public static event Action<CallbackStatus> RoomJoiningStatus = delegate { };
    public static event Action<Player, bool> PlayerEnterExit = delegate { };
    #endregion


    internal TypedLobby Wager = new TypedLobby("Wager", LobbyType.Default);
    internal TypedLobby Tournament = new TypedLobby("Tournament", LobbyType.Default);

    #region Custom Properties
    public const string rName = "Room Name";
    public const string rAmt = "Room EntryFee";
    public const string rMap = "Room Map";
    public const string rTyp = "Room GameTypre";

    public const string pAvt = "PlayerAvatar";
    public const string pCar = "Player Car";
    public const string pSr = "Player Start Rank";

    public static string RoomAmt;
    public static string RoomName;
    public static int RoomAmtT;
    public List<int> Cars = new List<int>();
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void Init()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonServerJoiningStatus(CallbackStatus.Success);
            return;
        }

        Debug.Log("Connecting to photon");
        PhotonNetwork.ConnectUsingSettings();

        PhotonServerJoiningStatus(CallbackStatus.None);
    }
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("OnConnected success");
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster on " + PhotonNetwork.CloudRegion);

        Debug.Log("Nickname: " + UserDatabase.Instance.localUserData.nickname);
        PhotonNetwork.LocalPlayer.NickName = UserDatabase.Instance.localUserData.nickname;

        Hashtable data = new Hashtable()
        {
            {"avt",UserDatabase.Instance.localUserData.avtarId.ToString() }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(data);

        PhotonServerJoiningStatus(CallbackStatus.Success);

        Rank = 0;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected cause: " + cause);
    }

    #region  Custom Methods
    public void JoinLobby(GameMode mode)
    {
        Debug.Log("Joining Lobby");
        LobbyJoiningStatus(CallbackStatus.None);

        TypedLobby _lobby = mode == GameMode.Wager ? Wager : Tournament;
        PhotonNetwork.JoinLobby(_lobby);
    }

    int roomEntryFee = 0;
    public void JoinRandomRoom(int amt)
    {
        Debug.Log("Joining Random room of " + amt);
        roomEntryFee = amt;
        int modeID = 0;
        if (FreeToPlay)
            modeID = 10;
        else
            modeID = Constants.modeId;
        string game = "";


        if (BlockchainDataManager.currentGameMode != GameMode.Tournament)
        {

            Hashtable data = new Hashtable
             {
                  { rAmt, roomEntryFee.ToString() + Constants.selectedLevel.ToString() +game },
                //{ rMap, Constants.selectedLevel },
                //{ rTyp, modeID }
             };

            PhotonNetwork.JoinRandomRoom(data, 0);
        }
        else
        {
            Hashtable data = new Hashtable
             {
                  { rAmt, TournamentRoomId}
                
             };

            PhotonNetwork.JoinRandomRoom(data, 0);
        }



       
    }

    public void JoinOrCreateRoom(string RoomName)
    {


        RoomOptions opt = new RoomOptions();
        opt.IsVisible = true;
        opt.MaxPlayers = 2;

        PhotonNetwork.JoinOrCreateRoom(RoomName, opt, TypedLobby.Default);
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

       // Debug.Log("Room count> " + roomList.Count);
        foreach (var r in roomList)
        {
            string msg = "";
            foreach (var rP in r.CustomProperties)
                msg += string.Format("\tK: {0}, V: {1}\n", rP.Key, rP.Value);
            Debug.Log(msg);
        }

        base.OnRoomListUpdate(roomList);
    }

    public void CreateRoom(int entryFee)
    {
        string randomRoomName = Mathf.Abs(PhotonNetwork.ServerTimestamp).ToString(); //UnityEngine.Random.Range(1001, 9999).ToString();

        RoomOptions roomOpt = new RoomOptions();
        roomOpt.MaxPlayers = 2;
        roomOpt.EmptyRoomTtl = 0;
        roomOpt.CleanupCacheOnLeave = true;

        int modeID = 0;
        if (FreeToPlay)
            modeID = 10;
        else
            modeID = Constants.modeId;
        string game = "";
        //if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
        //    game = modeID.ToString() + 99.ToString();

        if (BlockchainDataManager.currentGameMode != GameMode.Tournament)
        {
            Hashtable data = new Hashtable{
            { rName, randomRoomName },
            { rAmt, entryFee.ToString() + Constants.selectedLevel.ToString() + game},
            //  { rMap, Constants.selectedLevel },
            //{ rTyp, modeID }
            };

            roomOpt.CustomRoomPropertiesForLobby = new string[] { rAmt, rName };
            roomOpt.CustomRoomProperties = data;

            //RoomAmt = entryFee.ToString();
            //RoomName = randomRoomName;

            roomOpt.IsVisible = true;
            roomOpt.IsOpen = true;

            PhotonNetwork.CreateRoom(randomRoomName, roomOpt);
        }
        else
        {
            ///
            Hashtable data = new Hashtable{
            { rName, randomRoomName },
            { rAmt, TournamentRoomId }
            };

            roomOpt.CustomRoomPropertiesForLobby = new string[] { rAmt, rName };
            roomOpt.CustomRoomProperties = data;

            //RoomAmt = entryFee.ToString();
            //RoomName = randomRoomName;

            roomOpt.IsVisible = true;
            roomOpt.IsOpen = true;

            PhotonNetwork.CreateRoom(randomRoomName, roomOpt);
        }
       
    }
    #endregion

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined: " + PhotonNetwork.CurrentLobby);
        LobbyJoiningStatus(CallbackStatus.Success);

        base.OnJoinedLobby();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Lobby Left");
        base.OnLeftLobby();
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined: " + PhotonNetwork.CurrentRoom.Name);



        string msg = "";
        foreach (var rP in PhotonNetwork.CurrentRoom.CustomProperties)
            msg += string.Format("\tK: {0}, V: {1}\n", rP.Key, rP.Value);
        Debug.Log(msg);

        RoomJoiningStatus(CallbackStatus.Success);

        //Hashtable data = new Hashtable();
       // data.Add("avt", UserDatabase.Instance.localUserData.avtarId);
        //data["avt"] = UserDatabase.Instance.localUserData.avtarId;

        //PhotonNetwork.LocalPlayer.SetCustomProperties(data);

        PlayerEnterExit(PhotonNetwork.LocalPlayer, true);



        base.OnJoinedRoom();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Joined Failed: " + message);
        RoomJoiningStatus(CallbackStatus.Failed);

        if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
        {

        }

        base.OnJoinRoomFailed(returnCode, message);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Random Room Joined Failed: " + message);
        CreateRoom(roomEntryFee);

        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");

        base.OnCreatedRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("failed to create room: " + message);
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnLeftRoom()
    {
        Debug.LogError("You left room 1");

        PlayerEnterExit(PhotonNetwork.LocalPlayer, false);

        base.OnLeftRoom();

        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.NickName);

        PlayerEnterExit(newPlayer, true);

        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left: " + otherPlayer.NickName);
         
        PlayerEnterExit(otherPlayer, false);
/*        try
        {
*//*            MainControllerManager._instance.ShowMessageCoroutine();
*//*        }
        catch(Exception e)
        {
            Debug.Log("Custom room leave function exception cuz"+ e);
        }*/
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Player Property updated");

        foreach (var pP in changedProps)
            Debug.Log("K: " + pP.Key +", V: " + pP.Value);

        NetworkPlayerManager.instance.UpdatePlayerStats(targetPlayer, changedProps);

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }


    #region Raise Events

    //byte codes
    public const byte MasterStartGame = 1;
    public const byte PlayerReady = 2;
    public const byte LevelLoaded = 3;
    public const byte SetupRace = 4;
    public const byte AllRacersReady = 5;
    public const byte SetRacerPreferences = 6;
    public const byte RaceCountdownTimer = 7;
    public const byte StartRace = 8;

    public const byte PowerPick = 11;
    public const byte PowerUsed = 12;

    public const byte NodeChanged = 21;
    public const byte AddSkid = 22;

    public const byte RaceEnded = 31;

    // Delegates
    public static event Action<int, int> OnPowerPickup = delegate { };
    public static event Action<bool, int, PowerUpsHandler.PowerUpType> OnPowerUsed = delegate { };
    //public static event Action<int, Vector3, Vector3, float, int> OnEmitSkid = delegate { };

    public static void PhotonRaiseEventsSender_All(byte eventCode, object[] eventContent)
    {

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);


    }

    public static void FreeModePickup()
    {

        OnPowerPickup(123, 123);

    }

    public static void PhotonRaiseEventsSender_MasterClient(byte eventCode, object[] eventContent)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
    }
    public static void PhotonRaiseEventsSender_Other(byte eventCode, object[] eventContent)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        /*if (photonEvent.Code < 200)
        {
            Debug.LogError("-------------------------------------");
            Debug.LogError("Code : " + photonEvent.Code);
            if (photonEvent.CustomData != null)
                foreach (var v in (object[])photonEvent.CustomData)
                    Debug.LogError("\tCustome Data : " + v);
            Debug.LogError("-------------------------------------");
        }*/

        int actorNo;
        object[] dataReceiver;
        switch (photonEvent.Code)
        {
            case MasterStartGame:
                //Debug.Log("MasterStartGame");
                PlayerListingManager.Instance.LoadRaceScene();
                break;
            case PlayerReady:
                dataReceiver = (object[])photonEvent.CustomData;
                actorNo = (int)dataReceiver[0];
                bool status = (bool)dataReceiver[1];
                NetworkPlayerManager.instance.SetPlayerReadyStatus(actorNo, status);
                break;
            case LevelLoaded:
                dataReceiver = (object[])photonEvent.CustomData;
                actorNo = (int)dataReceiver[0];

                //Debug.Log("..." + actorNo);
                //Debug.Log("..." + NetworkPlayerManager.instance);
                NetworkPlayerManager.instance.LevelLoaded(actorNo);
                break;
            case SetupRace:
                StartCoroutine(NetworkPlayerManager.instance.SetUp());
                break;
            case AllRacersReady:
                //Debug.Log("AllRacersReady");
                StartCoroutine(NetworkPlayerManager.instance.SetPlayerReady());
                break;
            case SetRacerPreferences:
                RGSK.RaceManager.instance.SetRacerPreferences();
                break;
            case RaceCountdownTimer:
                dataReceiver = (object[])photonEvent.CustomData;
                string timerMsg = dataReceiver[0].ToString();
                RGSK.RaceUI.instance.raceWaitTimeText.text = timerMsg;
                break;
            case StartRace:
                RGSK.RaceManager.instance.StartRaceCountdown();
                break;

            case PowerPick:
                dataReceiver = (object[])photonEvent.CustomData;
                actorNo = (int)dataReceiver[0];
                int pwrUuid = (int)dataReceiver[1];
                OnPowerPickup(actorNo, pwrUuid);
                break;
            case PowerUsed:
                dataReceiver = (object[])photonEvent.CustomData;
                bool active = (bool)dataReceiver[0];
                actorNo = (int)dataReceiver[1];
                PowerUpsHandler.PowerUpType pwType = (PowerUpsHandler.PowerUpType)dataReceiver[2];
                OnPowerUsed(active, actorNo, pwType);
                break;

            case NodeChanged:
                dataReceiver = (object[])photonEvent.CustomData;
                int cn = (int)dataReceiver[0];
                actorNo = (int)dataReceiver[1];

                NetworkPlayerManager.instance.NodeChanged(cn, actorNo);
                break;
            /*case AddSkid:
                dataReceiver = (object[])photonEvent.CustomData;
                actorNo = (int)dataReceiver[1];
                Vector3 pos = (Vector3)dataReceiver[2];
                Vector3 normal = (Vector3)dataReceiver[3];
                float intensity = (float)dataReceiver[4];
                int lastIndex = (int)dataReceiver[5];

                OnEmitSkid(actorNo, pos, normal, intensity, lastIndex);
                break;*/

            case RaceEnded:
                dataReceiver = (object[])photonEvent.CustomData;
                actorNo = (int)dataReceiver[0];

                NetworkPlayerManager.instance.RacerFinished(actorNo);
                break;
        }


    }
    #endregion


}