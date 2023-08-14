using System;
using UnityEngine;
using System.Collections;
using SimpleJSON;
using Newtonsoft.Json;

[Serializable]
public struct WalletData
{
    public bool isLogin;
    public string walletAddress;
    public string walletBalance;
}

public class BlockchainDataManager : MonoBehaviour
{
    public static BlockchainDataManager instance;
    [Header("Testing")]
    public string dummyAccount = "0x62a20044C8CD64b74a51Bb321eEdE6c6DaeF0691";
    public bool useDebugger = true;
    public GameObject debugger;

    [Space(5)]
    public TransferShoeToken transferShoeToken;

    [Space(5)]
    [SerializeField] WalletData _walletData;
    public WalletData WalletData
    {
        get => _walletData;
        set { _walletData = value; OnWalletDataChanged(_walletData); }
    }
    public event Action<WalletData> OnWalletDataChanged = delegate { };

    public static bool rememberme = false;

    #region Network
    public const string Blockchain =   "Binance"; //"ethereum";
    public const string Network =  "Testnet";  //rinkeby";
    public const int ChainId = 97; //4;
    #endregion

    #region Game Wallet Data
    public const string ShowToken = "0xD8087bDDBA4330CD44a20fFA84b4A1ee80c1a3D3"; //"0x8F973d1C33194fe773e7b9242340C3fdB2453b49";
    public const string gameWalletAddress = "0xB45AA2e895B1Fa27e0d08c4F2472AFcD99ECC3a6";
    #endregion

    public static GameMode currentGameMode = GameMode.Wager;

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

#if UNITY_EDITOR
        if (debugger != null) debugger.SetActive(useDebugger);
#else
        if (debugger != null) debugger.SetActive(useDebugger);
#endif
    }
/*
    void Start()
    {
        // Get the current Android activity.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // Get the intent that started this activity.
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

        // Get the data sent from the Android app.
        string data = intent.Call<string>("getStringExtra", "android.intent.extra.TEXT");
        int myInteger = intent.Call<int>("getIntExtra", "myInteger", 5); // 5 is the default value
        PlayerPrefs.SetString("EscrowId",data);
        Debug.Log("Data received: " + PlayerPrefs.GetString("EscrowId"));
    }*/

    void Start()
    {
        // Get the current Android activity.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // Get the intent that started this activity.
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

        // Get the data sent from the Android app.
        string data = intent.Call<string>("getStringExtra", "game_data");

        // Check if the data is not null before using it.
        if (!string.IsNullOrEmpty(data))
        {
            Debug.Log("Data received: " + PlayerPrefs.GetString("EscrowId"));
            UnpackData(data);
        }
        else
        {
            Debug.Log("No data received.");
        }
    }
    public class Json
    {
        public string escrowId { get; set; }
        public string walletAddress { get; set; }
    }
    public void UnpackData(string json)
    {
        Json obj = JsonConvert.DeserializeObject<Json>(json);

        string EscrowID = obj.escrowId;
        string WalletAddress = obj.walletAddress;
        Debug.Log("EscrowId "+ EscrowID + " WalletAddress "+ WalletAddress);
        PlayerPrefs.SetString("EscrowId", EscrowID);
        PlayerPrefs.SetString("WalletAddress", WalletAddress);
    }

    private void OnApplicationQuit()
    {
        if (rememberme) return;

        if (PlayerPrefs.HasKey(GenericStringKeys.loginAuth))
        {
            string authData = PlayerPrefs.GetString(GenericStringKeys.loginAuth, string.Empty);

            string[] auth = authData.Split('|');
            if (!bool.Parse(auth[2]))
            {
                Debug.LogError("Deleting Auth Data ");
                PlayerPrefs.DeleteKey(GenericStringKeys.loginAuth);
            }
        }
    }
}