using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField username, password;
    public Button rememberToggel;
    public Button forgetPassword, loginBtn, signupBtn,freeModeBtn;

    private UiManager uiManager;
    private bool rememberMe = false;
    private bool RememberMe
    {
        get => rememberMe;
        set
        {
            rememberMe = value;
            BlockchainDataManager.rememberme = rememberMe;
            rememberToggel.transform.GetChild(0).gameObject.SetActive(rememberMe);
        }
    }

    private void OnEnable()
    {
        Init();
        ApiManager.instance.loginApi.OnLastdataRecieved += OnLoginDataRecieved;
    }
    private void OnDisable()
    {
        ApiManager.instance.loginApi.OnLastdataRecieved -= OnLoginDataRecieved;
    }

    EventSystem system;
    void Start()
    {
        system = EventSystem.current;

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
            DbLoginCall(username.text, password.text);
    }


    /// <summary>
    /// login initialising thing is going on here 
    /// adding the method to login button and signup and guest user
    /// </summary>
    void Init()
    {
        uiManager = UiManager.Instance;

#if UNITY_EDITOR
        //username.text = "lucifer";
        //password.text = "123456";
/*        username.text = "sreeharry";
        password.text = "123123";*/
#else
        /*username.text = "ananta2";
        password.text = "123456";*/
        username.text = string.Empty;
        password.text = string.Empty;
#endif

        string authData = PlayerPrefs.GetString(GenericStringKeys.loginAuth, string.Empty);

        if(!string.IsNullOrEmpty( authData))
        {
            string[] auth = authData.Split('|');
            username.text = auth[0];
            password.text = auth[1];
        }

        loginBtn.interactable = true;

/*        if(PhotonManager.IsGuest)
        {
            OnGuestUser();
            return;
        }*/

        if (string.IsNullOrEmpty(authData))
        {
            //$$$ Debug.LogError("No auth Data Found");
            RememberMe = false;

            loginBtn.OnClick(() => DbLoginCall(username.text, password.text));
            freeModeBtn.OnClick(() => OnGuestUser());
            signupBtn.OnClick(delegate { uiManager.SwitchPannel(false); });
            rememberToggel.OnClick(() => RememberMe = !RememberMe);
        }
        else
        {
            string[] auth = authData.Split('|');
            //if (auth.Length != 3 || string.IsNullOrEmpty(auth[0]) || string.IsNullOrEmpty(auth[1]))
            //{
            //    //if (PlayerPrefs.HasKey(GenericStringKeys.loginAuth))
            //    //    PlayerPrefs.DeleteKey(GenericStringKeys.loginAuth);
            //    PlayerPrefs.Save();
            //    Init();
            //}
            RememberMe = bool.Parse(auth[2]);
            DbLoginCall(auth[0], auth[1]);
        }
    }

    void DbLoginCall(string username, string password)
    {
        Debug.Log("name: " + username);
        Debug.Log("pass: " + password);

        if (string.IsNullOrEmpty(username))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyNickname, Color.red, 2f);
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyPassword, Color.red, 2f);
            return;
        }

        GSF_MainMenu.Instance.loadingPanel.SetActive(true);
        loginBtn.interactable = false;
        ApiManager.instance.loginApi.Download(username, password,false);
    }

    void OnGuestUser()
    {
        string _username = PlayerPrefs.GetString("username");
        PhotonManager.IsGuest = true;
        ApiManager.instance.loginApi.Download(_username, _username, true);
    }

    private void OnLoginDataRecieved(bool status, LoginApiData _loginData)
    {
        if (status)
        {
            if (_loginData.status.status >= 400)
                Helper.ShowMessage(uiManager.logText, _loginData.status.msg, Color.red, 2f);
            else
            {
                UserDatabase.Instance.localUserData = _loginData.loginData;
                ApiManager.instance.referralDetailsApi.Download();

                UserDatabase.Instance.IsLogin = true;

                string userdata = string.Format("{0}|{1}|{2}", username.text, password.text, rememberMe);
                PlayerPrefs.SetString(GenericStringKeys.loginAuth, userdata);
            }
        }
        else
            Helper.ShowMessage(uiManager.logText, "Check your username or password.", Color.red, 2f);

        GSF_MainMenu.Instance.loadingPanel.SetActive(false);
        loginBtn.interactable = true;
    }
}