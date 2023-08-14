using System.Collections;
using UnityEngine;

public class EnvironmentSelectionHandler : MonoBehaviour
{
    public GameObject[] allModes = null;
    public UnityEngine.UI.Button NeonMap;


    public Transform TournamentParent;
    public GameObject T_Prefab;

    public GameObject WagerMode;
    public GameObject T_mode;
    private void OnEnable()
    {
        Constants.isFromMainMenu = false;


        if (NeonMap)
        {
            if (PhotonManager.FreeToPlay)
            {
                NeonMap.interactable = false;
                NeonMap.transform.Find("BG").GetComponent<UnityEngine.UI.Image>().color = new Color32((byte)68, (byte)68, (byte)68, (byte)255);
            }
            else
            {
                NeonMap.interactable = true;
                NeonMap.transform.Find("BG").GetComponent<UnityEngine.UI.Image>().color = new Color32((byte)255, (byte)255, (byte)255, (byte)255);
            }
        }
        try
        {
            CancelInvoke();
        }
        catch { }
        SetMode();
    }
    public void SelectLevel(int number)
    {
        Debug.Log("selectedd level");
        Constants.selectedLevel = number;
        for (int i = 0; i < allModes.Length; i++)
            allModes[i].SetActive(false);
        allModes[number].SetActive(true);
        Constants.isFromMainMenu = true;
    }

    public GameObject loadingObject = null;
    public string nextSceneName = "GamePlay";

    GameMode gameMode;
    public void PlayLevel(int modeId)
    {
        //if(PhotonManager.IsGuest && modeId != 0)
        //{
        //    return;
        //}

        Constants.modeId = modeId;
        // loadingObject.SetActive(true);

        GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.gameObject.SetActive(true);


        loadingObject.SetActive(true);
        PhotonManager.instance.Init();
        gameMode = BlockchainDataManager.currentGameMode;
        PhotonManager.PhotonServerJoiningStatus += PhotonServerJoiningStatus;

        if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
        {
            GSF_PlayerSelection.Instance.Selection_UI.WagetAmountWindow.SetActive(false);
            GSF_PlayerSelection.Instance.Selection_UI.PlayerListingWindow.SetActive(true);
        }
        else
        {
            GSF_PlayerSelection.Instance.Selection_UI.WagetAmountWindow.SetActive(true);
            GSF_PlayerSelection.Instance.Selection_UI.PlayerListingWindow.SetActive(false);
            // GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.Init(GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.gm);
        }
        //SceneManager.LoadSceneAsync(nextSceneName);
    }

    private void PhotonServerJoiningStatus(CallbackStatus status)
    {
        switch (status)
        {
            case CallbackStatus.Success:
                PhotonManager.PhotonServerJoiningStatus -= PhotonServerJoiningStatus;
                PhotonManager.LobbyJoiningStatus += LobbyJoiningStatus;
                PhotonManager.instance.JoinLobby(gameMode);
                Debug.Log("join server");
                break;
            case CallbackStatus.Failed:
                Debug.Log("Failed to join server");
                break;
            case CallbackStatus.None:
                Debug.Log("Failed to join server");
                break;
        }
    }

    private void LobbyJoiningStatus(CallbackStatus status)
    {
        switch (status)
        {
            case CallbackStatus.Success:
                PhotonManager.LobbyJoiningStatus -= LobbyJoiningStatus;
                GSF_PlayerSelection.Instance.Selection_UI.waitingRoom.GameMode = gameMode;
                loadingObject.SetActive(false);
                Debug.Log("join Lobby");
                break;
            case CallbackStatus.Failed:
                Debug.Log("Failed to join Lobby");
                break;
            case CallbackStatus.None:
                Debug.Log("Failed to join Lobby2");
                break;
        }


    }

    #region Instance
    static EnvironmentSelectionHandler _instace;
    public static EnvironmentSelectionHandler Instance
    {
        get
        {
            if (_instace == null)
                _instace = FindObjectOfType<EnvironmentSelectionHandler>();
            return _instace;
        }
    }
    #endregion


    #region Tournament setup

    public void SetMode()
    {
        if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
        {
            WagerMode.SetActive(false);
            T_mode.SetActive(true);

            InvokeRepeating("SetUpTournament", 0.1f, 20);   //SetUpTournament();
        }
        else
        {
            WagerMode.SetActive(true);
            T_mode.SetActive(false);
        }
    }

    public void SetUpTournament()
    {
        int j = 0;
        foreach (Transform t in TournamentParent)
        {
            if (j != 0)
                Destroy(t.gameObject);

            j++;
        }

        for (int i = 0; i < ApiManager.instance.tournamentData.tournamentData.Tournament.Count; i++)
        {
            GameObject obj = Instantiate(TournamentParent.GetChild(0).gameObject, TournamentParent);
            TournamentData t = ApiManager.instance.tournamentData.tournamentData.Tournament[i];
            TournamentWindow tw = obj.GetComponent<TournamentWindow>();
            obj.transform.localPosition = Vector3.zero;



            tw._id = t._id;
            tw.status = t.status;
            tw.tournamentId = t.tournamentId;
            tw.numberofplayers = t.numberofplayers;
            // tw.housecut = t.housecut;
            tw.nameoftournament = t.nameoftournament;
            tw.starttiming = t.starttiming;
            tw.Endtiming = t.endingTime;
            tw.MapName = t.tournamentmap;
            tw.numberofplayers = t.numberofplayers;
            
            tw.SetInfo();

            obj.SetActive(true);
            TournamentParent.GetChild(0).gameObject.SetActive(false);

            System.DateTime endtime = System.DateTime.Parse(t.endingTime);

            if (endtime < System.DateTime.Now)
                obj.transform.SetAsLastSibling();
            else
                obj.transform.SetSiblingIndex(1);

          //  StartCoroutine(ForceScrollDown(TournamentParent.parent.GetComponent<UnityEngine.UI.ScrollRect>()));
        }
    }


    IEnumerator ForceScrollDown(UnityEngine.UI.ScrollRect slider)
    {
        // Wait for end of frame AND force update all canvases before setting to bottom.
        yield return new WaitForSeconds(0.5f);
        Canvas.ForceUpdateCanvases();
        slider.horizontalNormalizedPosition = 0f;
        //SliderPrivate.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
    #endregion 
}
