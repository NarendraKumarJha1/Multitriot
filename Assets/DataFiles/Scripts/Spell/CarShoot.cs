using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShoot : MonoBehaviour
{
    public static CarShoot _instance;
    private Rigidbody _rb;
    public float m_Thrust = 3000;
    private Vector3 _targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    public float timeRemaining = 5;
    SpellShooter _spellShooter = null;
    Transform _targetObject = null;
    [SerializeField]
    public GameObject _playerRef = null;
    [SerializeField]
    public GameObject _OilVFX;
    public SpellType spellType;

    [SerializeField]
    public GameObject _sparkVFX;

    public SpellType _spellType
    {
        set
        {
            spellType = value;
            Debug.Log("Assigned Spell " + spellType);
        }
        get
        {
            return spellType;
        }
    }

    [Header("bools")]
    //If target is not loacked
    public bool _isTargetLocked = false;
    public bool _instantiated = false;
    public bool _canFire = false;
    public bool timerIsRunning = false;

    private void Start()
    {
        var _fireballRef = FindObjectOfType<General>();
        _spellType = _fireballRef._spellType;

        if (_instance == null)
        {
            _instance = this;
        }
        ScaleFireBall();
        _rb = GetComponent<Rigidbody>();
        _playerRef = FindObjectOfType<PlayerManager>().gameObject;
    }

    public void ScaleFireBall()
    {
        LeanTween.scale(this.gameObject, _targetScale, 5f);
    }

    public void Fire()
    {
        _spellShooter = FindObjectOfType<SpellShooter>();
        if (_spellShooter._targetObject != null)
        {
            _targetObject = _spellShooter._targetObject.GetComponent<SpellEffect>()._target.transform;
            _isTargetLocked = true;
        }
        else
        {
            Debug.LogError(" Error getting targetObject");
            _targetObject = General._instance.GetTarget();
        }

        Debug.Log("Fired");
        Debug.LogWarning("Actual Thrust " + m_Thrust);
        Debug.LogWarning("100X Thrust " + m_Thrust * 100);
        var _force = Mathf.Abs(m_Thrust);
        _force = Convert.ToInt32(_force);
        if (m_Thrust < 50)
        {
            _force = 3500;
        }
        else
        {
            _force = _force * 110;
        }
        Debug.LogWarning(" After Actual Thrust " + m_Thrust + " Force :- " + _force);
        Debug.LogWarning("After 100X Thrust " + m_Thrust * 100);
        if (_spellType == SpellType.Freeze || _spellType == SpellType.Tp || _spellType == SpellType.Inverse)
        {
            LookAtTarget(_force);
        }
        else if (_spellType == SpellType.Invisible || _spellType == SpellType.Oil)
        {
            LookAtSelf(_force);
        }
        Destroy(this.gameObject, 5f);

    }

    private void FixedUpdate()
    {
        Debug.LogWarning("#Spelltype from Carshoot" + _spellType);
        if (_canFire && _isTargetLocked)
        {
            Debug.Log("resetting target " + _targetObject.transform.position + " spell " + this.gameObject);
            if (_targetObject != null)
            {
                this.gameObject.transform.LookAt(_targetObject.transform);
                _rb.AddForce(transform.forward * m_Thrust);
            }
        }
    }

    //For absorbing spell make spell look at the car
    private void LookAtSelf(float _force)
    {
        if(_spellType == SpellType.Invisible)
        {
            _force = 6500;
        }
        else
        {
            _force = 4500;
        }
        m_Thrust = _force;
        this.gameObject.transform.LookAt(GetComponentInParent<RCC_CarControllerV3>().gameObject.transform);
        _rb.AddForce(transform.forward * m_Thrust);
    }

    private void LookAtTarget(float _force)
    {
        if (_targetObject != null)
        {
            this.gameObject.transform.LookAt(_targetObject.transform);
            m_Thrust = _force;
            _rb.AddForce(transform.forward * m_Thrust/3);
            _canFire = true; // Set _canFire to true after adding force
            Debug.LogWarning("##TargetObject is not equal null thrust " + m_Thrust);
        }
        else if (_targetObject == null)
        {
             m_Thrust = 6000;
             this.gameObject.transform.LookAt(General._instance.GetTarget());
             Debug.LogWarning("##TargetObject is null thrust "+ m_Thrust);
             _rb.AddForce(transform.forward * m_Thrust);
             _canFire = true; // Set _canFire to true after adding force
        }
    }

    public void SetThrust(float Val)
    {
        m_Thrust = Mathf.Abs(Val);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.GetComponent<RCC_AICarController>() || other.gameObject.GetComponent<BotManager>())
        {
            Destroy(this.gameObject);
            try
            { 
                Instantiate(_sparkVFX, this.transform.position, Quaternion.identity, other.transform);
            }catch(Exception e)
            {

            }
        }

        else if (_spellType == SpellType.Invisible)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Destroy(this.gameObject);
            }
        }

        CheckForOilSpell(other);


    }

    private void CheckForOilSpell(Collider other)
    {
        if (other.gameObject.CompareTag("Track") && _spellType == SpellType.Oil && !_instantiated)
        {
            if (_OilVFX != null)
            {
                GameObject _oil = Instantiate(_OilVFX, new Vector3(transform.position.x, other.transform.position.y + 0.3f, transform.position.z), Quaternion.identity);
                _instantiated = true;
                Destroy(this);
            }
        }
    }
}
