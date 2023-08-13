using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingWindowManager : MonoBehaviour
{
    public static FloatingWindowManager instance;

    [Header("Transiction Pop-up")]
    public GameObject transictionPopup;
    public Text text_tx;
    public Button addTokenBtn, redeemTokenBtn, closeBtn_tx;

    [Header("Add Token")]
    public GameObject addTokenPopup;
    public Text text_addTx, logText;
    public InputField tokenAmount;
    public Button addTBtn, closeBtn_addTx;

    [Header("Redeem Token")]
    public GameObject redeemTokenPopup;
    public Text text_RedeemTx, RedeemlogText;
    public InputField RedeemAmount;
    public Button redeemTBtn, closeBtn_redeemTx;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public Text infoMessage;
    public Button infoCancelBtn;

   

   

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        CloseAllWindows();
    }
    void CloseAllWindows()
    {
        FadeOff();
        transictionPopup.SetActive(false);
        addTokenPopup.SetActive(false);
        redeemTokenPopup.SetActive(false);
        infoPanel.SetActive(false);
        
    }

    public void OpenTokenPopup()
    {
        CloseAllWindows();
        FadeOn();
        transictionPopup.SetActive(true);

        text_tx.text = GenericStringKeys.AddRedeemShoe;

        addTokenBtn.OnClick(OpenAddToken);
        redeemTokenBtn.OnClick(OpenRedeemPanel);

        closeBtn_tx.OnClick(() =>
        {
            FadeOff();
        });
    }

    public void OpenAddToken()
    {
        CloseAllWindows();
        FadeOn();
        addTokenPopup.SetActive(true);

        logText.gameObject.SetActive(false);
        text_addTx.text = GenericStringKeys.ShoeAmount;

        tokenAmount.text = string.Empty;
        addTBtn.OnClick(CheckInput);

        closeBtn_addTx.OnClick(() =>
        {
            FadeOff();
        });

        void CheckInput()
        {
            if (string.IsNullOrEmpty(tokenAmount.text))
            {
                logText.text = GenericStringKeys.InvalidShoeAmount;
                StartCoroutine(ShowTimedLog());
                return;
            }

            int amt = int.Parse(tokenAmount.text);
            double wAmt = double.Parse(BlockchainDataManager.instance.WalletData.walletBalance);
            if (amt <= 0 || amt > wAmt)
            {
                logText.text = GenericStringKeys.InvalidShoeAmount;
                StartCoroutine(ShowTimedLog());
                return;
            }

            BlockchainDataManager.instance.transferShoeToken.TxStatus += TransferShoeToken_TxStatus;
            BlockchainDataManager.instance.transferShoeToken.TransferShoe(amt);
            CloseAllWindows();
            OpenInfoPanel();
        }

        IEnumerator ShowTimedLog()
        {
            logText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            logText.gameObject.SetActive(false);
        }
    }

    private void TransferShoeToken_TxStatus(bool _status, string txHash, int count)
    {
        BlockchainDataManager.instance.transferShoeToken.TxStatus -= TransferShoeToken_TxStatus;

        addTokenCount = count;
        ApiManager.instance.addTokenApi.OnLastdataRecieved += AddTokenStatus;
        ApiManager.instance.addTokenApi.Download(txHash, count.ToString());
    }
    int addTokenCount = 0;
    void AddTokenStatus(bool status, ApiStatus _status)
    {
        ApiManager.instance.addTokenApi.OnLastdataRecieved -= AddTokenStatus;
        if (status)
        {
            if (_status.status > 400)
                OpenInfoPanel(GenericStringKeys.TxFailed);
            else
            {
                double _wAmt = double.Parse(BlockchainDataManager.instance.WalletData.walletBalance);
                double _addAmt = (double)addTokenCount * 1E18;

                double newAmt = _wAmt - _addAmt;

                double newIngameAmt = double.Parse(UserDatabase.Instance.localUserData.userWalletData.ingameToken) + (double)addTokenCount;

                WalletData wd = new WalletData()
                {
                    isLogin = BlockchainDataManager.instance.WalletData.isLogin,
                    walletAddress = BlockchainDataManager.instance.WalletData.walletAddress,
                    walletBalance = string.Format("{0:0}", newAmt)
                };

                BlockchainDataManager.instance.WalletData = wd;

                UserDatabase.Instance.localUserData.userWalletData.ingameToken = newIngameAmt.ToString();
                GSF_MainMenu.Instance.UpdateProfile();
                OpenInfoPanel(GenericStringKeys.TxSuccess);
            }
        }
        else
            OpenInfoPanel(GenericStringKeys.SomethingWentWrong);
    }

    public void OpenRedeemPanel()
    {
        CloseAllWindows();
        FadeOn();
        redeemTokenPopup.SetActive(true);
        RedeemAmount.text = string.Empty;
        double _ingameT = double.Parse(UserDatabase.Instance.localUserData.userWalletData.ingameToken);
        bool canRedeem = false;
        canRedeem = _ingameT > GlobalVariables.minRedeemValue;

        text_RedeemTx.text = canRedeem ? GenericStringKeys.RedeemShoe : string.Format(GenericStringKeys.RedeemMin, GlobalVariables.minRedeemValue);

        redeemTBtn.interactable = canRedeem;

        redeemTBtn.OnClick(Redeem);

        void Redeem()
        {
            if (string.IsNullOrEmpty(RedeemAmount.text))
            {
                RedeemlogText.text = GenericStringKeys.InvalidShoeAmount;
                StartCoroutine(ShowTimedLog());
                return;
            }

            int amt = int.Parse(RedeemAmount.text);
            double wAmt = double.Parse(UserDatabase.Instance.localUserData.userWalletData.ingameToken);
            if (amt <= 0 || amt > wAmt)
            {
                RedeemlogText.text = GenericStringKeys.InvalidShoeAmount;
                StartCoroutine(ShowTimedLog());
                return;
            }

            OpenInfoPanel();
            ApiManager.instance.redeemTokenApi.OnLastdataRecieved += RedeemStatus;
            ApiManager.instance.redeemTokenApi.Download(amt);

            IEnumerator ShowTimedLog()
            {
                RedeemlogText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2);
                RedeemlogText.gameObject.SetActive(false);
            }
        }

        closeBtn_redeemTx.OnClick(() =>
        {
            FadeOff();
        });
    }

    void RedeemStatus(bool status, ApiStatus _status)
    {
        ApiManager.instance.redeemTokenApi.OnLastdataRecieved -= RedeemStatus;
        if (status)
        {
            if (_status.status > 400)
                OpenInfoPanel(_status.msg);
            else
            {
               
                //double _wAmt = double.Parse(BlockchainDataManager.instance.WalletData.walletBalance);
                //double _addAmt = double.Parse(UserDatabase.Instance.localUserData.userWalletData.ingameToken) * 1E18;

                //double newAmt = _wAmt + _addAmt;

                //WalletData wd = new WalletData()
                //{
                //    isLogin = BlockchainDataManager.instance.WalletData.isLogin,
                //    walletAddress = BlockchainDataManager.instance.WalletData.walletAddress,
                //    walletBalance = string.Format("{0:0}", newAmt)
                //};

                //BlockchainDataManager.instance.WalletData = wd;

                //UserDatabase.Instance.localUserData.userWalletData.ingameToken = "0";
                //GSF_MainMenu.Instance.UpdateProfile();
                OpenInfoPanel("Redeem Succesful!");
            }
        }
        else
            OpenInfoPanel(GenericStringKeys.SomethingWentWrong);
    }

    public void OpenInfoPanel(string message = "Loading...")
    {
        CloseAllWindows();
        FadeOn();
        infoPanel.SetActive(true);

        infoMessage.text = message;

        if (message.Equals("Loading..."))
        {
            infoCancelBtn.gameObject.SetActive(false);
        }
        else
        {
            infoCancelBtn.gameObject.SetActive(true);
            infoCancelBtn.OnClick(() =>
            {
                FadeOff();
            });
        }
    }


    

    
    void FadeOn()
    {
        CanvasGroup cg = this.GetComponent<CanvasGroup>();

        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void FadeOff()
    {
        CanvasGroup cg = this.GetComponent<CanvasGroup>();

        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
