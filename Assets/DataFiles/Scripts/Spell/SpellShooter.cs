using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System;
using System.Runtime.InteropServices;

public class SpellShooter : MonoBehaviour
{
    //PlayerRef 
    private GameObject _playerRef;
    private RCC_CarControllerV3[] _playersRef;
    private General _fireballRef;

    Vector3 origin;
    Vector3 direction = Vector3.forward; // Set direction vector
    Message _message;

    //Locked Car
    public Transform _targetObject = null;

    [SerializeField] public List <Transform> _targetObjects = null;

    RaycastHit _tempHit;
    private bool _hasPrevHit = false;
    private bool _isAnimating = false;
    private int _tempBotIndex = -1;


    //Distance variables
    private float _minDistance = 0;

    private void Start()
    {
        var _playerManager = FindObjectOfType<PlayerManager>()._isPlayer == true? FindObjectOfType<PlayerManager>():null;
        _playerRef = _playerManager.gameObject;
        _fireballRef = _playerRef.GetComponent<General>();
        origin = this.transform.position;
        // Create a new line renderer component
        _message = FindFirstObjectByType<Message>();
        _message.gameObject.SetActive(false);

        _playersRef = FindObjectsOfType<RCC_CarControllerV3>();
    }

    void Update()
    {
        var _carshoot = _fireballRef._spellType;
        Debug.LogWarning("#Spelltype "+ _carshoot);
        if (_targetObjects!=null)
        {

            switch (_carshoot)
            {
                case SpellType.Freeze:
                    Debug.LogWarning("#Spelltype Targetting " + _fireballRef._spellType);
                    AssignTarget(_targetObjects);
                    break;
                case SpellType.Tp:
                    Debug.LogWarning("#Spelltype Targetting " + _fireballRef._spellType);
                    AssignTarget(_targetObjects);
                    break;
                case SpellType.Inverse:
                    Debug.LogWarning("#Spelltype Targetting " + _fireballRef._spellType);
                    AssignTarget(_targetObjects);
                    break;
            }
        }

        if (!_targetObjects.Contains(_targetObject))
        {
            _targetObject = null;
            _message.gameObject.SetActive(false);
        }
    }

    public void PopUpMessage(GameObject _other)
    {
        _message.gameObject.SetActive(true);
        string var = _other.gameObject.name;
        var = var.Replace("(Clone)", "");
        _message._message.text = "Target Locked " + var;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        BotManager botManager = null;
        try
        {
            botManager = other.gameObject.GetComponent<BotManager>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        if (botManager != null && botManager._isAI && !_targetObjects.Contains(other.transform))
        {
            _targetObjects.Add(other.transform);
            //ShowAndHideTargetCube(other.gameObject,true);
            //AssignTarget(_targetObjects);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_targetObjects.Contains(_targetObject))
        {
            _targetObject = null;
            _message.gameObject.SetActive(false );
        }
    }
    private void OnTriggerExit(Collider other)
    {
        BotManager botManager = null;
        try
        {
            botManager = other.gameObject.GetComponent<BotManager>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        if (botManager!=null && botManager._isAI && _targetObjects.Contains(other.transform))
        {
            _targetObjects.Remove(other.transform);
            ShowAndHideTargetCube(other.gameObject, false);
        }

    }

    //Hide and Show Target Cube 
    private void ShowAndHideTargetCube(GameObject _gO, bool val)
    {
        _gO.GetComponent<BotManager>()._targetCube.SetActive(val);
    }

    private void AssignTarget(List<Transform> _gO)
    {
        _minDistance = 0;
        foreach(Transform _g in _gO)
        { 
            float _distance =Mathf.Abs(Vector3.Distance(_playerRef.transform.position, _g.position));
            if(_minDistance == 0)
            {
                _minDistance = _distance; 
            }
            Debug.Log("Distance "+ _distance);
            if (_distance <= _minDistance)
            {
                _targetObject = _g.transform;
                _minDistance = _distance;
                ShowAndHideTargetCube(_targetObject.gameObject, true);
                PopUpMessage(_targetObject.gameObject);
            }
            else
            {
                ShowAndHideTargetCube(_g.gameObject, false);
            }
        }
    }

}
