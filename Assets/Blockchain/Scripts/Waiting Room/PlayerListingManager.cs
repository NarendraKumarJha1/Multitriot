using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Globalization;

public class PlayerListingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text roomInfo;
    [SerializeField] Button startBtn, backBtn;
    [SerializeField] Transform playerContainer, notificationContainer;
    [SerializeField] GameObject playerPrefab, notificationPrefab;

    [SerializeField] List<Player> roomPlayers = new List<Player>();
    int playerID;
    private void OnEnable()
    {
        Init();

        PhotonManager.PlayerEnterExit += OnPlayerCountChange;


        // Debug.LogError("dasdasd" + timme);
        canSpawn = false;
    }
    private void OnDisable()
    {
        PhotonManager.PlayerEnterExit -= OnPlayerCountChange;
    }



    void Init()
    {
        foreach (Transform t in playerContainer) Destroy(t.gameObject);
        foreach (Transform t in notificationContainer) Destroy(t.gameObject);

        backBtn.OnClick(OnLeaveRoom);

        //OnPlayerCountChange(PhotonNetwork.LocalPlayer, true);

        if (PhotonNetwork.IsMasterClient)
        {
            startBtn.gameObject.SetActive(true);
            startBtn.OnClick(StartGame);
        }
        else
            startBtn.gameObject.SetActive(false);

        if (PhotonManager.IsGuest)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient);
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("start game");
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;

        object[] _data = new object[] { };
        PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.MasterStartGame, _data);

        //LoadRaceScene();

    }

    private bool cancountdown;
    [SerializeField]
    private Text Countdown;
    private TimeSpan ts;

    DateTime timme;
    private void Update()
    {


        if (cancountdown)
        {


            DateTime now = DateTime.Now;


            TimeSpan ts = timme - now;




            if (ts.Minutes == 0 && ts.Seconds < 1)
            {
                //startgame
                if (!canSpawn)
                {
                    startBtn.onClick.Invoke();
                    canSpawn = true;
                }
            }
            else
            {
                Countdown.text = "Game starts in " + ts.Minutes + ":" + ts.Seconds;
            }
        }
    }


    Coroutine loadLevelCoroutine = null;
    public void LoadRaceScene()
    {
        NetworkPlayerManager.instance.InitPlayers();

        EnvironmentSelectionHandler.Instance.loadingObject.SetActive(true);

        Debug.Log("Loading LoadRaceScene.." + GSF_PlayerSelection.Instance.NextScene.ToString());
        if (loadLevelCoroutine != null)
            StopCoroutine(loadLevelCoroutine);

        loadLevelCoroutine = null;
        loadLevelCoroutine = StartCoroutine(LoadLevelAsync(GSF_PlayerSelection.Instance.NextScene.ToString()));
    }
    private IEnumerator LoadLevelAsync(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitUntil(() => async.isDone);

        yield return new WaitForSeconds(1.0f);

        EnvironmentSelectionHandler.Instance.loadingObject.SetActive(false);

        Debug.Log("Loaded!!!");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        if (loadLevelCoroutine != null)
            StopCoroutine(loadLevelCoroutine);
    }

    void OnLeaveRoom()
    {

        PhotonNetwork.LeaveRoom();

        // PhotonNetwork.Disconnect();
    }
    const string dateFormat = "ddd MMM dd HH\':\'mm\':\'ss \'GMT\'K yyyy";
    private void OnPlayerCountChange(Player player, bool isEntered)
    {
        CheckStartBtnStatus();
        string msg = string.Format(isEntered ? GenericStringKeys.PlayerJoin : GenericStringKeys.PlayerLeft, player.NickName);
        Debug.Log("Msg: " + msg);

        if (isEntered)
        {
            string tokenname = "";
            if (!PhotonManager.FreeToPlay)
                tokenname = "Coin";
            else
                tokenname = "Dummy";

            if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
                tokenname = "Fuel";


            string roomInfoMsg =
                "Room name: " + PhotonNetwork.CurrentRoom.Name + "\n\n" +
                "Players: " + PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers + "\n\n" +
                "Entry Fee: " + GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.selectedRoomAmt + " " + tokenname;
            roomInfo.text = roomInfoMsg;
            foreach (Player _p in PhotonNetwork.PlayerList)
            {
                
                Player p = roomPlayers.Find(x => x == _p);
                if (p == null)
                {
                    WR_Player newP = Instantiate(playerPrefab, playerContainer).GetComponent<WR_Player>();
                    newP.Init(_p);
                    roomPlayers.Add(_p);
                }
            }


            SetPlayerID();
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {

                string time = (string)PhotonNetwork.CurrentRoom.CustomProperties["time"];

                if (PhotonNetwork.IsMasterClient)
                {
                    if (string.IsNullOrEmpty(time))
                    {
                        DateTime dt = DateTime.Now;

                        dt = dt.AddSeconds(10);

                        string nowString = dt.ToString(dateFormat, CultureInfo.InvariantCulture);

                        //"MM/dd/yyyy h:mm:ss tt"



                        ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable
                        {
                          { "time", nowString }
                        };

                        PhotonNetwork.CurrentRoom.SetCustomProperties(data);
                    }
                }
                CancelInvoke();
                Invoke("CheckForPlayers", 2);
            }
        }
        else
        {
            Player p = roomPlayers.Find(x => x == player);
            if (p != null)
                roomPlayers.Remove(p);

            if (player == PhotonNetwork.LocalPlayer)
                GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.ToggelAmtPanel(true);

            string tokenname = "";
            if (!PhotonManager.FreeToPlay)
                tokenname = "Coin";
            else
                tokenname = "Dummy";

            if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
                tokenname = "Fuel";
            try
            {
                string roomInfoMsg =
                        "Room name: " + PhotonNetwork.CurrentRoom.Name + "\n\n" +
                        "Players: " + PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers + "\n\n" +
                        "Entry Fee: " + GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.selectedRoomAmt + " " + tokenname;
                roomInfo.text = roomInfoMsg;
            }
            catch { }

            if (player == PhotonNetwork.LocalPlayer)
            {
                GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.backBtn.onClick.Invoke();
            }

            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    cancountdown = false;
                    Countdown.text = "Waiting for more players...";
                    ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable
                    {
                    { "time", "" }
                    };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(data);
                }
            }
            catch { }
        }

        if (player != PhotonNetwork.LocalPlayer)
        {
            StartCoroutine(ShowNotification(msg));
            InvokeRepeating("checkForPlayers", 2, 2);
        }





    }
    //Custom made function for giving the player ID to players in the room by nj 
    private void SetPlayerID()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                playerID = 0;
                UserDatabase.Instance.localUserData.id = playerID.ToString();
                ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable();
                data.Add("playerID", playerID);
                PhotonNetwork.CurrentRoom.SetCustomProperties(data);
            }
            else if (!PhotonNetwork.IsMasterClient)
            {
                playerID = 1;
                UserDatabase.Instance.localUserData.id = playerID.ToString();
                ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable();
                data.Add("playerID", playerID);
                PhotonNetwork.CurrentRoom.SetCustomProperties(data);
            }
        }
    }

    string timer;
    private bool canSpawn;
    private void CheckForPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            timer = (string)PhotonNetwork.CurrentRoom.CustomProperties["time"]; // "10/22/2022 2:48:46";  //
            cancountdown = true;
            //Debug.LogError(timer);
            timme = DateTime.ParseExact(timer, dateFormat, CultureInfo.InvariantCulture);
        }
    }

    private void checkForPlayers()
    {
        CheckStartBtnStatus();

        if (PhotonNetwork.IsMasterClient)
        {
            startBtn.gameObject.SetActive(true);
            startBtn.onClick.RemoveAllListeners();
            startBtn.OnClick(StartGame);
        }
    }
    void CheckStartBtnStatus()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient && !PhotonNetwork.InRoom) return;

#if UNITY_EDITOR
        startBtn.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 0;
#else
        startBtn.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;

         if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            startBtn.interactable = true;
        }
        else
        {
            startBtn.interactable = false;
        }
#endif


    }

    IEnumerator ShowNotification(string msg)
    {
        Text notifiaction = Instantiate(notificationPrefab, notificationContainer).GetComponent<Text>();
        notifiaction.text = msg;

        yield return new WaitForSeconds(10f);

        Destroy(notifiaction.gameObject);
    }

    #region Instance
    static PlayerListingManager _instace;
    public static PlayerListingManager Instance
    {
        get
        {
            if (_instace == null)
                _instace = FindObjectOfType<PlayerListingManager>();
            return _instace;
        }
    }
    #endregion
}