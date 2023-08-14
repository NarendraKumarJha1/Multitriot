using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiManager : MonoBehaviour
{
    public LoginManager loginManager;
    public SignUpManager signUpManager;
    public ForgetPassword PasswordManager;
   
    public Text logText;

    private void OnEnable()
    {
        SwitchPannel(true);
        logText.gameObject.SetActive(false);
    }

    public void SwitchPannel(bool loginActive)
    {
        loginManager.gameObject.SetActive(loginActive);
        signUpManager.gameObject.SetActive(!loginActive);
    }

    #region Instance
    static UiManager _instance;
    public static UiManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UiManager>();
            return _instance;
        }
    }
    #endregion
}