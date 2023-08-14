using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


//Statistics.cs keeps track of the racer's rank, name, vehicle name, lap, race times, race state, saving best times, wrong way detecion etc.
namespace RGSK
{
    public class Statistics : MonoBehaviour
    {
        public bool isPlayer = false;

        [System.Serializable]
        public class RacerDetails
        {
            public string racerName;
            public string vehicleName;
        }

        [SerializeField] public RacerDetails racerDetails;
        public RacerDetails RacerDetail
        {
            get => racerDetails;
            set => racerDetails = value;
        }

        //Int
        public int rank; //current rank
        public int lap; //current lap
        public int checkpoint; //current checkpoint(Checkpoint Race)

        //Strings
        public string currentLapTime; //current lap time string displayed by RaceUI.cs
        public string prevLapTime; //Previous lap time string displayed by RaceUI.cs
        public string totalRaceTime; //Total lap time string displayed by RaceUI.cs
        public string bestLapTime; //Best lap time string for current session;

        //Floats
        private float lapTimeCounter; // keeps track of our current Lap time counter
        private float totalTimeCounter; //keeps track of our total race time
        private float prevLapCounter;
        private float bestLapCounter; //keeps track of the current session's best lap time
        private float dotProduct; //used for wrong way detection
        private float registerDistance = 10.0f; //distance to register a passed node
        private float reviveTimer;
        [SerializeField] private float wrongwayTimer; //delay timer

        //Hidden Vars
        //[HideInInspector]
        public Transform lastPassedNode;

        //[HideInInspector]
        public Transform target; //progress tracker target
        [SerializeField] int currentNodeNumber; //next node index in the "path" list
        public int CurrentNodeNumber
        {
            get => currentNodeNumber;
            set
            {
                currentNodeNumber = value;
               /* RankManager.instance.OnNodeChange();*/
            }
        }
        //[HideInInspector]
        public List<Transform> path = new List<Transform>();
        //[HideInInspector]
        public List<bool> passednodes = new List<bool>();
        //[HideInInspector]
        public List<Transform> checkpoints = new List<Transform>();
        //[HideInInspector]
        public List<bool> passedcheckpoints = new List<bool>();
        //[HideInInspector]
        public bool finishedRace;
        //[HideInInspector]
        public bool knockedOut;
        //[HideInInspector]
        public bool goingWrongway;
        //[HideInInspector]
        public bool passedAllNodes;
        //[HideInInspector]
        public bool infiniteLaps;
        //[HideInInspector]
        public float speedRecord; //speed trap top speed

        public bool init = false;
        private PhotonView pv;
        void Start()
        {
            init = false;
            Init();
            pv = GetComponent<PhotonView>();

            if (PhotonManager.IsGuest)
                racerDetails.vehicleName = transform.name;
        }

        void Init()
        {
            passedAllNodes = false;

            FindPath();
            FindCheckpoints();
            Initialize();

            init = true;
            CurrentNodeNumber = 0;
            currentNodeNumber = 0;
        }

        void FindPath()
        {
            if (!RaceManager.instance) return;
            if (!RaceManager.instance.pathContainer) return;

            Transform[] nodes = RaceManager.instance.pathContainer.GetComponentsInChildren<Transform>();

            foreach (Transform p in nodes)
                if (p != RaceManager.instance.pathContainer)
                    path.Add(p);

            passednodes = new List<bool>(new bool[path.Count]);

            lastPassedNode = path[0];
        }
        void FindCheckpoints()
        {
            if (!RaceManager.instance) return;
            if (!RaceManager.instance.checkpointContainer)
                return;

            Checkpoint[] _checkpoint = RaceManager.instance.checkpointContainer.GetComponentsInChildren<Checkpoint>();

            foreach (Checkpoint c in _checkpoint)
            {
                //Find SpeedTrap Checkpoints
                if (RaceManager.instance._raceType == RaceManager.RaceType.SpeedTrap)
                    if (c.checkpointType == Checkpoint.CheckpointType.Speedtrap)
                        checkpoints.Add(c.transform);

                //Find Time Checkpoints
                if (RaceManager.instance._raceType == RaceManager.RaceType.Checkpoints)
                    if (c.checkpointType == Checkpoint.CheckpointType.TimeCheckpoint)
                        checkpoints.Add(c.transform);
            }

            passedcheckpoints = new List<bool>(new bool[checkpoints.Count]);
        }
        void Initialize()
        {
            if (!RaceManager.instance) return;
            lap = 1;

            //Set the start timer for a checkpoint race
            if (RaceManager.instance._raceType == RaceManager.RaceType.Checkpoints)
                lapTimeCounter = RaceManager.instance.initialCheckpointTime;

            //Set the elimination timer for a eimination race
            if (RaceManager.instance._raceType == RaceManager.RaceType.Elimination)
                lapTimeCounter = RaceManager.instance.eliminationTime;

            //Set the time limit timer for a drift race
            if (RaceManager.instance._raceType == RaceManager.RaceType.Drift && RaceManager.instance.timeLimit)
                lapTimeCounter = RaceManager.instance.driftTimeLimit;

            infiniteLaps = RaceManager.instance._raceType == RaceManager.RaceType.TimeTrial;


        }

        void Update()
        {
            if (path.Count <= 0)
            {
                init = false;
                Debug.LogError("Init Again");
                Init();
                return;
            }

/*            if (init)
                GetComponent<ProgressTracker>().Init();*/

            if (!target)
                return;

            GetPath();
            CalculateRaceTimes();
            CheckForWrongway();
            CheckForRespawn();
        }

        void GetPath()
        {
            if (CurrentNodeNumber >= path.Count)
            {
                if (raceEnded == null)
                {
                    raceEnded = StartCoroutine(RaceEndNodeInc());
                    CurrentNodeNumber = 0;
                }
                //Debug.LogError("why reaching here????");
                return;
            }

            int n = CurrentNodeNumber;

            Transform fNode = path[n] as Transform;
            Vector3 fNodeVector = target.InverseTransformPoint(fNode.position);

            //register that we have passed this node
            if (fNodeVector.magnitude <= registerDistance)
            {
                CurrentNodeNumber++;

                passednodes[n] = true;

                //set our last passed node
                if (n != 0)
                    lastPassedNode = path[n - 1];
                else
                    lastPassedNode = path[path.Count - 1];
                if (gameObject.tag == "Player") //Testing
                    Debug.LogError("Current Node Numbers is= " + CurrentNodeNumber);
            }

            //Check if all nodes have been passed
            bool passed = true;
            foreach (bool pass in passednodes)
            {
                if (!pass)
                {
                    passed = false;
                    break;
                }
            }
            passedAllNodes = passed;

            // Debug.LogError("Current Path is= " + n);
            //Reset the currentNodeNumber after passing all the nodes
            if (CurrentNodeNumber >= path.Count)
                CurrentNodeNumber = 0;
        }

        // Race time calculations
        void CalculateRaceTimes()
        {
            if (RaceManager.instance.raceStarted && !knockedOut && !finishedRace)
            {

                if (RaceManager.instance.timerType == RaceManager.TimerType.CountUp)
                {
                    lapTimeCounter += Time.deltaTime;
                }

                if (RaceManager.instance.timerType == RaceManager.TimerType.CountDown)
                {
                    lapTimeCounter -= Time.deltaTime;

                    if (lapTimeCounter <= 0)
                    {
                        if (RaceManager.instance._raceType == RaceManager.RaceType.Checkpoints)
                        {
                            knockedOut = true;

                            RaceManager.instance.KnockoutRacer(this);
                        }

                        if (RaceManager.instance._raceType == RaceManager.RaceType.Drift)
                        {
                            if (!knockedOut && !finishedRace)
                                FinishRace();
                        }
                    }
                }

                totalTimeCounter += Time.deltaTime;
            }

            //Format the time strings
            currentLapTime = RaceManager.instance.FormatTime(lapTimeCounter);

            totalRaceTime = RaceManager.instance.FormatTime(totalTimeCounter);
        }

        public void NewLap()
        {

            // Debug.LogError("lap " + lap + " / " + RaceManager.instance.totalLaps + "    " + finishedRace);
            if (finishedRace || knockedOut) return;

            prevLapTime = currentLapTime;
            prevLapCounter = lapTimeCounter;

            CheckForBestTime();
            //Reset passed nodes
            for (int i = 0; i < passednodes.Count; i++)
                passednodes[i] = false;

            //Reset passed checkpoints
            for (int i = 0; i < passedcheckpoints.Count; i++)
                passedcheckpoints[i] = false;

            CurrentNodeNumber = 0;
            //Lap increment logic
            lap++;

            if (GameController.instance && transform.tag == "Player")
            {
                //if (GameController.instance.levelToLoad == LevelToLoad.Futuristic ||
                //    GameController.instance.levelToLoad == LevelToLoad.Lava ||
                //    GameController.instance.levelToLoad == LevelToLoad.Shoefy)
                //{
                if (RaceManager.instance.totalLaps > 1 && !GameController.instance.isStartTextChanged)
                {
                    if (lap == RaceManager.instance.totalLaps)
                    {
                        GameController.instance.ChangeStartPointText();
                        GameController.instance.isStartTextChanged = true;
                    }
                }
                //}
            }

            if (!infiniteLaps)
            {
                //Show final lap indication on the final lap
                //if (gameObject.CompareTag("Player"))
                //    ShowEffect();

                if (lap > RaceManager.instance.totalLaps)
                    if (!knockedOut && !finishedRace)
                    {
                        if (!PhotonManager.IsGuest)
                            FinishRace();
                        else
                            FinishRaceFreeMode();

                        if (gameObject.CompareTag("Player"))
                            ShowEffect();
                    }
            }

            //Reset the lap timer based on the Timer Type
            if (RaceManager.instance.timerType != RaceManager.TimerType.CountDown)
                lapTimeCounter = 0.0f;

            //Check for knockout
            if (RaceManager.instance._raceType == RaceManager.RaceType.LapKnockout)
                if (this.rank == RankManager.instance.TotalRacers - 1)
                    RaceManager.instance.KnockoutRacer(RaceManager.instance.GetLastPlace());
        }
        public void ShowEffect()
        {
            if (GetComponent<PowerUpsHandler>())
                GetComponent<PowerUpsHandler>().ShowParticleEffect();
        }

        IEnumerator RaceEndCutScene()
        {
            if (GameController.instance)
            {
                GameController.instance.PauseBtn.SetActive(false);
                GameController.instance.rccCanvasObject.SetActive(false);
            }
            RCC_CarControllerV3 thisCarControl = GetComponent<RCC_CarControllerV3>();
            thisCarControl.canControl = false;
            thisCarControl.externalController = true;
            yield return new WaitForSeconds(1f);
            thisCarControl.brakeInput = 1;
            thisCarControl.GetComponent<Rigidbody>().useGravity = false;
            thisCarControl.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void FinishRace()
        {

            Debug.Log("finished called is guest " + PhotonManager.IsGuest);
            if (finishedRace) return;


            pv.RPC("SendMyRaceFinished", RpcTarget.All, racerDetails.racerName);


            finishedRace = true;

            object[] _data = new object[] { GetComponent<PhotonView>().Controller.ActorNumber };
            PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.RaceEnded, _data);

            //Player finish
            if (gameObject.CompareTag("Player"))
            {
                RaceManager.instance.EndRace(rank);
                Debug.Log("finished called 1 " + " Player id " + UserDatabase.Instance.localUserData.id);

                if (rank == 1)
                {

                    StartCoroutine(RaceEndCutScene());
                    CameraManager.instance.ActivateFinishPointCinematicCamera();
                }
                else
                {
                    if (GameController.instance)
                        GameController.instance.miniMapCamera.gameObject.SetActive(false);

                    StartCoroutine(RaceEndCutScene());
                }

                //setting winner here
                Invoke("SetWinner", 2);
            }

            foreach (AudioSource audio in gameObject.GetComponentsInChildren<AudioSource>())
            {
                audio.mute = true;
            }

            //Continue after finishing
            if (RaceManager.instance.continueAfterFinish)
            {
                AIMode();
            }
            else
            {
                RaceManager.instance.DisableRacerInput(gameObject);
            }


        }


        private void SetWinner()
        {
            if (!RankManager.instance.racersOnlineStats[0].Contains(transform.name))
                return;

            //set winner here
            if (PhotonManager.FreeToPlay)
            {
                //free to play winner
                int amt = System.Convert.ToInt32(PhotonManager.RoomAmtT);
                amt = amt * 2; // doubling the room amount
                ApiManager.instance.dumTokens.AddTokenss(amt.ToString());
                Debug.LogError("im the free to play winner");
            }
            else
            {
                if (BlockchainDataManager.currentGameMode == GameMode.Wager)
                {
                    ApiManager.instance.winRegister.Download(PhotonManager.RoomName);
                    Debug.LogError("im the wager mode winner");
                }
                else if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
                {
                    Debug.LogError("im the tournament winner");
                    ApiManager.instance.tournamentData.SetWinner(PhotonManager.TournamentRoomId, PhotonManager.RoomName, UserDatabase.Instance.localUserData.id);
                }
            }
        }

        [PunRPC]
        private void SendMyRaceFinished(string Myname)
        {
            //  Debug.LogError("New Entry @@@@@@@@@ =" + Myname);


            RankManager.instance.racersOnlineStats.Add(Myname + "^" + bestLapTime + "^" + totalRaceTime + "^" + racerDetails.vehicleName);

            if (PhotonManager.IsGuest)
                RaceUI.instance.ShowRaceResults();
        }

        public void FinishRaceFreeMode()
        {
            Debug.Log("finished called freemode");
            if (finishedRace) return;

            finishedRace = true;

            SendMyRaceFinished(racerDetails.racerName);
            //Player finish
            if (gameObject.tag == "Player")
            {
                RaceManager.instance.EndRace(rank);
                Debug.Log("finished called 1");

                if (rank == 1)
                {

                    //if (GameManager.Instance.currentGameMode == GameMode.CAREER) {
                    //    Debug.Log ("==========================" + PlayerPrefs.GetInt ("TrackIndex") + " ==== " + "Track #" + (PlayerPrefs.GetInt ("TrackIndex") + 2));
                    //    PlayerPrefs.SetInt ("Track #" + (PlayerPrefs.GetInt ("TrackIndex") + 2), 1);
                    //    PlayerPrefs.SetInt ("TrackIndex", PlayerPrefs.GetInt ("TrackIndex") + 1);
                    //    PlayerPrefs.Save ();
                    //} else if (GameManager.Instance.currentGameMode == GameMode.CHAMPIONSHIP) {
                    //    TournamentManager.Instance.SelectedTournament.EndCurrentRace (true); // player won

                    //  if (TournamentManager.Instance.SelectedTournament.isCompleted) {
                    //        RaceManager.instance.Invoke ("ShowTournamentWinCutscene", 5f);
                    //        RaceUI.instance.ShowTournamentCompletePanel (8);
                    //    } else {
                    //        RaceUI.instance.ShowTournamentRaceCompletePanel (3);
                    //        RaceUI.instance.ShowTournamentTreeChart (8);

                    //    }
                    //}

                    //FindObjectOfType<FinishPoint> ().PlayVictoryEffectsParticles ();
                    //FindObjectOfType<RCC_Camera> ().enabled = false;

                    StartCoroutine(RaceEndCutScene()); //hasnain


                    //this.gameObject.AddComponent<RCC_AICarController>(); //hasnain
                    //GetComponent<RCC_AICarController>().nextWaypointPassRadius = 10;

                    CameraManager.instance.ActivateFinishPointCinematicCamera();
                }
                else
                {
                    if (GameController.instance)
                    {
                        GameController.instance.miniMapCamera.gameObject.SetActive(false);
                    }

                    StartCoroutine(RaceEndCutScene());
                    //if (GameManager.Instance.currentGameMode == GameMode.CHAMPIONSHIP) {
                    //    TournamentManager.Instance.SelectedTournament.EndCurrentRace (false); // player lost the race
                    //    TournamentManager.Instance.SelectedTournament.CancelCurrentTournament ();

                    //    RaceUI.instance.ShowTournamentRaceCompletePanel (3);
                    //RaceUI.instance.Invoke ("Exit", 8f);
                    //}
                }

            }

            //foreach (AudioSource audio in gameObject.GetComponentsInChildren<AudioSource>())
            //{
            //    audio.mute = true;
            //}

            //Continue after finishing
            if (RaceManager.instance.continueAfterFinish)
            {
                AIMode();
            }
            else
            {
                RaceManager.instance.DisableRacerInput(gameObject);
            }
        }

        // Switches a player car to an AI controlled car
        public void AIMode()
        {
            if (GetComponent<PlayerControl>())
            {

                GetComponent<PlayerControl>().enabled = false;

                //if (GetComponent<OpponentControl>())
                //{
                //    GetComponent<OpponentControl>().enabled = true;
                //}
                //else
                //{
                //    gameObject.AddComponent<OpponentControl>();
                //}
            }
        }

        // Switches a AI car to a human controlled car
        public void PlayerMode()
        {
            //if (GetComponent<OpponentControl>())
            //{

            //    GetComponent<OpponentControl>().enabled = false;

            //    if (GetComponent<PlayerControl>())
            //    {
            //        GetComponent<PlayerControl>().enabled = true;
            //    }
            //    else
            //    {
            //        gameObject.AddComponent<PlayerControl>();
            //    }
            //}
        }

        void RegisterCheckpoint(Checkpoint.CheckpointType type, float timeAdd)
        {
            if (finishedRace || knockedOut) return;

            switch (type)
            {

                case Checkpoint.CheckpointType.Speedtrap:
                    if (RaceManager.instance._raceType != RaceManager.RaceType.SpeedTrap)
                        return;

                    //add to the racers total speed
                    float speed = 0;

                    if (GetComponent<Car_Controller>())
                        speed = GetComponent<Car_Controller>().currentSpeed;

                    if (GetComponent<Motorbike_Controller>())
                        speed = GetComponent<Motorbike_Controller>().currentSpeed;

                    speedRecord += speed;

                    //play a sound and show info
                    if (gameObject.tag == "Player")
                    {
                        try { SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.speedTrapSound); } catch { }

                        if (RaceManager.instance.showRaceInfoMessages)
                            RaceUI.instance.ShowRaceInfo("+ " + speed + " mph", 1.0f, Color.white);
                    }

                    break;

                case Checkpoint.CheckpointType.TimeCheckpoint:
                    //add our chekpoint
                    checkpoint++;

                    //add to the timer
                    lapTimeCounter += timeAdd;

                    //play a sound and show info
                    if (gameObject.tag == "Player")
                    {
                        try { SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.checkpointSound); } catch { }

                        if (RaceManager.instance.showRaceInfoMessages)
                            RaceUI.instance.ShowRaceInfo("+ " + RaceManager.instance.FormatTime(timeAdd), 1.0f, Color.white);
                    }
                    break;
            }
        }

        void GhostVehicleLogic(bool firstLap, bool beatLastLap)
        {
            return;
            if (!RaceManager.instance.enableGhostVehicle || !gameObject.GetComponent<GhostVehicle>()) return;

            //Always create a ghost and cache values after the first lap
            if (firstLap)
            {
                GetComponent<GhostVehicle>().CacheValues();
                RaceManager.instance.CreateGhostVehicle(gameObject);
            }

            //Create a ghost & cache the values if we beat the last ghost
            if (beatLastLap)
            {
                GetComponent<GhostVehicle>().CacheValues();
                RaceManager.instance.CreateGhostVehicle(gameObject);
            }
            else
            {
                //Use the cached values if we dont beat the last lap
                if (!firstLap)
                {
                    GetComponent<GhostVehicle>().UseCachedValues();
                    RaceManager.instance.CreateGhostVehicle(gameObject);
                }
            }

            //Reset the recorded values
            GetComponent<GhostVehicle>().ClearValues();
        }

        void CheckForBestTime()
        {
            //Best Lap Time
            if (bestLapCounter == 0)
            {
                bestLapCounter = lapTimeCounter;
                bestLapTime = RaceManager.instance.FormatTime(bestLapCounter);
                GhostVehicleLogic(true, false);
            }
            else if (prevLapCounter < bestLapCounter)
            {
                bestLapCounter = prevLapCounter;
                bestLapTime = RaceManager.instance.FormatTime(bestLapCounter);
                GhostVehicleLogic(false, true);
            }
            else if (prevLapCounter > bestLapCounter)
                GhostVehicleLogic(false, false);

            //Save Best Track Lap Time
            if (gameObject.CompareTag("Player"))
            {
                //Set new best
                if (!PlayerPrefs.HasKey("BestTimeFloat" + SceneManager.GetActiveScene().name))
                {
                    PlayerPrefs.SetString("BestTime" + SceneManager.GetActiveScene().name, RaceManager.instance.FormatTime(lapTimeCounter));

                    PlayerPrefs.SetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name, Mathf.Abs(lapTimeCounter));

                    if (RaceManager.instance.showRaceInfoMessages)
                    {
                        RaceUI.instance.ShowRaceInfo("New best time!", 2.0f, Color.white);
                    }
                }

                //Beat our best
                if (PlayerPrefs.GetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name) > lapTimeCounter)
                {
                    PlayerPrefs.SetString("BestTime" + SceneManager.GetActiveScene().name, RaceManager.instance.FormatTime(lapTimeCounter));

                    PlayerPrefs.SetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name, lapTimeCounter);

                    if (RaceManager.instance.showRaceInfoMessages)
                    {
                        RaceUI.instance.ShowRaceInfo("New best time!", 2.0f, Color.white);
                    }
                }
            }
        }

        void CheckForWrongway()
        {
            float nodeAngle = target.transform.eulerAngles.y;

            float transformAngle = transform.eulerAngles.y;

            float angleDifference = nodeAngle - transformAngle;

            //Set wrong way to true after a dealy of 1.0 seconds
            goingWrongway = (wrongwayTimer >= 1.0f);

            if (Mathf.Abs(angleDifference) <= 230f && Mathf.Abs(angleDifference) >= 120)
            {
                if (GetComponent<Rigidbody>().velocity.magnitude >= 5.0f)
                {
                    wrongwayTimer += Time.deltaTime;

                    int n = CurrentNodeNumber;
                    if (n > 1)
                    {
                        Transform rNode = path[n] as Transform;
                        Vector3 rNodeVector = target.InverseTransformPoint(rNode.position);

                        if (rNodeVector.magnitude >= 100)
                        {
                            CurrentNodeNumber--;

                            passednodes[n] = false;

                            if (n != 0)
                                lastPassedNode = path[n - 1];
                            else
                                lastPassedNode = path[path.Count - 1];

                            //  Debug.Log("Mine.." + GetComponent<Photon.Pun.PhotonView>().Controller.NickName);
                            try
                            {
                                if (GetComponent<PhotonView>().IsMine)
                                {
                                    Debug.Log("Mine.." + GetComponent<Photon.Pun.PhotonView>().Controller.NickName);
                                    object[] _data = new object[]
                                    {
                                    CurrentNodeNumber, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber
                                    };
                                    PhotonManager.PhotonRaiseEventsSender_Other(PhotonManager.NodeChanged, _data);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else
                    wrongwayTimer = 0.0f;
            }
            else
                wrongwayTimer = 0.0f;
        }

        void CheckForRespawn()
        {
            if (finishedRace && knockedOut)
                return;

            //incase the car flips over or going wrong way then respawn
            if (transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280 || RaceManager.instance.forceWrongwayRespawn && goingWrongway)
                reviveTimer += Time.deltaTime;
            else
                reviveTimer = 0.0f;

            if (reviveTimer >= 5.0f)
            {
                RaceManager.instance.RespawnRacer(transform, lastPassedNode, 3.0f);
                reviveTimer = 0.0f;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            //Debug.LogError("Finish Trigger Enterd");

/*            if (!PhotonManager.IsGuest)
            {
                if (!this.GetComponent<PhotonView>().IsMine) return;
            }
*/
            //Finish line
            //if (other.CompareTag("FinishLine") || other.CompareTag("Finish"))
            if (other.CompareTag("Finish"))
            {


                Debug.Log("finished");
                if (passedAllNodes)
                {
                    Debug.Log("finished added new lap");
                    NewLap();

                }

                //if(PhotonManager.IsGuest)
                //{
                //    if (currentNodeNumber > 100)
                //        NewLap();

                //    Debug.LogError("current node number " + currentNodeNumber);
                //}

            }

            //Checkpoint
            if (other.GetComponent<Checkpoint>())
            {
                for (int i = 0; i < checkpoints.Count; i++)
                {
                    if (checkpoints[i] == other.transform && !passedcheckpoints[i])
                    {
                        passedcheckpoints[i] = true;

                        RegisterCheckpoint(checkpoints[i].GetComponent<Checkpoint>().checkpointType, checkpoints[i].GetComponent<Checkpoint>().timeToAdd);
                    }
                }
            }
        }

        public Coroutine raceEnded = null;
        IEnumerator RaceEndNodeInc()
        {
            CurrentNodeNumber++;
            while (true)
            {
                yield return new WaitForSeconds(1f);
                CurrentNodeNumber++;
            }
        }

        public void StopRaceEndNodeInc()
        {
            StopCoroutine(raceEnded);
            raceEnded = null;
        }
    }
}