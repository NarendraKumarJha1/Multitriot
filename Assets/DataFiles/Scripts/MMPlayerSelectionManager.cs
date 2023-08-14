using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum Status
{
    Locked,
    Unclocked
}
[Serializable]
public class Car
{
    public GameObject _car;
    public string Name;
    public float _acceleration;
    public float _topSpeed;
    public float _handling;
    public float _nitro;
    public float _price;
    public Button _button;
    public Sprite _carSelected;
    public Sprite _carNormal;
    public Status _status;
}

public class MMPlayerSelectionManager : MonoBehaviour
{
    public Car[] _cars;

    [Header("Car properties variable")]
    [SerializeField]
    private TextMeshProUGUI _name, _acceleration, _topSpeed, _handling, _nitro, _price;

    [Header("Car Spawn point variable")]
    [SerializeField]
    private Transform _spawnPoint;

    //var for storing instantiated car
    private GameObject _carContainer;
    private int _index = 0;
    private int _prevIndex = 0;

    [Header("Effects")]
    [SerializeField]
    public GameObject _spawnEffect;


    [Header("Message")]
    [SerializeField] public GameObject _message;


    [Header("Panels")]
    [SerializeField] private GameObject _mainMenupanel;
    [SerializeField] private GameObject _garagePanel;

    [Header("Buttons")]
    [SerializeField] private Button _select;
    [SerializeField] private Button _next;
    [SerializeField] private Button _back;

    [Header("Bool")]
    public bool _isSpawning = false;

    private bool _isInitiated = false;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();        
        
        foreach (Car car in _cars)
        {
            if(car._status == Status.Locked)
            {
                car._button.GetComponent<Onhover>().Locked.gameObject.SetActive(true);
            }
        }
        Activate(0);
        _select.onClick.AddListener(() =>Play());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnPreviousClick();
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnNextClick();
        }
        if (_isSpawning)
        {
            foreach (Car car in _cars)
            {
                car._button.interactable = false;
            }
            _next.interactable = false;
            _back.interactable = false;
        }
        else if (!_isSpawning)
        {
            foreach (Car car in _cars)
            {
                car._button.interactable = true;
            }
            _next.interactable = true;
            _back.interactable = true;
        }

    }
    public  void OnPreviousClick() {
        if (_isSpawning)
        {
            return;
        }
        _isSpawning = true;
        UpdateCarProperties(false);
        DeactivateAllButton();
        if (_index < 0)
        {
            _index = _cars.Length-1;
            Debug.Log("Catching Exception "+ _index + " index"+_cars.Length);
        }
        else if(_index > _cars.Length - 1)
        {
            _index = 0;
            Debug.Log("Catching Exception " + _index);
        }
        ZoomCuurentButton(_index);
        Destroy(_carContainer != null ? _carContainer : null);
        StartCoroutine(InitiateCarSpawnEffect());
    }
    public void OnNextClick()
    {
        if (_isSpawning)
        {
            return;
        }
        _isSpawning = true;

        UpdateCarProperties(true);
        DeactivateAllButton();
        if (_index < 0)
        {
            _index = _cars.Length - 1;
            Debug.Log("Catching Exception " + _index + " index" + _cars.Length);
        }
        else if (_index > _cars.Length-1)
        {
            _index = 0;
            Debug.Log("Catching Exception " + _index);
        }

        ZoomCuurentButton(_index);
        Destroy(_carContainer != null ? _carContainer : null);
        StartCoroutine(InitiateCarSpawnEffect());
    }

    IEnumerator InitiateCarSpawnEffect()
    {
        if(_isSpawning)
        {
            yield return null;
        }
        _spawnEffect.SetActive(true);
        _isSpawning = true;
        yield return new WaitForSeconds(0.3f);
        
        _carContainer = Instantiate(_cars[_index]._car, _spawnPoint.position, _spawnPoint.rotation, _spawnPoint);
        _carContainer.AddComponent<ObjectMover>();
        _carContainer.gameObject.GetComponent<ObjectMover>().objectToMove = _carContainer;
        LeanTween.rotate(_carContainer, new Vector3(0, 800, 0), 5f);

        yield return new WaitForSeconds(3f);
        _spawnEffect.SetActive(false);
        _isSpawning = false;
    }

    private void UpdateCarProperties(bool val, bool _ignore = false)
    {

        //true for increment and false for decrement
        Debug.LogError("Updating Index " + _index + " val " + val);
        Debug.LogError("Updating Index " + _cars[_index].Name.ToString() + " val " + val);
        int _tempIndex = _index;
        if (!_ignore)
        {
            Debug.LogWarning("Cant Ignore");
            if (val)
            {
                if (_tempIndex >= _cars.Length - 1)
                {
                    _tempIndex = 0;
                }
                else
                {
                    _tempIndex++;
                }
            }
            else
            {
                if (_tempIndex == 0)
                {
                    _tempIndex = _cars.Length - 1;
                }
                else { 
                    _tempIndex--;
                }
            }
        }
        else
        {
            Debug.LogWarning("Ignore True ");
        }
        try
        {
            _name.text = _cars[_tempIndex].Name.ToString();
            _acceleration.text = _cars[_tempIndex]._acceleration.ToString();
            _topSpeed.text = _cars[_tempIndex]._topSpeed.ToString();
            _handling.text = _cars[_tempIndex]._handling.ToString();
            _nitro.text = _cars[_tempIndex]._nitro.ToString();
            _price.text = _cars[_tempIndex]._price.ToString();
            _prevIndex = _index;
        }
        catch (Exception e)
        {
            Debug.LogError("Index exception "+ _tempIndex);
        }
        if (!_ignore)
        {
            Debug.LogWarning("Cant Ignore");
            if (val)
            {
                _index++;
            }
            else
            {
                _index--;
            }
        }
        else
        {
            Debug.LogWarning("Ignore True ");
        }
        Debug.Log("Updated Index " + _index + " val " + val);
    }

    public void Activate(int index)
    {
        if (_isSpawning)
        {
            return;
        }
        _isSpawning = true;
        Destroy(_carContainer != null ? _carContainer : null);
        int i = 0;
        foreach (Car car in _cars)
        {
            if (i++ == index)
            {
                car._button.image.sprite = car._carSelected;
                StartCoroutine(InitiateCarSpawnEffect());
                _index = index;
                UpdateCarProperties(false, true);
            }
            else
            {
                car._button.image.sprite = car._carNormal;
            }
        }
        Debug.Log("Updated Index " + index);
    }


    private void DeactivateAllButton()
    {
        foreach (Car car in _cars)
        {
            car._button.image.sprite = car._carNormal;
        }
    }
    private void ZoomCuurentButton(int index)
    {
        int i = 0;
       foreach(Car car in _cars)
        {
            if(i++ == index)
            {
                car._button.image.sprite = car._carSelected;
            }
            else
            {
                car._button.image.sprite = car._carNormal;
            }
        }
    }

    public void Play()
    {
        Debug.Log("Index on play" + _index);
        Debug.Log("Index on play after updating" + _index);
        if (_cars[_index]._status == Status.Unclocked)
        {
            PlayerPrefs.SetInt("CarIndex", _index);
            StartCoroutine(ToggleObject(_garagePanel, false, 0.9f));
            StartCoroutine(ToggleObject(_mainMenupanel, true, 1.1f));
            StartCoroutine(ShowMessage(true));
        }
        else
        {
            StartCoroutine(ShowMessage(false));
            PlayerPrefs.SetInt("CarIndex", -1);
        }

        Debug.Log("_indexSet " + PlayerPrefs.GetInt("CarIndex"));
    }

    IEnumerator ShowMessage(bool val)
    {
        _message.SetActive(true);
        if (val == true)
        {
            _message.GetComponent<TextMeshProUGUI>().text = "Selected";
        }
        else
        {
            _message.GetComponent<TextMeshProUGUI>().text = "Can't select locked cars";
        }
        yield return new WaitForSeconds(0.5f);
        _message.SetActive(false);
    }

   

    public IEnumerator ToggleObject(GameObject _obj, bool val, float _dur)
    {
        Debug.Log("Togle " + _obj + val + _dur);
        yield return new WaitForSeconds(_dur);
        _obj.SetActive(val);
    }
}

