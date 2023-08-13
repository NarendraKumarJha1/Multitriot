using UnityEngine;

public class UserDatabase : MonoBehaviour
{
    public LoginData localUserData = new LoginData();

    bool isLogin = false;
    bool IsEmpty = false;
    public bool IsLogin
    {
        get => isLogin;
        set
        {
            isLogin = value;
            OnLoginStatusChanged(isLogin, IsEmpty);
        }
    }

    public bool IsUsernameEmpty
    {
        get => IsEmpty;
        set
        {
            IsEmpty = value;
        }
    }

    void OnLoginStatusChanged(bool status, bool Isuempty)
    {
        if (status && Isuempty)
        {
            MenuCanvasManager.Instance.OnLogin();
            GSF_MainMenu.Instance.UpdateProfile();
        }
        else
        {
            if (PlayerPrefs.HasKey(GenericStringKeys.loginAuth))
            {
                PlayerPrefs.DeleteKey(GenericStringKeys.loginAuth);
                Debug.LogError("Deleting Auth Data ");
            }

            //MetamaskLoginManager.Instance.ResetWalletData();

            MenuCanvasManager.Instance.OnLogout();
        }
    }

    #region Instance
    static UserDatabase _instance;
    public static UserDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UserDatabase>();
            }
            return _instance;
        }
    }
    #endregion
}