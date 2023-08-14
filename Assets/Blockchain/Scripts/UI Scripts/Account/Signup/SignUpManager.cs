using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SignUpManager : MonoBehaviour
{
    public InputField[] inputFields = new InputField[5];
    public Button signupBtn, backBtn;

    private UiManager uiManager;

    private void OnEnable()
    {
        Init();
        ApiManager.instance.signUpApi.OnLastdataRecieved += OnSignupDataRecieved;
    }
    private void OnDisable()
    {
        ApiManager.instance.signUpApi.OnLastdataRecieved -= OnSignupDataRecieved;
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
            DbSignUpCall(inputFields[0].text, inputFields[1].text, inputFields[2].text, inputFields[3].text, inputFields[4].text);
        if (Input.GetKeyDown(KeyCode.Escape))
            uiManager.SwitchPannel(true);

    }

    void Init()
    {
        uiManager = UiManager.Instance;
        ResetFields();
        backBtn.OnClick(delegate { uiManager.SwitchPannel(true); });
        signupBtn.OnClick(() => DbSignUpCall(inputFields[0].text, inputFields[1].text, inputFields[2].text, inputFields[3].text, inputFields[4].text));
    }

    private void ResetFields()
    {
        foreach (InputField ip in inputFields) ip.text = string.Empty;
    }


    void DbSignUpCall(string username, string nickname, string email, string password, string referalcode)
    {
        //if (string.IsNullOrEmpty(username))
        //{
        //    Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyUsername, Color.red, 2f);
        //    return;
        //}
        if (string.IsNullOrEmpty(nickname))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyNickname, Color.red, 2f);
            return;
        }
        if (username.Equals(nickname))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.SameUserNick, Color.red, 2f);
            return;
        }
        if (string.IsNullOrEmpty(email))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyEmail, Color.red, 2f);
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.EmptyPassword, Color.red, 2f);
            return;
        }
        if (password.Length < 6)
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.PasswordLen, Color.red, 2f);
            return;
        }
        if (referalcode.Equals("000000"))
        {
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.InvalidReferral, Color.red, 2f);
            return;
        }

        GSF_MainMenu.Instance.loadingPanel.SetActive(true);
        ApiManager.instance.signUpApi.Download(username, nickname, email, password, referalcode);
    }

    private void OnSignupDataRecieved(bool status, ApiStatus _signupData)
    {
        if (ApiManager.instance.showDebugs)
            Debug.Log("Signup Data Status: " + status + " : "+ _signupData.status);
        if (status)
        {
            if (_signupData.status > 400)
                Helper.ShowMessage(uiManager.logText, _signupData.msg, Color.red, 2f);
            else //if (_signupData.status == 103)
            {
                //Debug.Log("Signup Data Status: " + status + " : " + _signupData.status);
                Helper.ShowMessage(uiManager.logText, "User registration successful", Color.white, 2f);
                uiManager.SwitchPannel(true);


            }


        }
        else
            Helper.ShowMessage(uiManager.logText, GenericStringKeys.SomethingWentWrong, Color.white, 2f);

        GSF_MainMenu.Instance.loadingPanel.SetActive(false);
    }
}