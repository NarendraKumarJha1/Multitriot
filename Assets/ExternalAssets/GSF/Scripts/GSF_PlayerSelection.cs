using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Selection_Elements
{
    public GameObject LoadingScreen;
    public Image FillBar;
    public Image lockedIndicator;
    public Text availableCashText;
    public GameObject priceTag;
    public GameObject WagetAmountWindow, PlayerListingWindow,floatingWindow;
    [Header("Player Attributes")]
    public Text nameText;
    public Image Speed_Bar;
    // public Text speedValue;
    public Image Handling_Bar;
    // public Text hndlingValue;
    public Image Acceleration_Bar;
    public Image lastBar;
    public Text topSpeedText = null, rpmText = null, frontSuspensionText = null, rearSuspensionText = null;
    // public Text AccelerationValue;
    public Text InfoText;

    [Header("UI Buttons")]
    public Button PlayLevelsBtn;
    public Button customizeBtn;
    public GameObject NextBtn;
    public GameObject PrevBtn;
    public Button buyBtn;
    
    [Header("Waiting Room")]
    public WaitingRoomManager waitingRoom;
}

[System.Serializable]
public class PlayerAttributes
{
    public string Name;
    [Tooltip("Text to display when this Player is locked.")]
    [Multiline]
    public string Info;
    public GameObject PlayerObject;
    public GameObject revertPlayerObject;
    [Range(0, 500)]
    public int Speed;
    [Range(0, 200)]
    public int Handling;
    [Range(0, 300)]
    public int Acceleration;
    [Range(0, 300)]
    public int lastValue;
    public int topSpeed = 220, RPM = 7000, frontSuspension = 40000, rearSuspension = 50000;
    public bool Locked;
    public int cashPrice;

    [Tooltip("Enter Level Number which must be unlocked to get this Player.")]
    public int LevelRequired = -1;

    [Header("Customization")]
    public int currentBodyColorIndex = -1;
    public int currentRimEmissionColorIndex = -1;

}

[System.Serializable]
public struct PlayerSaveableAttributes
{
    public string Name;
    // public int rewardedVideoPrice;
    public bool Locked;
    public PlayerCustomization playerCustomization;

}

[System.Serializable]
public struct PlayerCustomization
{
    public int bodyColorIndex;
    public int rimEmissionIndex;
}

public class GSF_PlayerSelection : MonoBehaviour
{
    [Header("Scene Selection")]
    public Scenes PreviousScene;
    public Scenes NextScene;

    [Header("UI Elements")]
    public Selection_Elements Selection_UI;

    [Header("Player Profile")]
    public Sprite[] avaterImages;

    [Header("Player Attributes")]
    public PlayerAttributes[] Players;
    public Transform vehicleDropPosition;
    public GameObject UnlockAllPlayersBtn;

    [Header("Ad Sequence ID")]
    public int SequenceID;
    public bool LoadingSequence;
    public int LoadingSequenceID;
    public int rewardedVideoSequenceId = 4;

    public GameObject liveView;

    AsyncOperation async = null;
    private int current;

    private CameraRotate orbitCamera;
    private UnityStandardAssets.ImageEffects.CameraFocus dof;

    public UnityEngine.Events.UnityEvent onSelectionChanged;

    void LocalLoadSavedPlayersData()
    {
        int i = 0;
        foreach (PlayerSaveableAttributes p in SaveData.Instance.players)
        {

            Players[i].Locked = p.Locked;
            Players[i].currentBodyColorIndex = p.playerCustomization.bodyColorIndex;
            Players[i].currentRimEmissionColorIndex = p.playerCustomization.rimEmissionIndex;

            i++;
        }
    }

    void LocalSavePlayersData()
    {
        List<PlayerSaveableAttributes> temp = new List<PlayerSaveableAttributes>();

        foreach (PlayerAttributes p in Players)
        {
            PlayerSaveableAttributes psa = new PlayerSaveableAttributes();
            psa.Name = p.Name;
            psa.Locked = p.Locked;
            psa.playerCustomization.bodyColorIndex = p.currentBodyColorIndex;
            psa.playerCustomization.rimEmissionIndex = p.currentRimEmissionColorIndex;

            temp.Add(psa);
        }

        SaveData.Instance.players = temp.ToArray();
        GSF_SaveLoad.SaveProgress();
    }
    private void OnEnable()
    {
        if (Constants.openEnvironmentSelection)
            nextScreen.SetActive(true);
        Constants.openEnvironmentSelection = false;
    }
    void Start()
    {
        liveView.SetActive(true);
        if (ScreenFader.Instance)
            ScreenFader.Instance.FadeOut(1f);

        orbitCamera = FindObjectOfType<CameraRotate>();
        dof = FindObjectOfType<UnityStandardAssets.ImageEffects.CameraFocus>();

        Time.timeScale = 1;
        AudioListener.pause = false;
        if (PlayerPrefs.HasKey("MASTER_VOLUME"))
        {
            // AudioListener.volume = PlayerPrefs.GetFloat("MASTER_VOLUME"); //$ree
        }
        Selection_UI.FillBar.fillAmount = 0;
        Selection_UI.LoadingScreen.SetActive(false);
        if (!GameManager.Instance.Initialized)
        {
            InitializeGame();
        }

        if (SaveData.Instance.players == null || SaveData.Instance.players.Length < Players.Length)
            LocalSavePlayersData();
        else
            LocalLoadSavedPlayersData();

        UpdateUI();
        onSelectionChanged.Invoke();

        if (PlayerPrefs.GetInt(Helper.UnlockAllPlayer_str) == 1 || PlayerPrefs.GetInt(Helper.UnlockAllEverything_str) == 1)
        {
            UnlockAllPlayersBtn.SetActive(false);
        }

        if (PhotonManager.IsGuest)
        {
            current = 5;
            GameManager.Instance.CurrentPlayer = current;
            UpdateUI();
            onSelectionChanged.Invoke();
        }
    }

    void InitializeGame()
    {
        SaveData.Instance = new SaveData();
        GSF_SaveLoad.LoadProgress();
        GameManager.Instance.Initialized = true;
    }


    void Update()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                BackBtn();
            }
        }
        if (async != null)
        {
            Selection_UI.FillBar.fillAmount = async.progress;
            if (async.progress >= 0.9f)
            {
                Selection_UI.FillBar.fillAmount = 1.0f;
            }
        }
    }

    void UpdateUI()
    {

        for (int i = 0; i < Players.Length; i++)
        {
            if (i == current)
            {
                //Players[i].PlayerObject.transform.position = vehicleDropPosition.position;
                Players[i].PlayerObject.SetActive(true);
                Players[i].revertPlayerObject.SetActive(true);
                if (dof)
                    //dof.transform = Players[i].PlayerObject.transform;

/*                RCC_CameraConfig playerCameraConfig = Players[i].PlayerObject.GetComponent<RCC_CameraConfig>();
*/                if (orbitCamera)
                {
/*                    orbitCamera.averageDistance = playerCameraConfig.distance;
                    orbitCamera.targetOffset = Vector3.up * playerCameraConfig.height;*/    
                }
                    orbitCamera.Init();
            }
            else if (i != current)
            {
                //Players[i].PlayerObject.transform.position = vehicleDropPosition.position;
                Players[i].PlayerObject.SetActive(false);
                Players[i].revertPlayerObject.SetActive(false);
            }
        }
        Selection_UI.nameText.text = "";
        Selection_UI.nameText.DOText(Players[current].Name, 0.5f);
        Selection_UI.Speed_Bar.DOFillAmount(Players[current].Speed / 360.0f, 0.5f);
        Selection_UI.Handling_Bar.DOFillAmount(Players[current].Handling / 360.0f, 0.5f);
        Selection_UI.Acceleration_Bar.DOFillAmount(Players[current].Acceleration / 360.0f, 0.5f);
        Selection_UI.lastBar.DOFillAmount(Players[current].lastValue / 360.0f, 0.5f);
        Selection_UI.topSpeedText.text = "" + Players[current].topSpeed;
        Selection_UI.rpmText.text = "" + Players[current].RPM;
        Selection_UI.frontSuspensionText.text = "" + Players[current].frontSuspension;
        Selection_UI.rearSuspensionText.text = "" + Players[current].rearSuspension;
        Selection_UI.InfoText.text = "";

        if (Players[current].LevelRequired > 0)
        {
            if (Players[current].LevelRequired <= SaveData.Instance.Level)
            {
                Players[current].Locked = false;
                Selection_UI.lockedIndicator.gameObject.SetActive(false);
                Selection_UI.PlayLevelsBtn.gameObject.SetActive(true);
                //Selection_UI.customizeBtn.gameObject.SetActive (true);
                Selection_UI.buyBtn.gameObject.SetActive(false);
            }
            else
            {
                Players[current].Locked = true;
                Selection_UI.InfoText.text = Players[current].Info;
                Selection_UI.lockedIndicator.gameObject.SetActive(true);
                Selection_UI.PlayLevelsBtn.gameObject.SetActive(false);
                //Selection_UI.customizeBtn.gameObject.SetActive (false);
                Selection_UI.buyBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            if (Players[current].Locked)
            {
                Selection_UI.PlayLevelsBtn.gameObject.SetActive(false);
                //Selection_UI.customizeBtn.gameObject.SetActive (false);
                Selection_UI.lockedIndicator.gameObject.SetActive(true);
                Selection_UI.buyBtn.gameObject.SetActive(true);
                Selection_UI.priceTag.SetActive(true);
                Selection_UI.priceTag.GetComponentInChildren<Text>().text = Players[current].cashPrice + "";

            }
            else
            {
                Selection_UI.PlayLevelsBtn.gameObject.SetActive(true);
                //Selection_UI.customizeBtn.gameObject.SetActive (true);
                Selection_UI.lockedIndicator.gameObject.SetActive(false);
                Selection_UI.buyBtn.gameObject.SetActive(false);
                Selection_UI.priceTag.SetActive(false);
            }
        }

        if (current == 0)
        {
            Selection_UI.PrevBtn.SetActive(false);
            Selection_UI.NextBtn.SetActive(true);
        }
        else if (current == Players.Length - 1)
        {
            Selection_UI.PrevBtn.SetActive(true);
            Selection_UI.NextBtn.SetActive(false);
        }
        else
        {
            Selection_UI.PrevBtn.SetActive(true);
            Selection_UI.NextBtn.SetActive(true);
        }

        if (PhotonManager.FreeToPlay)
        {
            string _value = string.Format("{0:#,###0} Dummy coins", UserDatabase.Instance.localUserData.tokens.dummytokens);
            Selection_UI.availableCashText.text = _value;
        }
        else
        {
            string _value = string.Format("{0:#,###0} Raze coins", UserDatabase.Instance.localUserData.userWalletData.ingameToken);
            Selection_UI.availableCashText.text = _value;
        }
    }

    public void Previous()
    {
        if (PhotonManager.IsGuest)
            return;
        current--;
        GameManager.Instance.CurrentPlayer = current;
        UpdateUI();
        onSelectionChanged.Invoke();
    }

    public void Next()
    {
        if (PhotonManager.IsGuest)
            return;
        current++;
        GameManager.Instance.CurrentPlayer = current;
        UpdateUI();
        onSelectionChanged.Invoke();
    }
    public GameObject nextScreen = null;
    public void PlayLevels()
    {
#if UNITY_EDITOR
        GameManager.Instance.EditorSession = false;
#endif
        GameManager.Instance.CurrentPlayer = current;
        //StartCoroutine (LoadScene (NextScene.ToString ()));
        if (!PhotonManager.IsGuest)
        {
            nextScreen.SetActive(true);
            liveView.SetActive(false);
        }
        else
        {
            //GuestMode code goes here //NEWDEV
            nextScreen.SetActive(true);
            //SceneManager.LoadScene(NextScene.ToString());
            liveView.SetActive(false);
           
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        Selection_UI.LoadingScreen.SetActive(true);
        if (LoadingSequence)
        {
            //ShowAds (LoadingSequenceID, "Loading Screen");
            yield return new WaitForSeconds(3);
        }
        async = SceneManager.LoadSceneAsync(sceneName);
        yield return async;
    }
    public void BackBtn()
    {
        Destroy(FindObjectOfType<UserDatabase>().gameObject);
        SceneManager.LoadScene(PreviousScene.ToString());
    }

    public void BuyCurrentPlayerWithCashPrice()
    {

        if (SaveData.Instance.Coins >= Players[current].cashPrice)
        {
            Debug.Log("Current player index: " + current);

            SaveData.Instance.Coins -= Players[current].cashPrice;
            GSF_SaveLoad.SaveProgress();

            Players[current].Locked = false;
            LocalSavePlayersData();
            UpdateUI();
        }
        else
        {
            Helper.ShowMessage(Selection_UI.InfoText, "You don't have enough coins yet!", Color.yellow, 2f);
        }
    }

    public void UnlockAllPlayers()
    {
        Helper.UnlockAllPlayers();
        LocalLoadSavedPlayersData();
        UpdateUI();
    }

    #region Instance
    static GSF_PlayerSelection _instance;
    public static GSF_PlayerSelection Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GSF_PlayerSelection>();
            return _instance;
        }
    }
    #endregion
}