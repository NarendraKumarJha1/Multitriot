using DG.Tweening;
using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMystic : MonoBehaviour
{
    public static GameManagerMystic _instance; 

    [Header("UI")]
    [SerializeField]
    GameObject _spellDameDescription;
    [SerializeField]
    GameObject _shortcutsPanel;
    [SerializeField] 
    GameObject _InGameDebugger;
    [SerializeField] 
    TextMeshProUGUI _prompt;
    [SerializeField]
    Button _back;

    bool _ingameDebugToggle = false;
    public bool _enable = true;
    public bool _debug = false;

    [Header("Player Cars")]
    [SerializeField]
    GameObject[] _Cars;
    [SerializeField]
    public GameObject _playerCar;

    [Header("Bot Cars")]
    [SerializeField]
    List<GameObject> _botCars;
    [SerializeField]
    public List<GameObject> _botCarsList;
    [SerializeField]
    GameObject[] _spawnPoints;

    [Header("Panel")]
    [SerializeField]
    GameObject _loadingPanel;
    [SerializeField]


    [Header("Bools")]
    private bool _spellDescriptionbeingAnimated = false;
    private bool _shortcutsBeingAnimated = false;
    public string modelPath; // Path to the model file on disk
    public GameObject targetObject; // Object to assign the loaded model to

    //Selected Player from garage

    private int _selectedPlayerIndex = -1;
    private int tweenIdForDescription = -2;
    private int tweenIdForShortcuts = -1;
    private void Awake()
    {
        _selectedPlayerIndex = PlayerPrefs.GetInt("CarIndex");
        Debug.Log("Player Selected :-"+ _selectedPlayerIndex);
        AddPlayerCar();
    }

    private void AddPlayerCar()
    {
        Debug.Log("Selected Player" + _selectedPlayerIndex);
        _botCars.Add(_Cars[_selectedPlayerIndex]);
    }

    void Start()
    {
        _instance = this;
        _InGameDebugger.SetActive(false);
        StartCoroutine(InstantiateCars());
        StartCoroutine(StartInitialScreen());
    }

    IEnumerator InstantiateCars()
    {
        int i = _botCars.Count;
        yield return new WaitForSeconds(2f);
        for(int index = 0; index<i;index++ )
        {
            GameObject _g =  Instantiate(_botCars[index], _spawnPoints[index].transform.position, _spawnPoints[index].transform.rotation);
            
            if (_g.CompareTag("Player"))
            {
                _playerCar = _g;
            }

            _botCarsList.Add(_g);
            try
            {
                if (_g.GetComponent<BotManager>())
                {
                    _g.GetComponent<BotManager>()._botIndex = index;
                }

            }catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            //Turn off the control
            _g.GetComponent<RCC_CarControllerV3>().canControl = false;
            _g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            _g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        }
    }

    IEnumerator StartInitialScreen()
    {
        _loadingPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitForSeconds(2f);
        _loadingPanel.SetActive(false);
        CameraManager.instance.SwitchToNextCountdownCamera();
        yield return new WaitForSeconds(1f);
        CameraManager.instance.SwitchToNextCountdownCamera();
        yield return new WaitForSeconds(1f);
        CameraManager.instance.SwitchToNextCountdownCamera();
        yield return new WaitForSeconds(1f);
        CameraManager.instance.SwitchToNextCountdownCamera();
        yield return new WaitForSeconds(1f);
        CameraManager.instance.SwitchToNextCountdownCamera();
        yield return new WaitForSeconds(1f);
        CameraManager.instance.ActivatePlayerCamera();
        foreach (GameObject _rc in _botCarsList)
        {
            Debug.Log("Turning on car control");

            _rc.GetComponent<RCC_CarControllerV3>().canControl = !_debug;
            _rc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.F1))
        {
            _enable = !_enable;
            _prompt.gameObject.SetActive(true);
            _prompt.text = _enable == true ? "Devloper mode Activated" : "Devloper mode Deactivated";
            StartCoroutine(DisablePromt());
        }

        if (_enable)
            CheckForInGameDebuggerToggle();

        StartCoroutine(CheckForDesPanel());
        StartCoroutine(CheckForShortCutsPanel());
    }

    IEnumerator CheckForShortCutsPanel()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_shortcutsBeingAnimated)
            {
                yield return null;
            }
            _shortcutsPanel.SetActive(true);
            tweenIdForShortcuts = LeanTween.moveLocalY(_shortcutsPanel.gameObject, -400f, 0.8f).setEaseOutBack().setOnComplete(()=> _shortcutsBeingAnimated = true).id;
        }
        else if (Input.GetKeyUp(KeyCode.F1))
        {
            if (!_shortcutsBeingAnimated)
            {
                yield return null;
            }

            tweenIdForShortcuts = LeanTween.moveLocalY(_shortcutsPanel.gameObject, -900f, 0.5f).setEaseInSine().setOnComplete(() => _shortcutsBeingAnimated = false).id;
            yield return new WaitForSeconds(0.5f);
            _shortcutsPanel.SetActive(false);
        }
    }
    IEnumerator CheckForDesPanel()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (_spellDescriptionbeingAnimated)
            {
                yield return null;
            }
            _spellDameDescription.SetActive(true);
            tweenIdForDescription = LeanTween.scale(_spellDameDescription.gameObject,new Vector3(1, 1, 1), 1f).setEaseOutBack().setOnComplete(()=>_spellDescriptionbeingAnimated = true).id;
        }else if (Input.GetKeyUp(KeyCode.F5))
        {
            if (!_spellDescriptionbeingAnimated)
            {
                yield return null;
            }
            tweenIdForDescription = LeanTween.scale(_spellDameDescription.gameObject, new Vector3(0, 0, 0), 0.5f).setEaseInSine().setOnComplete(() => _spellDescriptionbeingAnimated = false).id;
            yield return new WaitForSeconds(0.5f);
            _spellDameDescription.SetActive(false);
        }
    }
    public void Restart()
    {
        _spellDameDescription.transform.localScale = Vector3.zero;
    }
    IEnumerator DisablePromt()
    {
        yield return new WaitForSeconds(1f);
        _prompt.gameObject.SetActive(false);
    }

    private void CheckForInGameDebuggerToggle()
    {
        if(Input.GetKeyDown(KeyCode.K) && Input.GetKeyDown(KeyCode.J) && Input.GetKeyDown(KeyCode.H))
        {
            _ingameDebugToggle = !_ingameDebugToggle;
            _InGameDebugger.SetActive(_ingameDebugToggle);
        }
    }


/*    IEnumerator LoadModelAsync()
    {
        var requests = new List<AsyncOperation>();
        foreach (GameObject model in _botCars)
        {
            var request = Resources.LoadAsync<GameObject>(model.name);
            requests.Add(request);
        }

        // Wait for all models to be loaded
        foreach (var request in requests)
        {
            yield return request;
        }

        // Once all models are loaded, instantiate them at the spawn locations
        for (int i = 0; i < _botCars.Length; i++)
        {
            // Get the next spawn location
            GameObject spawnLocation = _spawnPoints[i % _spawnPoints.Length];

            // Instantiate the model at the spawn location
            GameObject loadedModel = Instantiate(requests[i % requests.Count].result, spawnLocation.transform.position, Quaternion.identity);

            // Parent the loaded model to the target object
            loadedModel.transform.SetParent(targetObject.transform, false);
        }
    }*/

    public void Back()
    {
        SceneManager.LoadScene(1);
    }
}
