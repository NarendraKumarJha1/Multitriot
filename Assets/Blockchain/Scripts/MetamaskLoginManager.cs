using System;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class MetamaskLoginManager : MonoBehaviour
{
    #region Public Variables
    [SerializeField] Button metamaskLoginBtn;
    [SerializeField] Text walletAddressText;
    [SerializeField] Text walletBalanceText;
    [SerializeField] Button ingameTokenBtn;
    [SerializeField] Text InGameCoinsText;
    [SerializeField] GameObject HostQuitWindow;
    #endregion

    #region Private Variables
    [SerializeField] WalletData walletData;

#if UNITY_WEBGL
    [DllImport("__Internal")] private static extern void Web3Connect();
    [DllImport("__Internal")] private static extern string ConnectAccount();
    [DllImport("__Internal")] private static extern void SetConnectAccount(string value);
#endif
    #endregion

    private void OnEnable()
    {
        Init();
        BlockchainDataManager.instance.OnWalletDataChanged += WalletDataUpdated;


        if (PhotonManager.HostQuit)
        {
            //show the game ended UI
            HostQuitWindow.SetActive(true);
            PhotonManager.HostQuit = false;
        }

        if (Application.platform == RuntimePlatform.Android)
            Debug.Log("Do something special here");
    }
    private void OnDisable()
    {
        BlockchainDataManager.instance.OnWalletDataChanged -= WalletDataUpdated;
        CancelInvoke();
    }

    private void Init()
    {
        ingameTokenBtn.OnClick(OnClickIngame);
        SetupMetamaskBtn();
        SetupWallet();
    }

    void SetupMetamaskBtn()
    {
        if (BlockchainDataManager.instance.WalletData.isLogin)
        {
            metamaskLoginBtn.GetComponentInChildren<Text>().text = GenericStringKeys.Connected;
            metamaskLoginBtn.GetComponentInChildren<Text>().color = Color.white;
            metamaskLoginBtn.OnClick(Disconnected);
        }
        else
        {
            metamaskLoginBtn.GetComponentInChildren<Text>().text = GenericStringKeys.Disconnected;
            metamaskLoginBtn.GetComponentInChildren<Text>().color = Color.red;
            metamaskLoginBtn.OnClick(Login);
            CancelInvoke();
        }
    }

    void SetupWallet()
    {
        if (!BlockchainDataManager.instance.WalletData.isLogin)
            Disconnected();
        else
            WalletDataUpdated(BlockchainDataManager.instance.WalletData);
    }


    /// <summary>
    /// wallet will be connected **$ree
    /// waller login button functionality is added here 
    /// if it already logged in to the wallet it will logout. 
    /// </summary>
    public void Login()
    {
        //if (PhotonManager.IsGuest)
        //{
        //   // Debug.LogError("Message: Guest user being used other functionalities");
        //    return;
        //}

        FloatingWindowManager.instance.OpenInfoPanel();



#if UNITY_EDITOR && UNITY_ANDROID
        //OnLoginSuccess(BlockchainDataManager.instance.dummyAccount);
        OnConnect();
#elif UNITY_STANDALONE
        OnConnect();
#elif UNITY_WEBGL
        Web3Connect();
        OnConnected();
#elif UNITY_ANDROID
        /////write android code here
#endif

        Invoke("turnOffLoading", 20);
    }

    private void turnOffLoading()
    {
        FloatingWindowManager.instance.FadeOff();
    }

    /// <summary>
    /// wallet disconnection logic **$ree
    /// </summary>
    public void Disconnected()
    {
        FloatingWindowManager.instance.OpenInfoPanel();
        ResetWalletData();
        SetupMetamaskBtn();
        FloatingWindowManager.instance.FadeOff();
    }

    public void ResetWalletData()
    {
        walletData = new WalletData()
        {
            isLogin = false,
            walletAddress = GenericStringKeys.WalletAddress,
            walletBalance = GenericStringKeys.WalletCoin
        };
        BlockchainDataManager.instance.WalletData = walletData;
    }

#if UNITY_STANDALONE || UNITY_ANDROID
    async void OnConnect()
    {
        try
        {
            int timestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int expirationTime = timestamp + 60;
            string message = string.Format("{0} ({1})", "ShoeFy", expirationTime.ToString());
            string signature = await Web3Wallet.Sign(message);
            string account = await EVM.Verify(message, signature);
            int now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (account.Length == 42 && expirationTime >= now)
                OnLoginSuccess(account);
        }
        catch
        {
            FloatingWindowManager.instance.FadeOff();
            Debug.LogError("Error: failed to login / Rejected");
        }
    }
#elif UNITY_WEBGL
    async private void OnConnected()
    {
        string account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        OnMetamaskResponse();
        SetConnectAccount("");
    }

    async void OnMetamaskResponse()
    {
        try
        {
            string message = "ShoeFy";
            string response = await Web3GL.Sign(message);
            VerifySign(response, message);
        }
        catch
        {
            FloatingWindowManager.instance.FadeOff();
            Debug.LogError("Error: failed to login / Rejected");
        }
    }
    async void VerifySign(string signature, string msg)
    {
        string address = await EVM.Verify(msg, signature);
        OnLoginSuccess(address);
    }
#endif

    void OnLoginSuccess(string address)
    {
        walletData.isLogin = true;
        walletData.walletAddress = address;
        GetWalletBalance();

        if (UserDatabase.Instance.localUserData.metamaskId.Length < 5)
        {
            //StartCoroutine(PostMetamaskID(Constants.BaseUrl + "/addmetamask"));

            StartCoroutine(CheckMetamaskID(Constants.BaseUrl + "/addmetamask"));
        }

        InvokeRepeating("GetWalletBalanceUpdate", 10, 15);
    }

    async void GetWalletBalance()
    {
       // Debug.LogError(BlockchainDataManager.Network + " = " + BlockchainDataManager.ShowToken + " = " + walletData.walletAddress);
        BigInteger balance = await ERC20.BalanceOf(BlockchainDataManager.Blockchain, BlockchainDataManager.Network, BlockchainDataManager.ShowToken, walletData.walletAddress);
        walletData.walletBalance = string.Format("{0:0}", balance);

        BlockchainDataManager.instance.WalletData = walletData;

        SetupMetamaskBtn();
        FloatingWindowManager.instance.FadeOff();
    }

    public async void GetWalletBalanceUpdate()
    {
        if (BlockchainDataManager.instance.WalletData.isLogin)
        {
            // Debug.LogError(BlockchainDataManager.Network + " = " + BlockchainDataManager.ShowToken + " = " + walletData.walletAddress);
            BigInteger balance = await ERC20.BalanceOf(BlockchainDataManager.Blockchain, BlockchainDataManager.Network, BlockchainDataManager.ShowToken, walletData.walletAddress);
            walletData.walletBalance = string.Format("{0:0}", balance);

            BlockchainDataManager.instance.WalletData = walletData;

            StartCoroutine(ApiManager.instance.loginApi.CheckInGameToken(Constants.BaseUrl + "/checkingametoken"));
            //SetupMetamaskBtn();
            //FloatingWindowManager.instance.FadeOff();

            string _value = string.Format("{0:#,###0} Raze", UserDatabase.Instance.localUserData.userWalletData.ingameToken);
/*           GSF_MainMenu.Instance.inGamecoinsTextShop.text = InGameCoinsText.text = _value;
*/        }
    }

    private void WalletDataUpdated(WalletData updatedData)
    {
        walletAddressText.FormatAddress(updatedData.walletAddress, updatedData.isLogin);
        walletBalanceText.FormatBalance(string.Format("{0:0}", updatedData.walletBalance, updatedData.isLogin));
    }

    void OnClickIngame()
    {
        if (!BlockchainDataManager.instance.WalletData.isLogin)
        {
            FloatingWindowManager.instance.OpenInfoPanel(GenericStringKeys.WalletNotConnected);
            return;
        }

        FloatingWindowManager.instance.OpenTokenPopup();
    }

    [ContextMenu("Dummy Login")]
    void DummyLogin()
    {
        OnLoginSuccess(BlockchainDataManager.instance.dummyAccount);
    }

    #region Instance
    static MetamaskLoginManager _instance;
    public static MetamaskLoginManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<MetamaskLoginManager>();
            return _instance;
        }
    }
    #endregion

    IEnumerator PostMetamaskID(string url)
    {
        WWWForm forms = new WWWForm();
        forms.AddField("metamaskId", walletData.walletAddress);
        UnityWebRequest www = UnityWebRequest.Post(url, forms);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
           // OnLastdataRecieved(false, null);
        }
    }

    IEnumerator CheckMetamaskID(string url)
    {
        WWWForm forms = new WWWForm();
        forms.AddField("metamaskId", walletData.walletAddress);
        UnityWebRequest www = UnityWebRequest.Post(url, forms);
        www.SetRequestHeader("Authorization", "Bearer " + UserDatabase.Instance.localUserData.token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:\n{1}:\n{2}", www.responseCode, www.downloadHandler.text, www.downloadHandler.text.Length.ToString()));

            JSONNode node = JSON.Parse(www.downloadHandler.text);

            if(node["success"] == "true"  && node["success"] == "false")
            {
                StartCoroutine(PostMetamaskID(Constants.BaseUrl + "/addmetamask"));
            }
            else
            {
                SetupMetamaskBtn();
                FloatingWindowManager.instance.OpenInfoPanel("Your metamask id is not matching with the registered metamask id or being used by another account.");
            }
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            // OnLastdataRecieved(false, null);
        }
    }

   
    
}
