using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class CheckPoint : MonoBehaviour {
	public string playerTag = "Player";
    private AudioSource _ad;
    public static CheckPoint instance;

    bool isGameStarted = true;

    //Collider collision tag
    private int _count = 0;
    private int _Fcount = 0;
    private int _PreFcount = 0;
    private int _PostCount = 0;
    private int _MidCount = 0;
    private void Awake()
    {
        _ad = GetComponent<AudioSource>();
        if(instance == null)
        {
            instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            _count++;
            LapManager.instance.ToggleText(1);
        }
        if (other.gameObject.CompareTag("FinalCheckPoint"))
        {
            _Fcount++;
            
        }if (other.gameObject.CompareTag("PreFinalCheckPoint"))
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

    private void Update()
    {
        if (_count > 1)
        {
            _ad.Play();
        }
        if (_count > 6 && _Fcount>1 && _PreFcount > 1 && _PostCount > 1 && _MidCount > 1)
        {
            LapManager.instance.SetPrevLap();
            LapManager.instance.IncLap();
            StartCoroutine(ResetCounters());
        }
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

    public void ReduceCount()
    {
        _count = 1;
        LapManager.instance.DecLap();
    }
}