using UnityEngine;

public enum ApiEnvironment { Dev, Live };

public class ApiManager : MonoBehaviour
{
    public bool showDebugs = true;

    public LoginApi loginApi;
    public SignUpApi signUpApi;
    public ReferralDetailsApi referralDetailsApi;
    public AddTokenApi addTokenApi;
    public RedeemTokenApi redeemTokenApi;
    public AddNftApi addNftApi;
    public GameRegister gameRegister;
    public WinRegister winRegister;
    public DummyTokens dumTokens;
    public ProfileEdit editProfile;
    public Tournament tournamentData;
/*    public InventoryApi inventory;
    public PurchaseItem purchaseItem;*/
    #region Instance
    public static ApiManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }
    #endregion
}