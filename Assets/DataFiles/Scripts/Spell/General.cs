using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class General : MonoBehaviour
{
    [Tooltip("Description")]
    public string _description = "This script manages spell workflow";

    [Header("Fireball Objects")]
    [SerializeField] GameObject _spawnPoint;
    [SerializeField] GameObject []_fireBall;
    [SerializeField] Sprite []_fireBallSprite;
    [SerializeField] Color[] _fireBallColor;
    [SerializeField] Image _spell;
    [SerializeField] Button _spellButton;
    [SerializeField] Transform _targetSpawnPoint;
    [SerializeField] GameObject _fireballTarget;

    [Header("Target Direction Transform")]
    [SerializeField] Transform _target;

    public static General _instance;
    private Rigidbody _rb;
    private GameObject _g;
    bool isThere = false;
    bool _canShoot = false;
    bool _shouldStartTimer = false;
    bool _shouldUpdate = true;
    bool _generated = false;
    bool _generating = false;
    bool _shouldUpdatePos = false;
    public GameObject _fireballRef;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI _timerText;
    public float timeRemaining = 6;
    public bool timerIsRunning = false;

    [SerializeField]
    public SpellType _spellType;



    //Global variables
    Vector3 pos;
    GameObject _targetObject;
    Quaternion Rot;

    [Header("Car VFX")]
    [SerializeField]
    public GameObject _tpEffect;

    [Header("Debug Section")]
    #region Debug variables

    public bool _testing = false;
    public int _indexSpell = -1;

    #endregion


    #region Misc Variables

    private int _prevIndex = 0;

    #endregion

    private void Start()
    {
        timerIsRunning = true;
        if (_instance == null)
        {
            _instance = this;
        }
        _rb = GetComponent<Rigidbody>();
        if (isThere == false)
        {
            StartCoroutine(GenerateFireBall());
        }
        GenerateTarget();
    }

    private void GenerateTarget()
    {
        //Vector3 pos = new Vector3(-106, 11, 333);
        //Quaternion Rot = Quaternion.Euler(0f, 120f, 0f);
        pos = new Vector3(_targetSpawnPoint.position.x, _targetSpawnPoint.position.y - 3, _targetSpawnPoint.position.z);
        Quaternion Rot = _targetSpawnPoint.rotation;
        _targetObject = Instantiate(_fireballTarget, pos,Rot);
    }

    //Spell
    #region

    //Generates the fireball
    IEnumerator GenerateFireBall()
    {
        int _ballIndex = 0;
        while (_prevIndex == _ballIndex)
        {
            _ballIndex = UnityEngine.Random.Range(0, _fireBall.Length);
        }
        _prevIndex = _ballIndex;
        _generated = false;
        _generating = true;
        _shouldStartTimer = true;
        timerIsRunning = true;
        timeRemaining = 6;
        yield return new WaitForSeconds(0.1f);
        _generating = false;
        _generated = true;
        Debug.Log("Ball Generated");
        if(_ballIndex == 1 || _ballIndex == 3)
        {
            _g = Instantiate(_fireBall[_ballIndex], _spawnPoint.transform.position, _spawnPoint.transform.rotation, _spawnPoint.transform);
        }
        else
        {
            _g = Instantiate(_fireBall[_ballIndex], _spawnPoint.transform.position, _spawnPoint.transform.rotation);
        }
        _g.GetComponent<Collider>().enabled = false;
        _fireballRef = _g;
        string _tag = _g.tag;
        isThere = true;
        UpdatePos(true);
        ChangeSpellIcon(_tag);
    }

    //Starts the countdown for the spell
    private void StartTimer()
    {
        Debug.Log("Time Started");

        if (timerIsRunning)
        {
            float seconds = Mathf.FloorToInt(timeRemaining % 60);
            Debug.Log(seconds);
            if (seconds >= 0)
            {
                _spellButton.gameObject.SetActive(true);
                _spellButton.interactable = false;
                _timerText.gameObject.SetActive(true);
                _timerText.text = (seconds).ToString();
            }
            else
            {
                _spellButton.gameObject.SetActive(true);
                _timerText.gameObject.SetActive(false);
            }
            
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                _canShoot = false;
                _spellButton.interactable = false;
                Debug.Log("Time left" + seconds);
            }
            else if (timeRemaining <= 0)
            {
                _shouldUpdatePos = true;
                Debug.Log("Time has run out!");
                _spellButton.interactable = true;
                timeRemaining = 0;
                timerIsRunning = false;
                _canShoot = true;
           }
        }
    }

    //Shoots the spell
    private void Shoot()
    {
        //Find spell via attached CarShoot spell
        var _carshoot = FindObjectOfType<CarShoot>();
        _carshoot._spellType = _spellType;

        Debug.LogWarning("#Spelltype Assigned" + _spellType);
        _carshoot.SetThrust(Mathf.Abs(_rb.velocity.z));
        if (_carshoot)
        {
            _carshoot.Fire();
        }
        _shouldStartTimer = false;
        Debug.Log("Shoot called isthere and generating "+ isThere  + " "+ _generating);
        if(isThere == false && _generating == false && timerIsRunning == false)
        {
            _generated = false;
            _generating = true;
            _shouldStartTimer = true;
            timerIsRunning = true;
            Debug.Log("Calling generating Fireball");
            StartCoroutine(GenerateFireBall());
        }

    }

    //Changes the spell icon 
    private void ChangeSpellIcon(string tag)
    {
        switch (tag)
        {
            case "PurpleBall":
                _spell.sprite = _fireBallSprite[0];
                _spellType = SpellType.Tp;
                Debug.LogWarning("#Spelltype Tp");
                break;
            case "GreenBall":
                _spell.sprite = _fireBallSprite[1];
                _spellType = SpellType.Invisible;
                Debug.LogWarning("#Spelltype Invisible");
                break;
            case "OrangeBall":
                _spell.sprite = _fireBallSprite[2];
                _spellType = SpellType.Inverse;
                Debug.LogWarning("#Spelltype Inverse");
                break;
            case "BlueBall":
                _spell.sprite = _fireBallSprite[4];
                _spellType = SpellType.Freeze;
                Debug.LogWarning("#Spelltype Freeze");
                break;
            case "BlackBall":
                _spell.sprite = _fireBallSprite[3];
                _spellType = SpellType.Oil;
                Debug.LogWarning("#Spelltype Oil");
                break;
        }
        Debug.Log("Spell Function called with spell type Oncreation " + _spellType);
    }
    //Gives the reference of the direction to the spell
    public Transform GetTarget()
    {
        return _target;
    }

    //Changes the Updatepos bool
    private void UpdatePos(bool stats)
    {
        _shouldUpdate = stats;
    }
    #endregion


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.timeScale >= 0.5)
        {
            if (isThere && timerIsRunning == false)
            {
                Debug.LogWarning("Self Thrust" + _rb.velocity);
                isThere = false;
                UpdatePos(false);
                _g.GetComponent<Collider>().enabled = true;
                Shoot();
            }
        }

        if (Input.GetButtonDown("Fire2") && Time.timeScale >= 0.5)
        {
            if(isThere && timerIsRunning == false)
            {
                Debug.LogWarning("Self Thrust" + _rb.velocity);
                isThere = false;
                UpdatePos(false);
                _g.GetComponent<Collider>().enabled = true;
                Shoot();
            }
        }
        if (_fireballRef && _shouldUpdate)
        {
            _fireballRef.transform.position = _spawnPoint.transform.position;
        }
        if (_shouldStartTimer)
        {
            StartTimer();
        }
        UpdateTargetSpawnPoint();
    }

    public void ShootFireBall()
    {
        if (isThere && timerIsRunning == false)
        {
            Debug.LogWarning("Self Thrust" + _rb.velocity);
            isThere = false;
            UpdatePos(false);
            _g.GetComponent<Collider>().enabled = true;
            Shoot();
        }
    }


    private void UpdateTargetSpawnPoint()
    {
        pos = new Vector3(_targetSpawnPoint.position.x, _targetSpawnPoint.position.y, _targetSpawnPoint.position.z);
        Rot = _targetSpawnPoint.rotation;
        _targetObject.transform.rotation = Rot;
        _targetObject.transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            Debug.LogWarning("Ramp");
        }
    }
}
