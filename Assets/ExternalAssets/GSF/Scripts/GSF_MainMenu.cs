using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GSF_MainMenu : MonoBehaviour
{
    [Header("Scene Selection")]
    public Scenes NextScene;
    [Header("Player Profile")]
    public Sprite[] avaterImages;
    public Image selectedAvatarFrame;
    public Text playerName;

    [Header("UI Panels")]
    public GameObject loadingPanel;
    public GameObject welcomeRewardPanel;
    public GameObject profileSetAskPanel;
    public GameObject HelpScreen;
    public GameObject ExitDialogue;
    public Button logoutBtn;

    [Header("Ad Sequence ID")]
    public int SequenceID;

    private UserDatabase db;

    [SerializeField] private AudioMixer mixer = null;
    private void OnEnable()
    {

    }
    void Start()
    {
        db = FindObjectOfType<UserDatabase>();

        // PlayerPrefs.DeleteAll ();
        Time.timeScale = 1;
        AudioListener.pause = false;

        if (!GameManager.Instance.Initialized)
        {
            InitializeGame();
        }

        InitializeUI();
        UpdateProfile();

        if (!PlayerPrefs.HasKey("WELCOME_REWARD") || PlayerPrefs.GetInt("WELCOME_REWARD") == 0)
        {
            PlayerPrefs.SetInt("WELCOME_REWARD", 1);
            PlayerPrefs.Save();

            //profileSetAskPanel.SetActive (true);

            //welcomeRewardPanel.SetActive (true);
            SaveData.Instance.Coins += 300;
            GSF_SaveLoad.SaveProgress();
        }
        if (ScreenFader.Instance)
            ScreenFader.Instance.FadeOut(1f);
        //Invoke("SetVolume", 0.1f

        mixer.SetFloat("MusicSFXVolume", PlayerPrefs.GetFloat("MusicSFXValue", 0));
        mixer.SetFloat("SoundSFXVolume", PlayerPrefs.GetFloat("SoundSFXValue", 0));

        logoutBtn.OnClick(() => UserDatabase.Instance.IsLogin = false);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        // ApiManager.instance.dumTokens.GetTokens();
    }

    public Text inGamecoinsText = null;
    public Text inGameDummyCoinsText = null;

    void InitializeGame()
    {
        SaveData.Instance = new SaveData();
        GSF_SaveLoad.LoadProgress();
        GameManager.Instance.Initialized = true;
    }

    void InitializeUI()
    {
        HelpScreen.SetActive(false);
        ExitDialogue.SetActive(false);
    }

    public void UpdateProfile()
    {

        selectedAvatarFrame.sprite = avaterImages[UserDatabase.Instance.localUserData.avtarId];


        //PlayerPrefs.SetInt("SELECTED_AVATAR", UserDatabase.Instance.localUserData.avtarId);

        //try
        //{
        if (db.IsLogin)
        {
            // playerName.text = db.localUserData.username;
            playerName.text = db.localUserData.nickname;
            string _value = string.Format("{0:#,###0} Raze", db.localUserData.userWalletData.ingameToken);
            inGamecoinsText.text = _value;
            _value = string.Format("{0:#,###0} DCoins", db.localUserData.tokens.dummytokens);
            inGameDummyCoinsText.text = _value;
            //Invoke("GetDummyTokens", 2);
        }
        //}
        //catch { }
    }

    private void GetDummyTokens()
    {
        string _value = string.Format("{0:#,###0} DCoins", ApiManager.instance.dumTokens.tokens.dummytokens);
        inGameDummyCoinsText.text = _value;
        Debug.LogError(_value);
    }
    public void OnClickGameMode(int mode)
    {
        PhotonManager.FreeToPlay = false;
        //if (PhotonManager.IsGuest)
        //{
        //    if (mode == 3)
        //    {

        //        loadingPanel.SetActive(true);
        //        BlockchainDataManager.currentGameMode = (GameMode)mode;
        //        SceneManager.LoadSceneAsync(NextScene.ToString());
        //    }
        //    else
        //    {
        //        Debug.LogError("Message: Guest user being used other functionalities");
        //        //Helper.ShowMessage(uiManager.logText, _loginData.status.msg, Color.red, 2f);
        //    }
        //    return;
        //}
        /*if (!BlockchainDataManager.instance.WalletData.isLogin)
        {
            FloatingWindowManager.instance.OpenInfoPanel(GenericStringKeys.WalletNotConnected);
            return;
        }*/

        //TURNON Later
        if (BlockchainDataManager.instance.WalletData.isLogin)
        {
            loadingPanel.SetActive(true);
            BlockchainDataManager.currentGameMode = (GameMode)mode;
            SceneManager.LoadSceneAsync(NextScene.ToString());
        }
        else
        {

            FloatingWindowManager.instance.OpenInfoPanel(GenericStringKeys.WalletNotConnected);


        }
    }

    public void OnClickFreeToPlayMode(int mode)
    {
        if (BlockchainDataManager.instance.WalletData.isLogin)
        {
            loadingPanel.SetActive(true);
            BlockchainDataManager.currentGameMode = (GameMode)mode;
            SceneManager.LoadSceneAsync(NextScene.ToString());
            PhotonManager.FreeToPlay = true;
        }
        else
        {
            FloatingWindowManager.instance.OpenInfoPanel(GenericStringKeys.WalletNotConnected);
        }
    }

    public void ExitPopUp()
    {
        ExitDialogue.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetSaveData()
    {
        SaveData.Instance = null;
        GSF_SaveLoad.DeleteProgress();
        SaveData.Instance = new SaveData();
        GSF_SaveLoad.LoadProgress();
    }

    #region Instance
    static GSF_MainMenu _instance;
    public static GSF_MainMenu Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GSF_MainMenu>();
            return _instance;
        }
    }
    #endregion
}