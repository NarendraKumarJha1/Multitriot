using System;
using UnityEngine;
using HWRWeaponSystem;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class PowerUpsHandler : MonoBehaviour
{
    #region Enum
    public enum PowerUpType { None, Boost, Missile, Conversion }
    public enum CurrentState { Car, Robot }
    #endregion

    #region Local Classes
    [Serializable]
    public class PowersData
    {
        public PowerUpType powerUpType = PowerUpType.Boost;
        public float powerCount = 1000;
    }
    #endregion

    public static PowerUpsHandler instance;
    public bool isAI = false;
    public bool _engine = true;
    public CurrentState currentState = CurrentState.Car;
    public List<PowersData> powersDatas = new List<PowersData>();

    public float currentTime = 0;
    public float powerAfterTime = 6;

    public float backToRobotTime = 15;
    public float remainingTime = 0;
    public KeyCode fireKey = KeyCode.F;
    public GameObject speedparticle;
    public bool isSpeedParticle;
    public bool _hasBoost = true;

    float robotConvertionTime = 0;
    Rigidbody rigidbody;
    public ProgressBar PbC;
    PhotonView photonView;

    public VehicleNitro aiNitro = null;
    private void OnEnable()
    {
        currentTime = Time.time;

        photonView = GetComponent<PhotonView>();
    }

    private void Awake()
    {

        _engine = true;
    /*    if (!isAI)
            SetUi();*/

        if (!rigidbody)
            rigidbody = this.GetComponent<Rigidbody>();
        if (instance == null)
        {
            instance = this;
        }
        aiNitro = GetComponent<VehicleNitro>();
    }

    public void KillEngine()
    {
        _engine = false;
    }

    public void StartEngine()
    {
        _engine = true;
    }
    private void Start()
    {
        if (!isAI)
        {
            PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);
            //Debug.Log("Powercount" + _pd.powerCount);
            PbC.BarValue = _pd.powerCount;
        }
    }
    private void Update()
    {
        CheckForNitro();
        IncreaseNitro();

        CheckSpeedForparticle();
        PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);
        if(_pd.powerCount<1000 && aiNitro.NOSBool == false)
        {
            StartCoroutine(IncreasePowerOfBoost());
        }
        //Debug.Log("Powercount" + _pd.powerCount);
    }

    private void IncreaseNitro()
    {

    }
    IEnumerator IncreasePowerOfBoost()
    {
        if (isAI)
        {
            yield return null;
        }
        else
        {
            PowersData _pd = powersDatas.Find(x => x.powerUpType == PowerUpType.Boost);
            while (_pd.powerCount < 1000 && !aiNitro.NOSBool)
            {
                yield return new WaitForSeconds(0.1f);
                _pd.powerCount = _pd.powerCount + 0.8f;
                PbC.BarValue = _pd.powerCount;

                if (_pd.powerCount > 30)
                {
                    _hasBoost = true;
                }
                else if (_pd.powerCount < 20)
                {
                    _hasBoost = false;
                }
            }
        }
    }

    private void CheckForNitro()
    {
        if (!isAI)
        {
            if (Input.GetKey(KeyCode.LeftShift) && _engine == true && Time.timeScale>0.5)
            {
                Debug.LogError("Boost1");
                if (GetPowerCount(PowerUpType.Boost) > 0 && _hasBoost)
                {
                    Debug.LogError("Boost2");
                    Boost();
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                VehicleNitro.instance.DisableNitro();
                aiNitro.BoostUp();
            }
        }
    }

    void CheckSpeedForparticle()
    {
        if (isSpeedParticle)
            speedparticle.SetActive(this.GetComponent<RCC_CarControllerV3>().speed > 180 && currentState != CurrentState.Robot);
    }

    public float nitorDuration = 5, extraForceAmount = 1000;

    void Boost()
    {
        if (GetPowerCount(PowerUpType.Boost) > 0 &&_hasBoost)
        {
            DecrementPower(PowerUpType.Boost);
            aiNitro.ActivateNitro(nitorDuration, extraForceAmount, 5000);
        }
        else
        {
            aiNitro.DisableNitro();
            _hasBoost = false;
        }
    }

    bool HasPower(PowerUpType power)
    {

        PowersData _pd = powersDatas.Find(x => x.powerUpType == power);
        //Debug.Log("Powercount" + _pd.powerCount);
        if (_pd != null)
            return _pd.powerCount > 0;

        return false;
        /*        return true;*/
    }

    void DecrementPower(PowerUpType powerUpType)
    {
        PowersData _pd = powersDatas.Find(x => x.powerUpType == powerUpType);

        if (_pd != null)
        {
            _pd.powerCount = _pd.powerCount - 5;
            if (_pd.powerCount < 0)
                _pd.powerCount = 0;
            if (!isAI)
            {
                PbC.BarValue = _pd.powerCount;
            }
        }
    }

    public float GetPowerCount(PowerUpType powerUpType)
    {
        PowersData _pd = powersDatas.Find(x => x.powerUpType == powerUpType);

        if (_pd != null)
            return _pd.powerCount;

        return 0;
    }

    public GameObject particleEffectParent = null;
    public void ShowParticleEffect()
    {
        if (!isAI)
        {
            particleEffectParent.SetActive(true);
            disableCoroutine = StartCoroutine(DisableEffect());
        }
    }
    Coroutine disableCoroutine;
    IEnumerator DisableEffect()
    {
        yield return new WaitForSeconds(3.0f);

        particleEffectParent.SetActive(false);
        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine);
        disableCoroutine = null;
    }

    public void AddPowerUp(PowerUpType pickUpType)
    {
        AddPower(pickUpType);
    }

    void AddPower(PowerUpType pickUpType)
    {
        for (int i = 0; i < powersDatas.Count; i++)
        {
            if (pickUpType == powersDatas[i].powerUpType)
            {
                powersDatas[i].powerCount++;
                break;
            }
        }
    }
    public bool IsCar()
    {
        return currentState == CurrentState.Car;
    }
}