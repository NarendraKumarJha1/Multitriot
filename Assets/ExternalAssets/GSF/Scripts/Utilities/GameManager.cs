using UnityEngine;

public class GameManager
{
    private static GameManager instance;

    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameManager();
            return instance;
        }
    }

    public bool Initialized = false;
    public static int totalLevelsCount = 30;
    public int CurrentLevel = 0;
    public int CurrentPlayer = 0;
    public string GameStatus;
    public int Objectives;
    public bool SessionAd = false;

#if UNITY_EDITOR
    public bool EditorSession = true;
#endif

    public int totalFlips = 0;

    public void TaskComplete()
    {
        if (Objectives > 0)
            Objectives--;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GSF_GameController>().OnLevelCheck(0);
    }

    public void GameLoose(int reasonIndex = 0)
    {
        if (GameStatus != "Loose")
        {
            GameStatus = "Loose";
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GSF_GameController>().OnLevelCheck(reasonIndex);
        }
        else
        {
            Debug.LogWarning("Game loose being called multiple times !");
        }
    }

    public void PauseTimer()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GSF_GameController>().TimerPaused = true;
    }

    public void ResumeTimer()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GSF_GameController>().TimerPaused = false;
    }

    public void UpdateInventory()
    {
        //Give items to player here
    }
}