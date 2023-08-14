using Org.BouncyCastle.Crypto.Macs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    public bool _isPlayer = false;

    [SerializeField]
    public GameObject _playerCanvas;


    [SerializeField]
    public TextMeshProUGUI _speed;
    internal float KMH;

    private void Start()
    {
        if (!_isPlayer)
        {
            _playerCanvas.SetActive(false);
        }
    }
    private void Update()
    {
        KMH = RCC_SceneManager.Instance.activePlayerVehicle.speed;
        SetSpeed(Mathf.RoundToInt(KMH));
    }
    private void SetSpeed(int _speedVal)
    {
        _speed.text = _speedVal.ToString();
    }
}
