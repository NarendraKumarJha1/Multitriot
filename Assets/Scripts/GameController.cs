using RGSK;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum LevelToLoad
{
    None,
    Futuristic,
    Shoefy,
    Lava
};

public enum GameState { None, Waiting, Start, End }

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    public GameState gameState = GameState.None;
    public Material futuristicSkidmarkmat;
    public Material lavaSkidmarkmat;
    public Material neonSkidmarkmat;
    public Material defaultRCCSkidmarkmat;
    public GameObject startPoint = null;
    public Camera rccMainCamera = null;
    public GameObject rccCanvasObject = null;
    public GameObject rskCanvasObject = null;
    public GameObject rgskParent = null, rccParent = null;
    public GameObject[] hidegameObjects = null;
    public GameObject PauseBtn = null;
    public LevelToLoad levelToLoad = LevelToLoad.None;
    public int totalLaps = 2;
    public Transform[] mainSpawnPoints;
    public delegate void UpdateMainCamera(Camera cameraToUpdate);
    public static event UpdateMainCamera updateMainCamera;
    public Camera gamePlayCamera = null;
    public RaceManager raceManager = null;
    public CheckpointContainer checkpointContainer = null;
    public WaypointCircuit waypointCircuit = null;
    public RCC_Camera rCC_Camera = null;
    public RCC_CarControllerV3[] allPlayersPrefabs = null;
    public int playerIndex = 0;
    public bool isStartTextChanged = false;

    int markerSize = 0;
    public int GetMarkerSize() => markerSize;

    [SerializeField] GameObject currentPlayer = null;
    [SerializeField] PowerUpsHandler powerUpsHandler = null;
    public GameObject CurrentPlayer
    {
        get => currentPlayer;
        set
        {
            currentPlayer = value;
            powerUpsHandler = currentPlayer.GetComponent<PowerUpsHandler>();
        }
    }
    public void StartGamePlay()
    {
        updateMainCamera?.Invoke(gamePlayCamera);
    }
    public Camera miniMapCamera = null;

    [Serializable]
    public struct MyData
    {
        public string name;
        public LevelToLoad levelType;
        public Material skyboxMaterial;
        public WaypointCircuit wayPointPath;
        public CheckpointContainer checkpointContainer;
        public Transform[] spawnPoints;
        public Transform miniMapTransform;
        public int miniMapSize;
        public int markerSize;
    }
    public List<MyData> myDatas;

    public WaypointCircuit currentWayPointPath;

    [SerializeField] string environmentPath = "";

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (Constants.isFromMainMenu)
        {
            levelToLoad = (LevelToLoad)Constants.selectedLevel + 1;
            environmentPath = levelToLoad.ToString();

            Debug.Log("levelToLoad: " + levelToLoad);
            Debug.Log("environmentPath: " + environmentPath);

            totalLaps = Constants.modeId == 0 ? 1 : 2;

            if (GameManager.Instance != null)
                playerIndex = GameManager.Instance.CurrentPlayer;

            //  AudioListener.volume = PlayerPrefs.GetFloat("MASTER_VOLUME"); //$ree
        }

        raceManager.totalLaps = totalLaps;
        raceManager.selectedCarName = allPlayersPrefabs[playerIndex].gameObject.name;

        EnableControllers(false);

        for (int i = 0; i < myDatas.Count; i++)
        {
            if (!myDatas[i].levelType.Equals(levelToLoad)) continue;

            RenderSettings.skybox = myDatas[i].skyboxMaterial;
            myDatas[i].wayPointPath.gameObject.SetActive(true);

            miniMapCamera.transform.SetPositionAndRotation(myDatas[i].miniMapTransform.position, myDatas[i].miniMapTransform.rotation);

            miniMapCamera.orthographicSize = myDatas[i].miniMapSize;
            markerSize = myDatas[i].markerSize;

            for (int j = 0; j < mainSpawnPoints.Length; j++)
                mainSpawnPoints[j].SetPositionAndRotation(myDatas[i].spawnPoints[j].position, myDatas[i].spawnPoints[j].rotation);

            currentWayPointPath = myDatas[i].wayPointPath;
            currentWayPointPath.gameObject.SetActive(true);
            raceManager.pathContainer = currentWayPointPath.transform;
            raceManager.checkpointContainer = myDatas[i].checkpointContainer.transform;
            myDatas[i].checkpointContainer.transform.gameObject.SetActive(true);
            break;
        }

        loadLevelCoroutine = StartCoroutine(LoadLevelAsync(environmentPath));
    }

    void OnEnable()
    {
        SetRccCameraStatus(false);
    }

    private void Start()
    {
        gameState = GameState.None;

        Material _mat = levelToLoad switch
        {
            LevelToLoad.Futuristic => futuristicSkidmarkmat,
            LevelToLoad.Lava => lavaSkidmarkmat,
            LevelToLoad.Shoefy => neonSkidmarkmat,
            _ => defaultRCCSkidmarkmat,
        };

        RCC_Settings.Instance.skidmarksManager.GetComponent<MeshRenderer>().material = _mat;
    }

    void EnableControllers(bool isEnable)
    {
        rccParent.SetActive(isEnable);
        rgskParent.SetActive(isEnable);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiController.pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SetRccCameraStatus(bool status)
    {
        rccMainCamera.enabled = status;
        foreach (var item in hidegameObjects)
            item.SetActive(status);
    }

    Coroutine loadLevelCoroutine;
    private IEnumerator LoadLevelAsync(string path)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);

        //asyncOperation.allowSceneActivation = false;
        yield return new WaitUntil(() => asyncOperation.isDone);

        rgskParent.SetActive(true);
        rccParent.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        if (asyncOperation.progress >= 1.0f)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(path));

        //asyncOperation.allowSceneActivation = true;
        startPoint = GameObject.FindGameObjectWithTag("StartPoint");

        if (loadLevelCoroutine != null)
            StopCoroutine(loadLevelCoroutine);
    }

    public UiController uiController = null;
    public void SetMissileUi(int missileCount, bool isCarState)
    {
        uiController.SetMissileUi(missileCount, isCarState);
    }
    public void SetConversionSprite(bool isCar, int amount)
    {
        uiController.SetConversionSprite(isCar, amount);
    }

    public Animator conversionRobot = null;

    public GameObject[] allControllerButtons = null;
    public void SetControllerButtons()
    {
        //Debug.Log("window value is : " + Application.platform);
        for (int i = 0; i < allControllerButtons.Length; i++)
            allControllerButtons[i].SetActive(!Constants.IsWindowEditor());
    }

    public void GivePower(PowerUpsHandler.PowerUpType pickUpType, float nitroAmountToFill)
    {

        if (powerUpsHandler && !powerUpsHandler.isAI)
        {
            Debug.Log("PowerUPs  are doing fine");
            if (pickUpType == PowerUpsHandler.PowerUpType.Boost)
            {
                if (NitroManager.Instance)
                    NitroManager.Instance.AddNitroAmount(nitroAmountToFill);
            }
            powerUpsHandler.AddPowerUp(pickUpType);
        }
    }

    public void ChangeStartPointText()
    {
        Invoke(nameof(ChangeStartPointTextDelay), 5f);
    }
    void ChangeStartPointTextDelay()
    {
        startPoint.transform.GetChild(1).gameObject.SetActive(false);
        startPoint.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void HideStartPointText()
    {
        Invoke(nameof(ChangeStartPointTextDelay2), 5f);
    }
    void ChangeStartPointTextDelay2()
    {
        startPoint.transform.GetChild(1).gameObject.SetActive(false);
    }
}
