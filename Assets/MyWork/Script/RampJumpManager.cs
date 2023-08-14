using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampJumpManager : MonoBehaviour
{
    public static RampJumpManager _instance;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _PlayerCar;

    private float _brakeInput;
    private bool _inAir;
    private bool _lowerRamp;
    private bool _midRamp;
    private bool _upperRamp;
    private bool _isLanded;

    private bool _midLRamp = false;
    private bool _midRRamp = false;
    enum JumpStatus
    {
        Jumped,
        CanJump
    }

    public enum JumpAbility
    {

        Able,
        Unable
    }

    private RCC_Settings RCCSettingsInstance;
    public RCC_Settings RCCSettings
    {
        get
        {
            if (RCCSettingsInstance == null)
            {
                RCCSettingsInstance = RCC_Settings.Instance;
            }
            return RCCSettingsInstance;
        }
    }

    float _speed;

    JumpStatus jumpStatus;
    JumpAbility jumpAbility;

    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        jumpStatus = JumpStatus.CanJump;
        jumpAbility = JumpAbility.Unable;
        if (RCC_SceneManager.Instance.activePlayerVehicle)
        {
            _speed = RCC_SceneManager.Instance.activePlayerVehicle.speed;
        }
    }

    private void Update()
    {
        Debug.Log("In air bool" + _inAir);
        GetSpeed();
        DebugBrakeStatus();
        CheckForFreeFall();
    }

    //Manipulating mass and gravity
    private void CheckForFreeFall()
    {
        if (_inAir || jumpStatus == JumpStatus.Jumped)
        {

            //_rb.mass = _lowerRamp ? 4000 : 5000;
            Physics.gravity = new Vector3(0, -18f, 0);
            if (_lowerRamp == false && _speed > 150f)
            {
                //_rb.mass = 22000;
            }
        }
        else
        {
            Physics.gravity = new Vector3(0, -10f, 0);
            //_rb.mass = 1500;
        }
    }

    //Manipulating mass and gravity after car lands for maintaining car stability 
    IEnumerator ResotoreDefaultMass()
    {
        //_rb.mass = 30000;
        Physics.gravity = new Vector3(0, -300f, 0);
        #region
        Debug.LogError("Mass" + _rb.mass);
        Debug.LogError("Gravity" + Physics.gravity);
        #endregion
        yield return new WaitForSeconds(5f);
        //_rb.mass = 1000;
        Physics.gravity = new Vector3(0, -10f, 0);
        #region
        Debug.LogError("Mass" + _rb.mass);
        Debug.LogError("Gravity" + Physics.gravity);
        #endregion
    }

    private void DebugBrakeStatus()
    {
        Debug.LogWarning("Reverse Stats :- " + RCC_SceneManager.Instance.activePlayerVehicle.canGoReverseNow);
    }

    public void GetSpeed()
    {
        _speed = RCC_SceneManager.Instance.activePlayerVehicle.speed;
    }

    public float GetSpeedOfCar()
    {
        return _speed = RCC_SceneManager.Instance.activePlayerVehicle.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collison Object " + other.gameObject.tag);

        CheckForRampCollision(other); // Checks for simple(Plane) ramp collision

        CheckCollisionOfUpperPartOfRamp(other); //Checks for upper part collision of twisted ramp.

        CheckCollisionOfMidPartOfRamp(other); //Checks for Mid part collision of twisted ramp.

        CheckCollisionOfLowerPartOfRamp(other); //Checks for lower part collision of twisted ramp.

        CheckForCarGrounded(other); //Checks if the car is landed

        CheckForAbilityToRotate(other); //Checks if the car properly crossed the ramp.

        if (other.gameObject.CompareTag("MidLeft"))
        {
            _midLRamp = true;
        }
        if (other.gameObject.CompareTag("MidRight"))
        {
            _midRRamp = true;
        }
        
    }

    //checks if the car properly crossed the ramp.
    private void CheckForAbilityToRotate(Collider other)
    {
        if (other.gameObject.CompareTag("MidRampCheck"))
        {
            _midRamp = true;
            jumpAbility = JumpAbility.Able;
        }
    }

    private void CheckForCarGrounded(Collider other)
    {
        if (other.gameObject.CompareTag("Track"))
        {
            _isLanded = true; // Cancel Maintain Position Coroutine
            StartCoroutine(CancelAnim());
            StartCoroutine(ResotoreDefaultMass());
            RestorePosAndRot();
            if (jumpStatus == JumpStatus.Jumped)
            {
                RestorePosAndRot();
            }
            CarShoot._instance.ScaleFireBall();
            StartCoroutine(DebugMassStats());
        }
    }

    IEnumerator CancelAnim()
    {
        for (int i = 5; i >= 0 ; i--){
            yield return new WaitForSeconds(0.01f);
            LeanTween.cancelAll();
            LeanTween.cancel(_PlayerCar);
        }

        var carshoot = FindObjectOfType<CarShoot>();
        Debug.LogError("Carshoot " + carshoot);
        if (carshoot)
        {
            carshoot.ScaleFireBall();
        }
    }

    IEnumerator DebugMassStats()
    {
        yield return null;
    }

    private void CheckCollisionOfMidPartOfRamp(Collider other)
    {
        if (other.gameObject.CompareTag("MidCurvedRamp"))
        {
            Debug.Log("Trigger Object " + other.gameObject.tag);
            Debug.LogError(" Mid Ramp ");
            _upperRamp = false;
            _midRamp = true;
            _lowerRamp = false;
            Spin(1);
        }
    }

    private void CheckCollisionOfLowerPartOfRamp(Collider other)
    {
        if (other.gameObject.CompareTag("LowerCurvedRamp"))
        {
            Debug.Log("Trigger Object " + other.gameObject.tag);
            Debug.LogError(" Lower Ramp ");
            _upperRamp = false;
            _midRamp = false;
            _lowerRamp = true;
            _inAir = true;
            StopCoroutine(SpinWithDelay(0));
            StartCoroutine(MaintainTheBalance(0, 2f, true));
        }
    }

    private void CheckCollisionOfUpperPartOfRamp(Collider other)
    {
        if (other.gameObject.CompareTag("HigherCurvedRamp"))
        {
            Debug.Log("Trigger Object " + other.gameObject.tag);
            Debug.LogError(" Higher Ramp ");
            _upperRamp = true;
            _midRamp = false;
            _lowerRamp = false;
            if (_speed > 200)
            {
                Spin(2);
            }
            else
            {
                Spin(1);
            }
        }
    }

    private void CheckForRampCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Ramp"))
        {
            Debug.Log("Collison Object " + other.gameObject.tag);
            if (jumpStatus == JumpStatus.CanJump)
            {
                JumpRotation();
            }
        }
    }

    private void RestorePosAndRot()
    {
        LeanTween.cancelAll();
        LeanTween.cancel(_PlayerCar);
        StopCoroutine(MaintainThePosition(0, 0));
        Debug.LogError("Cancelling all lean tweens");
        jumpStatus = JumpStatus.CanJump;
        jumpAbility = JumpAbility.Able;
        _inAir = false;
        _lowerRamp = false;
        _upperRamp = false;
        CarShoot._instance.ScaleFireBall();
    }

    private void JumpRotation()
    {
        _isLanded = false;
        Time.timeScale = 0.7f;
        if (_speed > 50 && _speed < 65)
        {
            LeanTween.rotate(_PlayerCar, new Vector3(-3, _PlayerCar.transform.localEulerAngles.y, _PlayerCar.transform.localEulerAngles.z), 4f);
            Debug.LogError("-8 rmp");
        }
        else if (_speed > 65 && _speed < 90)
        {
            LeanTween.rotate(_PlayerCar, new Vector3(-5, _PlayerCar.transform.localEulerAngles.y, _PlayerCar.transform.localEulerAngles.z), 4f);
            Debug.LogError("-11 rmp");
        }
        else if (_speed > 65 && _speed < 90)
        {
            LeanTween.rotate(_PlayerCar, new Vector3(-8, _PlayerCar.transform.localEulerAngles.y, _PlayerCar.transform.localEulerAngles.z), 4f);
            Debug.LogError("-14 rmp");
        }
        else if (_speed > 90)
        {
            LeanTween.rotate(_PlayerCar, new Vector3(-11, _PlayerCar.transform.localEulerAngles.y, _PlayerCar.transform.localEulerAngles.z), 4f);
            Debug.LogError("-50 rmp");
        }
        Time.timeScale = 1f;
        jumpStatus = JumpStatus.CanJump;
        _inAir = true;
        _lowerRamp = true;
    }
    private void Spin(int val)
    {
        _midRamp = false;
        if (jumpAbility == JumpAbility.Able && _speed > 95)
        {
            if (_midRamp || _upperRamp)
            {
                StartCoroutine(SpinWithDelay(val));
            }
        }
        else
        {
            Debug.LogError("spin but less speed");
            StartCoroutine(MaintainTheBalance(0, 2f, true));
        }
    }

    IEnumerator SpinWithDelay(int val)
    {
        yield return new WaitForSeconds(0.01f);
        Time.timeScale = 0.7f;
        Debug.LogError("Spinning");
        _isLanded = false;
        float _spinFactor = 0;
        if(_midRRamp)
        {
            _spinFactor = 720;
        }else if (_midLRamp)
        {
            _spinFactor = -360;
        }
        _midLRamp = false;
        _midRRamp = false;
        Debug.Log("SpinFactor " + _spinFactor);
        LeanTween.rotate(_PlayerCar, new Vector3(-10, _PlayerCar.transform.localEulerAngles.y, _spinFactor), 1f);
        if (val == 2)
        {
            StartCoroutine(SpinAgain(_spinFactor*2));
            StartCoroutine(MaintainThePosition(2f, 2f));
        }
        else
        {
            StartCoroutine(MaintainThePosition(1f, 2f));
        }
        jumpStatus = JumpStatus.Jumped;
        jumpAbility = JumpAbility.Unable;
        _inAir = true;
        _upperRamp = true;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;
    }

    IEnumerator MaintainThePosition(float wtime, float _animtime, bool rot = false)//maintain the balance of the car after the spin
    {
        Debug.LogError("maintain the balance of the car after the spin");
        Debug.LogError("wtime " + wtime + " " + "animtime " + _animtime + " " + "rot" + rot + " " + " lowerramp " + _lowerRamp + " " + " mid ramp " + _midRamp);
        if (!rot)
        {
            yield return new WaitForSeconds(wtime);
            Debug.LogError("Maintaining the pos " + _isLanded);
            LeanTween.rotate(_PlayerCar, new Vector3(-20, _PlayerCar.transform.localEulerAngles.y, _PlayerCar.transform.rotation.z), _animtime);
            _isLanded = false;
        }
    }

    IEnumerator MaintainTheBalance(float wtime, float _animtime, bool rot = false)//Tackle the situation when car jumps by lower part of the ramp 
    {
        Debug.LogError("wtime " + wtime + " " + "animtime " + _animtime + " " + "rot" + rot + " " + " lowerramp " + _lowerRamp + " " + " mid ramp " + _midRamp);
        Debug.LogError("handles the lower part of ramp jump");
        float _spinFactor = 0;
        if (rot && _lowerRamp && wtime == 0 )// handles the lower part of ramp jump
        {
            if (_speed > 120)
            {
                yield return new WaitForSeconds(wtime);
                if (_midLRamp)
                {
                    _spinFactor = 720;
                }
                else if(_midRRamp)
                {
                    _animtime = _animtime + 6;
                    _spinFactor = -720;
                }
                Debug.LogError("Maintaining the pos anti " + _isLanded);
                LeanTween.rotate(_PlayerCar, new Vector3(-20, _PlayerCar.transform.localEulerAngles.y, _spinFactor), _animtime);
            }
            else
            {
                yield return new WaitForSeconds(wtime);
                if (_midLRamp)
                {
                    _spinFactor = 720;
                }
                else if (_midRRamp)
                {
                    _animtime = _animtime + 6;
                    _spinFactor = -720;
                }
                Debug.LogError("Maintaining the pos anti " + _isLanded);
                LeanTween.rotate(_PlayerCar, new Vector3(-20, _PlayerCar.transform.localEulerAngles.y, _spinFactor), _animtime);
            }
        }
    }

    IEnumerator SpinAgain(float _spinFactor)
    {
        yield return new WaitForSeconds(0.9f);
        Debug.LogError("Spinning again" + _spinFactor);
        LeanTween.rotate(_PlayerCar, new Vector3(-7, _PlayerCar.transform.localEulerAngles.y, _spinFactor), 1f);
        _upperRamp = false;
    }

    public bool GetBrakeInput()
    {
        return _inAir;
    }
}
