using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] public GameObject _targetCube;
    [SerializeField] public GameObject _explodeVFX;
    [SerializeField] public bool _isAI = true;
    private Message _message;
    public int _botIndex;
    private void Start()
    {
        if(_isAI)
        {
            this.gameObject.tag = "Enemy";
        }
        _targetCube.SetActive(false);
        _message = FindFirstObjectByType<Message>();
    }

    private void OnApplicationQuit()
    {
        //Debug.LogWarning("Assigning EnemyTag");
        if (_isAI)
        {
            this.gameObject.tag = "Enemy";
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarShoot>())
        {
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Debug.Log("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);
            Debug.LogWarning("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);
            Debug.LogError("Hit by spell " + other + " Spell " + other.gameObject.GetComponent<CarShoot>()._spellType);

            StartCoroutine(Popup(other.gameObject));

            //Check if spell is teleportation spell
            switch (other.gameObject.GetComponent<CarShoot>()._spellType)
            {
                case SpellType.Tp:
                    Debug.LogWarning("##Teleport Init##" + this.gameObject.name);
                    StartCoroutine(TeleportPlayer(other.gameObject));
                    break;
                case SpellType.Freeze:
                    Debug.LogWarning("##Freeze## Init" + this.gameObject.name);
                    StartCoroutine(Freeze(other.gameObject));
                    break;
                case SpellType.Invisible:
                    Debug.LogWarning("##Invisible Init##" + this.gameObject.name);
                    StartCoroutine(Invisible(other.gameObject));
                    break;
                case SpellType.Inverse:
                    Debug.LogWarning("##Inverse Init##" + this.gameObject.name);
                    StartCoroutine(Inverse(other.gameObject));
                    break;
            }
        }
    }

    //Perform Inverse spell
    #region
    IEnumerator Inverse(GameObject gameObject)
    {
        #region UnderSpell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        RCC_CarControllerV3 _car = GetComponent<RCC_CarControllerV3>();
        float startTime = Time.time;
        float duration = 2f; // Set the duration of the loop to 9 seconds


        bool _tempTractionHelper = _car.tractionHelper;
        float _tempTractionHelperStrenth = _car.tractionHelperStrength;
        bool _tempSteerHelper = _car.steeringHelper;
        float _tempSteerHelperLinear = _car.steerHelperLinearVelStrength;
        float _tempSteerHelperAngular = _car.steerHelperAngularVelStrength;
        _car.tractionHelper = true;
        _car.tractionHelperStrength = 0;
        _car.steeringHelper = true;
        _car.steerHelperLinearVelStrength = 0;
        _car.steerHelperAngularVelStrength = 0;
        _car.steerInput = 1;

        int randomNumber = UnityEngine.Random.Range(0, 2);
        int _steerInput = (randomNumber == 0) ? -1 : 1;
        while (Time.time - startTime < duration)
        {
            _car.steerInput = _steerInput;
            yield return null;
        }
        startTime = Time.time;
        duration = 2f;
        randomNumber = UnityEngine.Random.Range(0, 2);
        _steerInput = (randomNumber == 0) ? -1 : 1;
        while (Time.time - startTime < duration)
        {
            _car.steerInput = _steerInput;
            yield return null;
        }
        #endregion


        yield return new WaitForSeconds(5f);
        
        #region Spell released
        _car.tractionHelper = _tempTractionHelper;
        _car.tractionHelperStrength = _tempTractionHelperStrenth;
        _car.steeringHelper = _tempSteerHelper;
        _car.steerHelperLinearVelStrength = _tempSteerHelperLinear;
        _car.steerHelperAngularVelStrength = _tempSteerHelperAngular;

        #endregion
    }
    #endregion

    //Perform Teleportation
    #region
    IEnumerator TeleportPlayer(GameObject gameObject)
    {
        #region Under Spell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        //Getting Player transform
        Transform _player = gameObject.GetComponent<CarShoot>()._playerRef.transform;
        _player.GetComponent<General>()._tpEffect.SetActive(true);
        //Player rb
        Rigidbody _playerRB = _player.GetComponent<Rigidbody>();
        //Bot rb
        Rigidbody _botRB = GetComponent<Rigidbody>(); 
        //Initiating Tp Vfx Effect
        //Bot Transform
        Transform _bot = transform;
        //Bot position or Player Target postion
        

        yield return new WaitForSeconds(1.4f);
        Vector3 _posTo = _bot.position;
        //Player position or bot Target position
        Vector3 _posFrom = _player.position;
        //Bot Rotation or Player Target rotation
        Quaternion _rotTo = _bot.rotation;
        //Player rotation or bot target rotation
        Quaternion _rotFrom = _player.rotation;

        //Normalized velocity vectors of player
        Vector3 _playerVelocityDirection = _playerRB.velocity.normalized;
        //Normalized velocity vectors of bot
        Vector3 _botVelocityDirection = _botRB.velocity.normalized;
        yield return new WaitForSeconds(0.1f);

        #endregion

        #region Spell released
        _player.GetComponent<General>()._tpEffect.SetActive(false);
        //Spell Released
        //Changing player postion with the target bot
        _player.position = _posTo;
        _player.rotation = _rotTo;
        _bot.position = _posFrom;
        _bot.rotation = _rotFrom;
        _playerRB.velocity = _botVelocityDirection*_playerRB.velocity.magnitude;
        _botRB.velocity = _playerVelocityDirection*_botRB.velocity.magnitude;
        #endregion
    }
    #endregion

    //Perform Freeze spell
    #region

    IEnumerator Freeze(GameObject gameObject)
    {
        //Under Spell
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        Rigidbody _bot = this.gameObject.GetComponent<Rigidbody>();
        this.gameObject.GetComponent<RCC_CarControllerV3>().canControl = false;
        _bot.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        yield return new WaitForSeconds(3f);
        //Spell Released
        this.gameObject.GetComponent<RCC_CarControllerV3>().canControl = true;
        _bot.constraints = RigidbodyConstraints.None;
        Debug.Log("Freeze called");
    }
    #endregion

    //Perform Invisible spell
    #region
    IEnumerator Invisible(GameObject gameObject)
    {
        Debug.Log("Spell Function called with spell type " + gameObject.GetComponent<CarShoot>()._spellType);
        GetComponent<SpellEffect>().SpellFunc(gameObject.GetComponent<CarShoot>()._spellType);
        yield return new WaitForSeconds(8f);
    }

    #endregion

    IEnumerator Popup(GameObject _other)
    {
        _message.gameObject.SetActive(true);
        _message._message.text = "Hit by Spell " + _other.gameObject;
        yield return new WaitForSeconds(3f);
        _message.gameObject.SetActive(false);
    }
}
