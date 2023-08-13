using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public enum GameMode { Wager = 1, Tournament = 2, Freemode = 3 };

public class WaitingRoomManager : MonoBehaviour
{
    public GameMode gm = GameMode.Wager;
    public GameMode GameMode
    {
        set
        {
            gm = value;
            Init(gm);
        }
    }

    [Header("Top Bar")]
    public Text titleText;
    public Button backBtn;

    [Header("Amount Room")]
    [SerializeField] GameObject amountPanel;
    [SerializeField] Transform roomAmtContainer;
    [SerializeField] GameObject roomPrefab;

    [Header("Waiting Room")]
    [SerializeField] GameObject waitingRoom;

    public GameObject LoadingScreen;
    public int selectedRoomAmt = 0;

    public delegate void OnClickRoomBtn(RoomPrefab roomPrefab);
    public static OnClickRoomBtn onClickRoomBtn;

    private void OnEnable()
    {
        onClickRoomBtn += OnGetButtonRespounse;
        PhotonManager.RoomJoiningStatus += RoomJoiningStatus;

        if(PhotonManager.IsGuest)
            LoadingScreen.SetActive(true);
        else
            LoadingScreen.SetActive(false);

       

    }
    private void OnDisable()
    {
        onClickRoomBtn -= OnGetButtonRespounse;
        PhotonManager.RoomJoiningStatus -= RoomJoiningStatus;
    }

    private void Start()
    {
        //if(PhotonManager.IsGuest)
        //{
        //    PhotonManager.instance.JoinRandomRoom(0);
        //}

    }

    

    public void Init(GameMode mode)//DEV
    {
        Debug.Log("INIT gamemode :- "+ GameMode.Wager);
        ToggelAmtPanel(true);
        gm = mode;

        foreach (Transform t in roomAmtContainer) Destroy(t.gameObject);

        titleText.text = gm == GameMode.Wager ? GenericStringKeys.Wager : GenericStringKeys.Tournament;

        Debug.Log("INIT gm :- "+ gm);
        if (PhotonManager.IsGuest)
        {
            titleText.text = "Free Mode";
            RoomPrefab rp = Instantiate(roomPrefab, roomAmtContainer).GetComponent<RoomPrefab>();
            rp.Init(100);
        }

        if (PhotonManager.FreeToPlay)
            titleText.text = "Free To Play Mode";

        if (gm == GameMode.Wager)
        {

            if (!PhotonManager.FreeToPlay)
            {
                foreach (int amt in GlobalVariables.instance.roomEntryAmt)
                {
                    RoomPrefab rp = Instantiate(roomPrefab, roomAmtContainer).GetComponent<RoomPrefab>();
                    rp.Init(amt);
                    break;
                }
            }
            else
            {
                RoomPrefab rp = Instantiate(roomPrefab, roomAmtContainer).GetComponent<RoomPrefab>();
                rp.Init(100);
            }
            //Debug.LogError("Reached Wager mode");
        }
        else if (gm == GameMode.Tournament)
        {
            Debug.LogError("Reached Tournament mode");
           // int amt = System.Convert.ToInt32(PhotonManager.RoomAmt);
           // PhotonManager.instance.JoinOrCreateRoom(PhotonManager.RoomAmt);
            PhotonManager.instance.JoinRandomRoom(255);
        }
        else if (gm == GameMode.Freemode)
        {
            Debug.LogError("Reached Free mode");
        }

        if (PhotonManager.IsGuest)
            PhotonManager.instance.JoinRandomRoom(255);
    }

    public void ToggelAmtPanel(bool openAmtPanel)
    {
        backBtn.gameObject.SetActive(openAmtPanel);
        if (openAmtPanel)
            backBtn.OnClick(PhotonNetwork.Disconnect);
       
        amountPanel.SetActive(openAmtPanel);
        waitingRoom.SetActive(!openAmtPanel);
        //PhotonNetwork.Disconnect();
    }

    void OnGetButtonRespounse(RoomPrefab roomPrefab)
    {
        PhotonManager.RoomAmtT = selectedRoomAmt = int.Parse(roomPrefab.roomAmount.text);
        
        PhotonManager.instance.JoinRandomRoom(selectedRoomAmt);
    }

    private void RoomJoiningStatus(CallbackStatus status)
    {
        switch (status)
        {
            case CallbackStatus.Success:
                ToggelAmtPanel(false);
                break;
            case CallbackStatus.Failed:
                if (selectedRoomAmt == 0) return;
                PhotonManager.instance.JoinRandomRoom(selectedRoomAmt);
                break;
            case CallbackStatus.None: break;
        }
    }
}