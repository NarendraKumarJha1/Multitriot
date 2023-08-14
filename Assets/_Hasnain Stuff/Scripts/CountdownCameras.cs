using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownCameras : MonoBehaviour
{
    public static CountdownCameras instance;
    public Camera[] _cameras;
    private int cameraIndex = 0;

    public GameObject player;

    public GameObject _canvas;

    public TextMeshProUGUI _countDown;

    private int _index = 5;
    private void Start()
    {
        if(instance == null)
        {
            instance = new CountdownCameras();
        }
        _canvas.SetActive(true);
    }
    private void Update()
    {
        if (!GameController.instance) return;

        if (!player)
            player = GameController.instance.CurrentPlayer;

            if (cameraIndex == _cameras.Length-1)
            {
                _canvas.SetActive(false);
            }
        
    }

    public Camera SwitchToNextCamera()
    {


        if (player)
            transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);

        if (cameraIndex < _cameras.Length)
        {
            _countDown.text = _index--.ToString();
            DisableAllCountdownCameras();
            _cameras[cameraIndex].enabled = true;
            _cameras[cameraIndex].gameObject.SetActive(true);
            cameraIndex++;
            return _cameras[cameraIndex - 1];
        }
        return null;
    }

    public void Go()
    {
        _countDown.text = "Go!";
    }

    public void DisableAllCountdownCameras()
    {
        foreach (Camera cam in _cameras)
        {
            cam.enabled = false;
            cam.gameObject.SetActive(false);
        }
    }
}