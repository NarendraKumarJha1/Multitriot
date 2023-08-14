using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class MMLapStats : MonoBehaviour
{
    public bool _isPlayer;

    //static instance
    public static MMLapStats instance;

    //Rank for leaderboard
    [SerializeField] public int _rankSetter;
    [SerializeField]
    public int _rank
    {
        get
        {
            return _rankSetter;
        }
        set { _rankSetter = value; }
    }

    [SerializeField] public string _racerName;
    [SerializeField] public string _vehicleName;
    [SerializeField] public string _lvl;

    private MMLeaderBoardManager _leaderBoardManager;


    [Header("Lap Time")]
    [SerializeField]
    private float _currTime;
    [SerializeField]
    private float _prevTime;
    [SerializeField]
    private float _bestTime = Mathf.Infinity;
    [SerializeField]
    private float _totalTime;
    [SerializeField]
    private float _timeOfReach;

    //Collider collision tag counter
    [Header("Lap Counter")]
    [SerializeField]
    private int _count = 0;
    [SerializeField]
    private int _Fcount = 0;
    [SerializeField]
    private int _PreFcount = 0;
    [SerializeField]
    private int _PostCount = 0;
    [SerializeField]
    private int _MidCount = 0;
    [SerializeField]
    private int _currentWaypointIndex = 0;

    [Header("String For Lap Stats")]
    [SerializeField] private string _currLap;
    [SerializeField] private string _bestLap;
    [SerializeField] private string _lastLap;
    [SerializeField] private string _lap;
    [SerializeField] private string _playerInGameRank;
    [SerializeField] private string _currentWaypointIndexstring;

    [Header("String For Lap Stats")]
    [SerializeField] private Text _currLapT;
    [SerializeField] private Text _bestLapT;
    [SerializeField] private Text _lastLapT;
    [SerializeField] private Text _lapT;
    [SerializeField] private Text _playerInGameRanktxt;

    [Header("LapCount")]
    [SerializeField]
    public int _lapCountLocal = 1;
    public int _targetLapCount;
    public int playersAhead = 0;
    public int playerBehind = 0;
    public int playerAlong = 0;

    bool _isFirstLap = false;
    public bool _finised = false;

    // Stores the lap stats of all racers
    public List<MMLapStats> _lapStatsList;
    public List<MMLapStats> sortedLapStatsList;
    public List<Transform> _waypoints;
    public List<int> _listOfAllCurrentWaypoints;
    public List<int> _listOfSubSetCurrentWaypoints;
    public List<int> _listOfAllCurrentLap;
    public Transform _currentWaypoint;
    public Dictionary<string, int> _rankLapList = new Dictionary<string, int>();
    public List<(int, int)> _rankings = new List<(int, int)>();

    private void Start()
    {
        _lapCountLocal = 1;
        instance = this;
        if (instance != null)
        {
            instance = this;
        }

        _currTime = Time.time;
        SetLap(_lapCountLocal);

        // Get all MMLapStats instances in the scene
        _lapStatsList = new List<MMLapStats>(FindObjectsOfType<MMLapStats>());
        Debug.Log("LapList Count" + _lapStatsList.Count);
        _waypoints = new List<Transform>(WaypointCircuit.instance.waypointList.items);

    }

    private void Update()
    {
        #region Calculate total Time
        _totalTime = Time.time - _currTime;
        int minutes = (int)(_totalTime / 60);
        float remainingTime = _totalTime % 60;
        int seconds = (int)remainingTime;
        int milliseconds = (int)((remainingTime - seconds) * 60);

        string _min = minutes.ToString();
        string _sec = seconds.ToString("00");
        string _miliSec = milliseconds.ToString("00");

        if (_isPlayer)
        {
            _currLapT.text = "TOTAL " + _min + " : " + _sec + " : " + _miliSec;
        }
        _currLap = _min + " : " + _sec + " : " + _miliSec;
        #endregion

        //Check for lap updation by counters
        CheckForLapUpdation();

        CheckException();
        _currentWaypoint = GetClosestWaypoint(transform.position);
        try
        {
            _currentWaypointIndexstring = _currentWaypoint.gameObject.name.Replace("Waypoint", string.Empty);
            _currentWaypointIndexstring = _currentWaypointIndexstring.Replace(" ", string.Empty);
        }
        catch (Exception e)
        {

        }
        bool success = int.TryParse(_currentWaypointIndexstring, out _currentWaypointIndex);
        
        
    }

    private void FixedUpdate()
    {
       CalculatePlayerPosition();
    }

    private void CalculatePlayerPosition()
    {
        sortedLapStatsList = _lapStatsList
        .OrderBy(_lapStats => _lapStats._lapCountLocal)
        .GroupBy(_lapStats => _lapStats._lapCountLocal)
        .SelectMany(group => group.OrderBy(_lapStats => _lapStats._currentWaypointIndex))
        .ToList();

        foreach (MMLapStats lapstat in sortedLapStatsList)
        {
            lapstat._playerInGameRank = (Mathf.Abs(sortedLapStatsList.IndexOf(lapstat) - sortedLapStatsList.Count)).ToString();
        }
        if (_isPlayer)
        {
            _playerInGameRanktxt.text = _playerInGameRank + "/" + _lapStatsList.Count;
        }
    }

    private void CheckException()
    {
        if (_count <= 1)
        {
            if (_Fcount > 0 || _PreFcount > 0 || _PostCount > 0 || _MidCount > 0)
            {
                _Fcount = 0;
                _PreFcount = 0;
                _PostCount = 0;
                _MidCount = 0;
            }
        }
    }

    private void CheckForLapUpdation()
    {
        if (_count >= 6 && _Fcount >= 1 && _PreFcount >= 1 && _PostCount >= 1 && _MidCount >= 1)
        {
            UpdatePrevLap();
            IncLap();
            StartCoroutine(ResetCounters());
        }

        if (_count >= _targetLapCount && _finised == false)
        {
            if (_isPlayer)
            {
                _rank = GetRank();
                // Update UI or do something with the player's rank
            }
        }
    }
    private int GetRank()
    {
        int rank = 1;

        foreach (var lapStats in _lapStatsList)
        {
            if (lapStats != this && lapStats._finised && lapStats._bestTime < _bestTime)
            {
                rank++;
            }
        }

        return rank;
    }
    IEnumerator ResetCounters()
    {
        _count = 0;
        yield return new WaitForSeconds(0.5f);
        _Fcount = 0;
        _PreFcount = 0;
        _PostCount = 0;
        _MidCount = 0;
    }

    public void ResetLapCounter()
    {
        _count = 0;
        _Fcount = 0;
        _PreFcount = 0;
        _PostCount = 0;
        _MidCount = 0;
    }

    public void RaceFinished()
    {
        if (_isPlayer)
        {
            _finised = true;
            LapManager.instance.RaceFinished();
        }
        Debug.LogWarning(" ## Race finished Executing ## ");
        GameObject _g = Instantiate(LapManager.instance._LeaderBoardInstance._playerStats, LapManager.instance._LeaderBoardInstance._spawnPoint.transform);
        GetComponent<Collider>().enabled = false;
        ShowPlayerDetails(_g);
        Debug.LogWarning(" ## Race finished Executed ## ");
        Invoke("DisableWithDelay", 2f);
    }
    private void DisableWithDelay()
    {
        this.gameObject.SetActive(false);
    }
    private void ShowPlayerDetails(GameObject _g)
    {
        MMPlayerStatsManager _mmPlayerStatsManager = _g.GetComponent<MMPlayerStatsManager>();
        _mmPlayerStatsManager._name.text = _racerName;
        _mmPlayerStatsManager._rank.text = _rank.ToString();
        _mmPlayerStatsManager._lvl.text = _lvl.ToString();
        _mmPlayerStatsManager._time.text = _currLap.ToString();
    }

    public void UpdatePrevLap()
    {
        if (_isFirstLap == false)
        {
            _prevTime = _totalTime - _timeOfReach;
            _bestTime = _totalTime - _timeOfReach;
            Debug.LogWarning(_prevTime);
            _timeOfReach = _totalTime;
            SetBestLap();
            _isFirstLap = true;
        }
        else
        {
            _prevTime = _totalTime - _timeOfReach;
            _timeOfReach = _totalTime;
        }

        string _min = Mathf.FloorToInt(_prevTime / 60).ToString();
        string _sec = Mathf.FloorToInt(_prevTime % 60).ToString("f0");
        string _miliSec = Mathf.FloorToInt((_prevTime % 1) * 100).ToString("f0");
        if (_isPlayer)
        {
            _lastLapT.text = "Last " + _min + " : " + _sec + " : " + _miliSec;
        }
        _lastLap = "Last " + _min + " : " + _sec + " : " + _miliSec;
        if (_prevTime < _bestTime)
        {
            _bestTime = _prevTime;
            SetBestLap();
        }
    }
    public void SetBestLap()
    {
        string _min = Mathf.FloorToInt(_bestTime / 60).ToString();
        string _sec = Mathf.FloorToInt(_bestTime % 60).ToString("f0");
        string _miliSec = Mathf.FloorToInt((_bestTime % 1) * 100).ToString("f0");
        if (_isPlayer)
        {
            _bestLapT.text = "Best " + _min + " : " + _sec + " : " + _miliSec;
        }
        _bestLap = "Best " + _min + " : " + _sec + " : " + _miliSec;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            _count++;
        }
        if (other.gameObject.CompareTag("FinalCheckPoint"))
        {
            _Fcount++;

        }
        if (other.gameObject.CompareTag("PreFinalCheckPoint"))
        {
            _PreFcount++;
        }
        if (other.gameObject.CompareTag("PostFinalCheckPoint"))
        {
            _PostCount++;
        }
        if (other.gameObject.CompareTag("MidFinalCheckPoint"))
        {
            _MidCount++;
        }
    }

    public void ReduceCount()
    {
        _count = 1;
        if (_lapCountLocal < 1)
        {
            _lapCountLocal--;
            SetLap(_lapCountLocal);
        }
    }
    public void SetLap(int _lapcount)
    {
        if (_isPlayer)
        {
            _lapT.text = _lapcount.ToString() + "/" + _targetLapCount;
        }
    }
    public void IncLap()
    {
        _lapCountLocal++;
        SetLap(_lapCountLocal);
    }

    #region Runtime Position
    public Transform GetClosestWaypoint(Vector3 position)
    {
        _currentWaypointIndex = 0;
        Transform closestWaypoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform waypoint in _waypoints)
        {
            float distance = Vector3.Distance(position, waypoint.position);
            if (distance < closestDistance)
            {
                closestWaypoint = waypoint;
                closestDistance = distance;
                _currentWaypointIndex++;
            }
        }

        return closestWaypoint;
    }
    #endregion
}
