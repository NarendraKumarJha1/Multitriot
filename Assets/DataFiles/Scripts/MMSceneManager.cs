using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class AudioManagerClass
{
    public string Name;
    public AudioClip[] _audioClip;
}
public class MMSceneManager : MonoBehaviour
{
    private static MMSceneManager instance;
    
    public AudioManagerClass[] _audioManager;
    [Header("Audio source for playing permanent clips")]
    [SerializeField]
    public AudioSource _mainAudioSorce;

    [Header("Audio source for playing Temporary clips")]
    [SerializeField]
    public AudioSource _audioSourceTwo;

    [Header("Message Promp")]
    [SerializeField]
    public TextMeshProUGUI _message;


    private int _index = 0;
    private int _maxLength = 0;
    bool initiatedTrack = false;
    public bool _isAnimatingMessage = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _message.gameObject.SetActive(false);
        _audioSourceTwo = GetComponent<AudioSource>();
        //Playing MainMenu Track
        foreach (AudioManagerClass audioSource in _audioManager)
        {
            if (audioSource.Name == "MainMenu")
            {
                _mainAudioSorce.clip = audioSource._audioClip[0];
            }
        }
        //Getting Max length of gameplay 
        foreach (AudioManagerClass audioSource in _audioManager)
        {
            if (audioSource.Name == "GamePlay")
            {
                _maxLength = audioSource._audioClip.Length;
            }
        }
        _mainAudioSorce.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9) &&initiatedTrack)
        {
            if (_index < 0)
            {
                foreach (AudioManagerClass audioSource in _audioManager)
                {
                    if (audioSource.Name == "GamePlay")
                        _index = audioSource._audioClip.Length-1;
                }
            }else if(_index > _maxLength-1)
            {
                _index = 0;
            }
            PlayClip("GamePlay", ++_index);
        }
        if (Input.GetKey(KeyCode.Alpha0) && initiatedTrack)
        {
            if (_index < 0)
            {
                foreach (AudioManagerClass audioSource in _audioManager)
                {
                    if (audioSource.Name == "GamePlay")
                        _index = audioSource._audioClip.Length - 1;
                }
            }
            else if (_index > _maxLength - 1)
            {
                _index = 0;
            }
            PlayClip("GamePlay", ++_index);
        }
        if (SceneManager.GetActiveScene().buildIndex > 1 && !initiatedTrack)
        {
            PlayClip("GamePlay");
            initiatedTrack = true;
            _index++;
        }

    }
    public void PlayClip(string _val, int _index = 0)
    {
        foreach (AudioManagerClass audioSource in _audioManager)
        {
            if (audioSource.Name == _val)
                _mainAudioSorce.clip = audioSource._audioClip[_index];
        }
        _mainAudioSorce.Play();
        StartCoroutine(Popup());
    }

    public void PlayTempClip(string _val, int _index = 0)
    {
        foreach (AudioManagerClass audioSource in _audioManager)
        {
            if (audioSource.Name == _val)
                _audioSourceTwo.clip = audioSource._audioClip[_index];
        }
        _audioSourceTwo.Play();
        
    }
    IEnumerator Popup()
    {
        if(_isAnimatingMessage)
        {
            yield return null;
        }
        _isAnimatingMessage = true;
        _message.gameObject.SetActive(true);
        _message.text = "Changed AudioTrack";
        yield return new WaitForSeconds (0.5f);
        _message.gameObject.SetActive(false);
        _isAnimatingMessage = false;
    }
}
