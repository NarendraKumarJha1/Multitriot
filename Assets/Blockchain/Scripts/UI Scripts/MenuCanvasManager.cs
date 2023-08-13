using UnityEngine;

public class MenuCanvasManager : MonoBehaviour
{
    public GameObject uiManager;
    public GameObject mainmenu;
    public GameObject DailyClaimablesWindow;
    public TMPro.TMP_Text DayText;
    public GameObject DataBasePrefab = null;
    public GameObject img;
    private void Awake()
    {
        if (FindObjectOfType<UserDatabase>() == null)
        {
            GameObject db = Instantiate(DataBasePrefab);
            DontDestroyOnLoad(db);
        }
    }

    private void Start()
    {
        Init();
    }

    public void ResetPanels()
    {
        uiManager.gameObject.SetActive(false);
        mainmenu.gameObject.SetActive(false);
    }

    void Init()
    {
        ResetPanels();
        uiManager.gameObject.SetActive(true);
        Invoke("closeExtrapage",3);
        mainmenu.gameObject.SetActive(false);
    }

    private void closeExtrapage()
    {
         img.SetActive(false);
        //GSF_MainMenu.Instance.loadingPanel.SetActive(false);
    }

    public void OnLogin()
    {
        ResetPanels();
        uiManager.gameObject.SetActive(false);
        mainmenu.gameObject.SetActive(true);

        if(!string.IsNullOrEmpty(UserDatabase.Instance.localUserData.Dc.Days))
        {
            DailyClaimablesWindow.SetActive(true);
            DayText.text = "Day " + UserDatabase.Instance.localUserData.Dc.Days;
        }
        else
        {
            DailyClaimablesWindow.SetActive(false);
        }
       
    }

    public void OnLogout()
    {
        ResetPanels();
        uiManager.gameObject.SetActive(true);
        Invoke("closeExtrapage", 5);
        mainmenu.gameObject.SetActive(false);
        PhotonManager.IsGuest = false;
    }

    #region Instance
    static MenuCanvasManager _instance;
    public static MenuCanvasManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<MenuCanvasManager>();
            return _instance;
        }
    }
    #endregion
}