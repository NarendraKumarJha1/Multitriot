using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//CameraManager.cs handles activating/deactivating race cameras based on their corresponing race states.
//This makes it better to manage race cameras

namespace RGSK
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager instance;

        private List<Camera> cameraList = new List<Camera>();

        private AudioListener audioListener;

        [Header("Starting Grid Camera")]
        public Camera startingGridCamera;

        [Header("Player Camera")]
        public GameObject playerCamera;


        [Header("Player Camera")]
        public GameObject countdownCanvas;

        [Header("Cinematic Camera")]
        public Camera cinematicCamera;

        [Header("MiniMap Camera")]
        public Camera minimapCamera;

        [Header("Countdown Cameras")]
        public CountdownCameras countdownCamerasContainer; //hasnain

        [Header("Finish Point Cinematic Camera")]
        public RCC_CinematicCamera finishPointCinematicCamera;

        public TextMeshProUGUI _countdown;

        void Awake()
        {
            instance = this;

            CreateAudioListener();

            AddCamerasToCameraList();

            var pos = minimapCamera.transform.position;
            pos.x = -976;
            pos.z = -515;
            minimapCamera.transform.position = pos; 
        }
        void Update()
        {
            //Make sure the minimap is only enabled in racing state
            if (minimapCamera && RaceManager.instance)
                minimapCamera.enabled = RaceManager.instance._raceState == RaceManager.RaceState.Racing;
        }

        public void ActivatePlayerCamera()
        {
            //finishPointCinematicCamera.Activate (false);
            countdownCamerasContainer.DisableAllCountdownCameras(); //
            StartCoroutine(Go());
           
            for (int i = 0; i < cameraList.Count; i++)
            {
                if (cameraList[i] == playerCamera)
                {
                    cameraList[i].enabled = true;
                    SetAudioListerParent(cameraList[i].transform);
                }
                else
                {
                    if (playerCamera != null)
                        cameraList[i].enabled = false;
                }
            }
        }

        IEnumerator Go()
        {
            playerCamera.gameObject.SetActive(true);
            _countdown.text = "Go!";
            yield return new WaitForSeconds(1f);
            countdownCanvas.gameObject.SetActive(false);
        }

        public void ActivateStartingGridCamera()
        {
            //finishPointCinematicCamera.Activate (false);
            countdownCamerasContainer.DisableAllCountdownCameras(); //

            for (int i = 0; i < cameraList.Count; i++)
            {
                if (cameraList[i] == startingGridCamera)
                {
                    cameraList[i].enabled = true;
                    SetAudioListerParent(cameraList[i].transform);
                }
                else
                {
                    if (startingGridCamera != null)
                        cameraList[i].enabled = false;
                }
            }
        }

        public void ActivateCinematicCamera()
        {
            finishPointCinematicCamera.Activate(false);
            countdownCamerasContainer.DisableAllCountdownCameras(); //

            for (int i = 0; i < cameraList.Count; i++)
            {
                if (cameraList[i] == cinematicCamera)
                {
                    cameraList[i].enabled = true;
                    SetAudioListerParent(cameraList[i].transform);
                }
                else
                {
                    if (cinematicCamera != null)
                        cameraList[i].enabled = false;
                }
            }
        }

        public void ActivateFinishPointCinematicCamera()
        {
            for (int i = 0; i < cameraList.Count; i++)
            {
                cameraList[i].enabled = false;
            }
            if (finishPointCinematicCamera)
                finishPointCinematicCamera.Activate(true);
            if (RaceUI.instance)
                RaceUI.instance.ShowTournamentRaceCompletePanel(2f); //hasnain
        }

        public void SwicthBetweenReplayCameras()
        {
            finishPointCinematicCamera.Activate(false);
            countdownCamerasContainer.DisableAllCountdownCameras();

            if (cinematicCamera.enabled)
            {
                ActivatePlayerCamera();
            }
            /*else if (playerCamera.enabled)
            {
                if (playerCamera.GetComponent<PlayerCamera>())
                    playerCamera.GetComponent<PlayerCamera>().SwitchCameras();
            }*/
        }

        public void SwitchToNextCountdownCamera()
        {
           // Debug.LogError("cameras fixed");
            for (int i = 0; i < cameraList.Count; i++)
            {
                cameraList[i].enabled = false;
            }

            Camera countDownCam = countdownCamerasContainer.SwitchToNextCamera();
            if (countDownCam != null)
                SetAudioListerParent(countDownCam.transform);
        }

        void SetAudioListerParent(Transform t)
        {
            audioListener.transform.parent = t;
            audioListener.transform.localPosition = Vector3.zero;
            audioListener.transform.localRotation = Quaternion.identity;
        }

        void AddCamerasToCameraList()
        {
            //Add the cameras to the cameraList for easier access later
            if (startingGridCamera && !cameraList.Contains(startingGridCamera))
                cameraList.Add(startingGridCamera);

          /*  if (playerCamera && !cameraList.Contains(playerCamera))
                cameraList.Add(playerCamera);*/

            if (cinematicCamera && !cameraList.Contains(cinematicCamera))
                cameraList.Add(cinematicCamera);

        }

        void CreateAudioListener()
        {
            //Create an audioListener to make them easier to manage
            audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();

            //Get rid of all other audioListeners so that we dont get that annoying debug message :)
            AudioListener[] allListeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
            foreach (AudioListener a in allListeners)
            {
                if (a != audioListener)
                    Destroy(a);
            }
        }
    }
}