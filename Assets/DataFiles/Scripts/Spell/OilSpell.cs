using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpell : MonoBehaviour
{
    [SerializeField] private bool _playerPassed = false;

    private void Start()
    {
        Destroy(gameObject,30f);
        StartCoroutine(TogglePlayerPassed());
    }

    IEnumerator TogglePlayerPassed()
    {
        yield return new WaitForSeconds(1.5f);
        _playerPassed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RCC_CarControllerV3>() && _playerPassed)
        {
            Debug.Log("Triggered");
            StartCoroutine(TractionOff(other.gameObject));
            GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator TractionOff(GameObject _other)
    {
        RCC_CarControllerV3 _car = _other.GetComponent<RCC_CarControllerV3>();
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
        float startTime = Time.time;
        float duration = 4f;
        
        int randomNumber = Random.Range(0, 2);
        int _steerInput = (randomNumber == 0) ? -1 : 1;
        while (Time.time - startTime < duration)
        {
            _car.steerInput = _steerInput;
            yield return null;
            Debug.LogWarning("Changing steerinput");
        }
        yield return new WaitForSeconds(4.2f);
        Debug.LogWarning("Assigning default properties");
        _car.tractionHelper = _tempTractionHelper;
        _car.tractionHelperStrength = _tempTractionHelperStrenth;
        _car.steeringHelper = _tempSteerHelper;
        _car.steerHelperLinearVelStrength = _tempSteerHelperLinear;
        _car.steerHelperAngularVelStrength = _tempSteerHelperAngular;
        Debug.LogWarning("Assigned default properties");
        Destroy(this,1f);
    }

}
