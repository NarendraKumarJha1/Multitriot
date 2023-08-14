using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleNitro : MonoBehaviour
{
    public static VehicleNitro instance;
    public Rigidbody rb;
    public AudioSource _nitroSound;
    public enum Mode
    {
        Acceleration,
        Impulse
    };
    public Mode mode = Mode.Acceleration;
    public float forceValue = 100f;
    public float maxVelocity = 6000f;
    bool isNiroPlaying = false;

    public ParticleSystem[] AfterBurnerEffects;

    public float minFOV;
    public float maxFOV;
    public bool NOS;
    public bool _heldDown;
    public bool NOSBool
    {

        get { return NOS; }
        set { NOS = value; }
    }

    Rigidbody m_rigidBody;
    public bool isAi = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GetComponent<PowerUpsHandler>().GetPowerCount(PowerUpsHandler.PowerUpType.Boost) < 1)
        {
            NOS = false;
        }
        if(NOS && !_nitroSound.isPlaying)
        {
            _nitroSound.Play();
        }
        if(!NOS && _nitroSound.isPlaying)
        {
            _nitroSound.Stop();
        }
        if(!isAi)
            PushCar();
    }
    public void BoostUp()
    {
        _heldDown = false;
        NOSBool = _heldDown;
    }
    public void BoostDown()
    {
        _heldDown = true;
        NOSBool = _heldDown;
    }

    void FixedUpdate()
    {
        if (NOS)
        {
            if (m_rigidBody.velocity.magnitude < maxVelocity)
            {
                m_rigidBody.AddRelativeForce(Vector3.forward * forceValue, mode == Mode.Acceleration ? ForceMode.Acceleration : ForceMode.Impulse);
                Debug.Log("Nitro");
                if (!AfterBurnerEffects[0].gameObject.activeInHierarchy)
                {
                    object[] _data = new object[] { true, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpsHandler.PowerUpType.Boost };
                    PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);

                    foreach (ParticleSystem ps in AfterBurnerEffects)
                        ps.gameObject.SetActive(true);
                }
            }
            else
                NOS = false;
        }
        else
        {
            if (AfterBurnerEffects[0].gameObject.activeInHierarchy)
            {
                object[] _data = new object[] { false, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, PowerUpsHandler.PowerUpType.Boost };
                PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.PowerUsed, _data);

                foreach (ParticleSystem ps in AfterBurnerEffects)
                    ps.gameObject.SetActive(false);
            }
        }
        NOSBool = NOS;
    }

    public void ActiveNosEffect(bool active)
    {
        foreach (ParticleSystem ps in AfterBurnerEffects)
            ps.gameObject.SetActive(active);
    }

    public void ActivateNitro(float duration, float extraForceAmount, float maxVelocityToMaintain)
    {
        //Debug.Log("Activating Nitro");
        NOS = true;
        CancelInvoke("DisableNitro");
        Invoke(nameof(DisableNitro), duration);
        if (isNiroPlaying == false)
        {
            
            isNiroPlaying = true;
        }


    }

    public void DisableNitro()
    {
        NOS = false;
        if (isNiroPlaying == true)
        {
           
            isNiroPlaying = false;
        }
    }

    private void PushCar()
    {
        if (NOS)
        {
            GetComponent<RCC_CarControllerV3>().gasInput = 1;
        }
    }
}