//Race_Manager.cs handles the race logic - countdown, spawning cars, asigning racer names, checking race status, formatting time strings and more important race functions.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace RGSK
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager instance;

        #region enum
        public enum RaceType { Circuit, LapKnockout, TimeTrial, SpeedTrap, Checkpoints, Elimination, Drift }
        public enum RaceState { StartingGrid, Racing, Paused, Complete, KnockedOut, Replay }
        public enum TimerType { CountUp, CountDown }
        public RaceType _raceType;
        public RaceState _raceState = RaceState.StartingGrid;
        public TimerType timerType = TimerType.CountUp;
        public enum PlayerSpawnPosition { Randomized, Selected }
        public PlayerSpawnPosition _playerSpawnPosition;
        public enum AISpawnType { Randomized, Order }
        public AISpawnType _aiSpawnType;
        //public OpponentControl.AiDifficulty aiDifficulty = OpponentControl.AiDifficulty.Custom;
        #endregion

        #region int
        public int totalLaps = 3;
        public int totalRacers = 4; //The total number of racers (player included)
        public int countdownFrom = 3;
        private int currentCountdownTime;
        public int playerStartRank = 4;
        #endregion

        #region float
        public float raceDistance; //Your race track's distance.
        public float countdownDelay = 3.0f;
        private float countdownTimer = 0.18f;
        public float initialCheckpointTime = 10.0f; //start time (Checkpoint race);
        public float driftTimeLimit = 60; //time limit (Drift race)
        public float eliminationTime = 30f; //start time (Elimination race);
        public float eliminationCounter; //timer for elimination
        public float ghostAlpha = 0.3f;
        public float goldDriftPoints = 10000;
        public float silverDriftPoints = 5000;
        public float bronzeDriftPoints = 1000;
        #endregion

        #region Transform
        public Transform pathContainer;
        public Transform spawnpointContainer;
        public Transform checkpointContainer;
        public Transform timeTrialStartPoint;
        #endregion

        #region GameObject
        public GameObject playerCar;
        public GameObject playerPointer, opponentPointer, racerName;
        public GameObject activeGhostCar;
        #endregion

        #region List
        public List<GameObject> opponentCars = new List<GameObject>();
        public List<Transform> spawnpoints = new List<Transform>();
        public List<RaceRewards> raceRewards = new List<RaceRewards>();
        public List<string> opponentNamesList = new List<string>();
        public List<Statistics> eliminationList = new List<Statistics>();
        #endregion

        public TextAsset opponentNames;
        public StringReader nameReader;
        public Shader ghostShader;
        public Material ghostMaterial;
        public string playerName = "You";

        #region bool
        private bool startCountdown;
        public bool continueAfterFinish = false; //Should the racers keep driving after finish.
        public bool showRacerNames = true; //Should names appear above player cars
        public bool showRacerPointers = true; //Should minimap pointers appear above all racers
        public bool showRaceInfoMessages = true; //Show final lap indication , new best lap, speed trap & racer knockout information texts
        public bool forceWrongwayRespawn; //should the player get respawned if going the wrong way
        public bool raceStarted; //has the race began
        public bool raceCompleted; //has the race began
        public bool allowDuplicateRacers; //allow duplicate AI
        public bool enableGhostVehicle = true;
        public bool useGhostMaterial = false;
        public bool enableReplay = true;
        public bool autoStartReplay; //automatically start the replay after finishing the race
        public bool showStartingGrid;
        public bool timeTrialAutoDrive = true;
        public bool penalties = true;
        public bool timeLimit;
        public bool assignPlayerName = true;
        public bool loadRacePreferences; //Load menu prefrences?
        public bool assignAiRacerNames = true;

        public GameObject tournamentWinCutscene;

        [SerializeField] GameObject player;

        public string selectedCarName;
        #endregion

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            CacheVariables();

            #region Diff Race Mode
            //Set appropriate racer & lap values according to race type.
            switch (_raceType)
            {
                case RaceType.Circuit:
                    timerType = TimerType.CountUp;
                    if (PhotonManager.IsGuest || BlockchainDataManager.currentGameMode == GameMode.Freemode)
                    {
                        totalRacers = 4;
                        totalLaps = 1;
                    }
                    else
                    {
                        totalRacers = 4;
                        totalLaps = 3;
                    }
                    break;

                /*case RaceType.Sprint:
                    totalLaps = 1;
                    continueAfterFinish = false;
                    timerType = TimerType.CountUp;
                    break;*/

                case RaceType.LapKnockout:
                    //if (totalRacers < 2)
                    //    totalRacers = 2;

                    //totalLaps = totalRacers - 1;
                    //timerType = TimerType.CountUp;
                    break;

                case RaceType.TimeTrial:
                    //totalRacers = 1;
                    //enableReplay = false;
                    //showStartingGrid = false;
                    //timerType = TimerType.CountUp;
                    //if (SoundManager.instance.musicStart == SoundManager.MusicStart.BeforeCountdown)
                    //    SoundManager.instance.musicStart = SoundManager.MusicStart.AfterCountdown;
                    break;

                case RaceType.Checkpoints:
                    //timerType = TimerType.CountDown;
                    break;

                case RaceType.Elimination:
                    //if (totalRacers < 2)
                    //{
                    //    totalRacers = 2;
                    //}
                    //eliminationCounter = eliminationTime;
                    //timerType = TimerType.CountDown;
                    break;

                case RaceType.Drift:
                    //totalRacers = 1;
                    //showStartingGrid = false;
                    //timerType = (timeLimit) ? TimerType.CountDown : TimerType.CountUp;
                    break;
            }
            #endregion

            ConfigureNodes();
            if (!PhotonManager.IsGuest)
            {
                SpawnRacers();
                ///Calling the game register here
                ///
                //Debug.LogError("Room name = "+ PhotonManager.instance.rname)
                PhotonManager.RoomName = (string)PhotonNetwork.CurrentRoom.CustomProperties["Room Name"];
                PhotonManager.RoomAmt = PhotonNetwork.CurrentRoom.CustomProperties["Room EntryFee"].ToString();

                if (PhotonManager.FreeToPlay)
                {
                    //deduct amount from dummy tokens

                    ApiManager.instance.dumTokens.RemoveTokenss(PhotonManager.RoomAmtT.ToString());
                }
                else
                {
                    if (BlockchainDataManager.currentGameMode == GameMode.Wager)
                    {
                        //Turnon later
                          ApiManager.instance.gameRegister.Download(PhotonManager.RoomAmtT.ToString(), PhotonManager.RoomName,"wager");
                    }
                    else if (BlockchainDataManager.currentGameMode == GameMode.Tournament)
                    {
                        ApiManager.instance.tournamentData.JoinTrnmnt(PhotonManager.TournamentRoomId, PhotonManager.RoomName,PhotonManager.instance.IsInventryItemUsed);
                    }
                }
            }
            else
            {
                SpawnRacersFreeMode();
            }


        }

        #region FreeMode
        void SpawnRacersFreeMode()
        {

            if (!playerCar)
            {
                Debug.LogError("Please add a player vehicle!");
                return;
            }

            //Find the children of the spawnpoint container and add them to the spawnpoints List.
            spawnpoints.Clear();

            Transform[] _sp = spawnpointContainer.GetComponentsInChildren<Transform>();

            foreach (Transform point in _sp)
            {
                if (point != spawnpointContainer)
                {
                    spawnpoints.Add(point);
                }
            }

            //Set appropriate values incase they are icnorrectly configured.
            totalRacers = SetValue(totalRacers, spawnpoints.Count);

            playerStartRank = SetValue(playerStartRank, totalRacers);

            totalLaps = SetValue(totalLaps, 1000);

            //Check for player spawn type
            if (_playerSpawnPosition == PlayerSpawnPosition.Randomized)
            {
                playerStartRank = Random.Range(1, totalRacers);
            }

            //Randomize spawn if total racers is greater than AI
            if (totalRacers - 1 > opponentCars.Count)
            {
                _aiSpawnType = AISpawnType.Randomized;

                allowDuplicateRacers = true;
            }

            //Spawn the racers
            for (int i = 0; i < totalRacers; i++)
            {
                if (spawnpoints[i] != spawnpoints[playerStartRank - 1] && opponentCars.Count > 0)
                {
                    //Spawn the AI

                    if (_aiSpawnType == AISpawnType.Randomized)
                    {
                        if (allowDuplicateRacers)
                        {
                            /*Instantiate(opponentCars[Random.Range(0, opponentCars.Count)], spawnpoints[i].position, spawnpoints[i].rotation);
                            var tempai = Instantiate(opponentCars[Random.Range(0, opponentCars.Count)], spawnpoints[i].position, spawnpoints[i].rotation) as GameObject;
                            if (GameController.instance)
                            {
                                GameController.instance.aiCars.Add(tempai);
                            }*/
                        }
                        else
                        {

                            int spawnIndex = Random.Range(0, opponentCars.Count);

                            if (spawnIndex > opponentCars.Count) spawnIndex = opponentCars.Count - 1;

                           /* Instantiate(opponentCars[spawnIndex], spawnpoints[i].position, spawnpoints[i].rotation);
                            tempai = Instantiate(opponentCars[spawnIndex], spawnpoints[i].position, spawnpoints[i].rotation) as GameObject;
                            if (GameController.instance)
                            {
                                GameController.instance.aiCars.Add(tempai);
                            }*/
                            opponentCars.RemoveAt(spawnIndex);
                        }
                    }
                    else if (_aiSpawnType == AISpawnType.Order)
                    {

                        int spawnIndex = 0;

                        if (spawnIndex > opponentCars.Count) spawnIndex = opponentCars.Count - 1;

                        Instantiate(opponentCars[spawnIndex], spawnpoints[i].position, spawnpoints[i].rotation);
                       /* tempai = Instantiate(opponentCars[spawnIndex], spawnpoints[i].position, spawnpoints[i].rotation);
                        if (GameController.instance)
                        {
                            GameController.instance.aiCars.Add(tempai);
                        }*/
                        opponentCars.RemoveAt(spawnIndex);
                    }
                }
                else if (spawnpoints[i] == spawnpoints[playerStartRank - 1] && playerCar)
                {
                    //Spawn the player

                    Transform spawnPos = (_raceType != RaceType.TimeTrial) ? spawnpoints[i] : timeTrialStartPoint;

                    player = (GameObject)Instantiate(Resources.Load("Cars/" + selectedCarName), spawnPos.position, spawnPos.rotation);
                    player.tag = "Player";
                    player.GetComponent<RCC_CameraConfig>().enabled = true;
                    player.GetComponent<VehicleNitro>().enabled = true;
                    player.GetComponent<BarrelRoll>().enabled = true;
                    player.GetComponent<PowerUpsHandler>().enabled = true;
                    Rigidbody rb = player.GetComponent<Rigidbody>();
                    rb.interpolation = RigidbodyInterpolation.Extrapolate;

                    if (GameController.instance)
                    {
                        GameController.instance.CurrentPlayer = player;
                        GameController.instance.rCC_Camera.playerCar = player.GetComponent<RCC_CarControllerV3>();
                    }
                    switch (_raceType)
                    {
                        case RaceType.Drift:
                            if (!player.GetComponent<DriftPointController>())
                                player.AddComponent<DriftPointController>();
                            break;

                        case RaceType.TimeTrial:
                            player.AddComponent<TimeTrialConfig>();

                            if (enableGhostVehicle)
                                player.AddComponent<GhostVehicle>();
                            break;
                    }
                }
            }

            //Set racer names, pointers and handle countdown after spawning the racers
            RankManager.instance.RefreshRacerCount();
            RaceUI.instance.RefreshInRaceStandings();
            SetRacerPreferencesFreeMode();

            //Start the countdown immediately if starting grid isn't shown
           /* if (!showStartingGrid)
            {
                StartCoroutine(Countdown(countdownDelay));
            }
            else
            {
                //Update cameras
                CameraManager.instance.ActivateStartingGridCamera();
            }*/

            //Start the countdown immediately if starting grid isn't shown
            //if (!showStartingGrid)
            //{            //if (!showStartingGrid)
            //{
            Debug.Log("started");
            StartCoroutine(CountdownFreeMode(countdownDelay));
            //}
            //else
            //{
            //    Debug.Log("starteds");
            //    //Update cameras
            //    CameraManager.instance.ActivateStartingGridCamera();
            //}
        }

        void SetRacerPreferencesFreeMode()
        {
            Statistics[] racers = GameObject.FindObjectsOfType(typeof(Statistics)) as Statistics[];

            List<Statistics> racerss = FindObjectsOfType<Statistics>().ToList();
            racerss = racerss.OrderBy(x => x.rank).ToList();

            RankManager.instance.racersStats.AddRange(racerss);
            RankManager.instance.TotalRacers = RankManager.instance.racersStats.Count;

            //Load opponent names if they havent already been loaded
            if (opponentNamesList.Count <= 0)
            {
                LoadRacerNames();
            }

            for (int i = 0; i < racers.Length; i++)
            {

                racers[i].name = ReplaceString(racers[i].name, "(Clone)");

                if (racers[i].gameObject.tag == "Player")
                {

                    //Player Name & Player Minimap Pointer
                    if (assignPlayerName)
                    {
                        racers[i].racerDetails.racerName = playerName;
                    }

                    if (showRacerPointers && playerPointer)
                    {
                        GameObject m_pointer = (GameObject)Instantiate(playerPointer);
                        m_pointer.GetComponent<RacerPointer>().target = racers[i].transform;
                    }
                }
                else
                {

                    //AI Racer Names
                    //if (assignAiRacerNames)
                    //{
                    //    //if (GameManager.Instance.currentGameMode != GameMode.CHAMPIONSHIP) {
                    //    //    int nameIndex = Random.Range (0, opponentNamesList.Count);
                    //    //    if (nameIndex > opponentNamesList.Count) nameIndex = opponentNamesList.Count - 1;
                    //    //    racers[i].racerDetails.racerName = opponentNamesList[nameIndex].ToString ();
                    //    //    opponentNamesList.RemoveAt (nameIndex);
                    //    //} else {
                    //    //    string name = "";
                    //    //    if (!TournamentManager.Instance.SelectedTournament.CurrentRace.racer1.isPlayer)
                    //    //        name = TournamentManager.Instance.SelectedTournament.CurrentRace.racer1.name;
                    //    //    else
                    //    //        name = TournamentManager.Instance.SelectedTournament.CurrentRace.racer2.name;

                    //    //    racers[i].racerDetails.racerName = name;
                    //    //}
                    //}

                    //Ai Racer Name Component
                    if (showRacerNames && racerName)
                    {
                        GameObject _name = (GameObject)Instantiate(racerName);
                        if (_name.GetComponent<RacerName>())
                        {
                            _name.GetComponent<RacerName>().target = racers[i].transform;
                            _name.GetComponent<RacerName>().Initialize();
                        }
                    }

                    //Ai Minimap Pointers
                    if (showRacerPointers && opponentPointer)
                    {
                        GameObject o_pointer = (GameObject)Instantiate(opponentPointer);

                        o_pointer.GetComponent<RacerPointer>().target = racers[i].transform;

                        Debug.Log("Instantiated Pointer");
                    }

                    //Ai Difficulty
                    //  racers[i].gameObject.GetComponent<OpponentControl>().SetDifficulty(aiDifficulty);
                }
            }
        }

        public void LoadRacerNames()
        {
            if (!(TextAsset)Resources.Load("RacerNames", typeof(TextAsset)))
            {
                Debug.Log("Names not found! Please add a .txt file named 'RacerNames' with a list of names to /Resources folder.");
                return;
            }
            int lineCount = 0;
            opponentNames = (TextAsset)Resources.Load("RacerNames", typeof(TextAsset));
            nameReader = new StringReader(opponentNames.text);

            string txt = nameReader.ReadLine();
            while (txt != null)
            {
                lineCount++;
                if (opponentNamesList.Count < lineCount)
                {
                    opponentNamesList.Add(txt);
                }
                txt = nameReader.ReadLine();
            }
        }

        private int SetValue(int val, int otherVal)
        {
            int myVal = val;

            if (val > otherVal)
            {
                myVal = otherVal;
            }
            else if (val <= 0)
            {
                myVal = 1;
            }

            return myVal;
        }

        public IEnumerator CountdownFreeMode(float delay)
        {
            if (_raceState == RaceState.Racing)
                yield break;
            if (_raceType == RaceType.TimeTrial)
                yield break;

            //ScreenFader.Instance.FadeInOut (3f, 0, () => {
            //Set the race state to racing
            SwitchRaceState(RaceState.Racing);
            //});

            //Update cameras
            // CameraManager.instance.ActivatePlayerCamera ();

            //Check whether music should be played now
            if (SoundManager.instance.musicStart == SoundManager.MusicStart.BeforeCountdown)
                SoundManager.instance.StartMusic();

            //wait for (countdown delay) seconds
            yield return new WaitForSeconds(delay);

            //set total countdown time
            currentCountdownTime = countdownFrom + 1;

            startCountdown = true;

            while (startCountdown == true)
            {

                countdownTimer -= Time.deltaTime / 3; // divided by 3 to increase delay b/w countdown 

                if (currentCountdownTime >= 1)
                {
                    if (countdownTimer < 0.01f)
                    {
                        currentCountdownTime -= 1;

                        countdownTimer = 1;

                        if (currentCountdownTime > 0)
                        {
                            RaceUI.instance.SetCountDownText(currentCountdownTime.ToString());
                            if (currentCountdownTime == 3)
                            {
                                RaceUI.instance.SetCountDownText("3");
                                RaceUI.instance.racingUI.countdown.color = Color.red;
                                RaceUI.instance.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_3);
                            }
                            else if (currentCountdownTime == 2)
                            {
                                RaceUI.instance.SetCountDownText("2");
                                RaceUI.instance.racingUI.countdown.color = Color.yellow;
                                RaceUI.instance.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_2);
                            }
                            else if (currentCountdownTime == 1)
                            {
                                RaceUI.instance.SetCountDownText("1");
                                RaceUI.instance.racingUI.countdown.color = Color.green;
                                RaceUI.instance.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_1);
                            }
                            else
                            {
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound);
                            }
                        }
                        CameraManager.instance.SwitchToNextCountdownCamera();
                        // ScreenFader.Instance.FadeInOut (1, 0, () => {

                        // });
                    }
                }
                else
                {
                    //Display GO! and call StartRace();
                    startCountdown = false;

                    //ScreenFader.Instance.FadeInOut (1, 0f, () => {
                    if (GameController.instance)
                    {
                        GameController.instance.SetRccCameraStatus(true);
                    }
                    RaceUI.instance.SetCountDownText("GO!");
                    RaceUI.instance.racingUI.countdown.color = Color.cyan;
                    RaceUI.instance.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                    CameraManager.instance.ActivatePlayerCamera();
                    SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.startRaceSound);

                    // if (player && player.GetComponent<RCC_CarControllerV3> ()) {
                    player.GetComponent<RCC_CarControllerV3>().enabled = true;
                    player.GetComponent<RCC_CarControllerV3>().canControl = true;
                    // }

                    StartRaceFreeMode();
                    //});

                    //Wait for 1 second and hide the text.
                    yield return new WaitForSeconds(2);

                    RaceUI.instance.SetCountDownText(string.Empty);

                    if (GameController.instance/* && totalLaps == 1*/)
                    {

                        if (totalLaps == 1)
                        {
                            GameController.instance.ChangeStartPointText();
                        }
                        else
                        {
                            GameController.instance.HideStartPointText();
                        }

                    }
                }

                yield return null;
            }

            // if (player && player.GetComponent<RCC_CarControllerV3> ()) {
            //     player.GetComponent<RCC_CarControllerV3> ().canControl = true;
            // }
        }

        public void StartRaceFreeMode()
        {
            //enable cars to start racing
            Statistics[] racers = GameObject.FindObjectsOfType(typeof(Statistics)) as Statistics[];

            List<Statistics> racerss = new List<Statistics>(racers);

            foreach (Statistics go in racers)
            {
                if (go.GetComponent<Car_Controller>())
                    go.GetComponent<Car_Controller>().controllable = true;

                if (go.GetComponent<Motorbike_Controller>())
                    go.GetComponent<Motorbike_Controller>().controllable = true;

                if (go.GetComponent<RCC_CarControllerV3>())
                    go.GetComponent<RCC_CarControllerV3>().canControl = true;

                if (_raceType == RaceType.Elimination)
                    eliminationList.Add(go);
                if (GameController.instance)
                    GameController.instance.StartGamePlay();
            }

            //Start replay recording
            if (enableReplay && GetComponent<ReplayManager>())
                GetComponent<ReplayManager>().GetRacersAndStartRecording(racerss);

            //Check whether music should be played now
            if (SoundManager.instance && SoundManager.instance.musicStart == SoundManager.MusicStart.AfterCountdown)
                SoundManager.instance.StartMusic();

            raceStarted = true;
        }
        #endregion

        void CacheVariables()
        {
            if (!pathContainer)
            {
                if (GameController.instance)
                    pathContainer = GameController.instance.currentWayPointPath.transform;
            }
            if (!spawnpointContainer)
                spawnpointContainer = FindObjectOfType<SpawnpointContainer>().transform;
            if (!checkpointContainer)
                checkpointContainer = FindObjectOfType<CheckpointContainer>().transform;
        }

        //Used to calculate track distance(in meters) & rotate the nodes correctly
        void ConfigureNodes()
        {
            Transform[] m_path = pathContainer.GetComponentsInChildren<Transform>();
            List<Transform> m_pathList = new List<Transform>();
            foreach (Transform node in m_path)
            {
                if (node == pathContainer) continue;
                m_pathList.Add(node);
            }

            for (int i = 0; i < m_pathList.Count; i++)
            {
                if (i < m_pathList.Count - 1)
                    m_pathList[i].transform.LookAt(m_pathList[i + 1].transform);
                else
                    m_pathList[i].transform.LookAt(m_pathList[0].transform);
            }

            raceDistance = pathContainer.GetComponent<WaypointCircuit>().distances[pathContainer.GetComponent<WaypointCircuit>().distances.Length - 1];
        }

        void SpawnRacers()
        {
            //Find the children of the spawnpoint container and add them to the spawnpoints List.
            spawnpoints.Clear();

            Transform[] _sp = spawnpointContainer.GetComponentsInChildren<Transform>();

            foreach (Transform point in _sp)
            {
                if (point == spawnpointContainer) continue;
                spawnpoints.Add(point);
            }

            //Set appropriate values incase they are icnorrectly configured.
            totalRacers = PhotonNetwork.CurrentRoom.PlayerCount;

            SpawnGameCar();
        }

        void SpawnGameCar()
        {
            int myIndex = PhotonNetwork.PlayerList.ToList().FindIndex(x => x.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            Transform spawnPos = spawnpoints[myIndex];
            player = PhotonNetwork.Instantiate(Path.Combine("Cars", selectedCarName), spawnPos.position, spawnPos.rotation);
            player.tag = "Player";
            Debug.Log("name of car is = " + Path.Combine("Cars", selectedCarName));
            Hashtable hashtable = new Hashtable()
            {
                {PhotonManager.pCar, selectedCarName},
                {PhotonManager.pSr, (myIndex+1)}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

            if (GameController.instance)
            {
                GameController.instance.CurrentPlayer = player;
                GameController.instance.rCC_Camera.playerCar = player.GetComponent<RCC_CarControllerV3>();
            }

            object[] _data = new object[]
            {
                PhotonNetwork.LocalPlayer.ActorNumber, true
            };
            PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.PlayerReady, _data);




            CameraManager.instance.ActivateStartingGridCamera();
        }

        public void SetRacerPreferences()
        {
            List<Statistics> racers = FindObjectsOfType<Statistics>().ToList();
            racers = racers.OrderBy(x => x.rank).ToList();

            RankManager.instance.racersStats.AddRange(racers);
            RankManager.instance.TotalRacers = RankManager.instance.racersStats.Count;

            foreach (Statistics stats in RankManager.instance.racersStats)
            {
                Player _p = stats.GetComponent<PhotonView>().Controller;
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p != _p) continue;

                    stats.name = stats.name.Replace(stats.name, stats.RacerDetail.racerName);

                    if (showRacerPointers)
                    {
                        if (p.IsLocal)
                        {
                            RacerPointer m_pointer = Instantiate(playerPointer).GetComponent<RacerPointer>();
                            m_pointer.target = stats.transform;
                        }
                        else
                        {
                            RacerPointer o_pointer = Instantiate(opponentPointer).GetComponent<RacerPointer>();
                            o_pointer.target = stats.transform;

                            if (showRacerNames)
                            {
                                GameObject _name = (GameObject)Instantiate(racerName);
                                if (_name.GetComponent<RacerName>())
                                {
                                    _name.GetComponent<RacerName>().target = stats.transform;
                                    _name.GetComponent<RacerName>().Initialize();
                                }
                            }
                        }
                        break;
                    }
                }
            }

            RaceUI.instance.ShowStartingGrid();

            StartCountDown();
        }

        void StartCountDown()
        {
            GameController.instance.gameState = GameState.Waiting;
            StartCoroutine(RaceStartTimer(10));
        }

        IEnumerator RaceStartTimer(int waitTime)
        {
            int sTime = waitTime;
            while (sTime > 3)
            {
                string timerMsg = string.Format(GenericStringKeys.RaceWaitTimer, sTime);

                object[] _data = new object[] { timerMsg };
                PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.RaceCountdownTimer, _data);

                yield return new WaitForSeconds(1f);

                sTime--;
            }

            PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.StartRace, null);
        }

        public void StartRaceCountdown()
        {
            StartCoroutine(Countdown());
            if (GameController.instance)
            {
                GameController.instance.SetRccCameraStatus(false); //hasnain
                GameController.instance.PauseBtn.SetActive(true);
            }
        }

        IEnumerator Countdown()
        {
            if (_raceState == RaceState.Racing)
                yield break;
            if (_raceType == RaceType.TimeTrial)
                yield break;

            SwitchRaceState(RaceState.Racing);

            //Check whether music should be played now
            if (SoundManager.instance.musicStart == SoundManager.MusicStart.BeforeCountdown)
                SoundManager.instance.StartMusic();

            //wait for (countdown delay) seconds
            yield return new WaitForEndOfFrame();

            //set total countdown time
            currentCountdownTime = countdownFrom + 1;

            startCountdown = true;
            RaceUI raceUI = RaceUI.instance;
            while (startCountdown)
            {
                countdownTimer -= Time.deltaTime / 3; // divided by 3 to increase delay b/w countdown 

                if (currentCountdownTime >= 1)
                {
                    if (countdownTimer < 0.01f)
                    {
                        currentCountdownTime -= 1;

                        countdownTimer = 1;

                        if (currentCountdownTime > 0)
                        {
                            raceUI.SetCountDownText(currentCountdownTime.ToString());
                            if (currentCountdownTime == 3)
                            {
                                raceUI.SetCountDownText("3");
                                raceUI.racingUI.countdown.color = Color.red;
                                raceUI.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_3);
                            }
                            else if (currentCountdownTime == 2)
                            {
                                raceUI.SetCountDownText("2");
                                raceUI.racingUI.countdown.color = Color.yellow;
                                raceUI.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_2);
                            }
                            else if (currentCountdownTime == 1)
                            {
                                raceUI.SetCountDownText("1");
                                raceUI.racingUI.countdown.color = Color.green;
                                raceUI.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound_1);
                            }
                            else
                            {
                                SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.countdownSound);
                            }
                        }
                        CameraManager.instance.SwitchToNextCountdownCamera();
                    }
                }
                else
                {
                    //Display GO! and call StartRace();
                    startCountdown = false;

                    //ScreenFader.Instance.FadeInOut (1, 0f, () => {
                    if (GameController.instance)
                    {
                        GameController.instance.SetRccCameraStatus(true);
                    }
                    RaceUI.instance.SetCountDownText("GO!");
                    RaceUI.instance.racingUI.countdown.color = Color.cyan;
                    RaceUI.instance.racingUI.countdown.GetComponent<DOTweenAnimation>().DORestart();
                    CameraManager.instance.ActivatePlayerCamera();
                    SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.startRaceSound);

                    // if (player && player.GetComponent<RCC_CarControllerV3> ()) {
                    //     player.GetComponent<RCC_CarControllerV3> ().canControl = true;
                    // }

                    if (player && player.GetComponent<RCC_CarControllerV3>())
                    {
                        player.GetComponent<Statistics>().CurrentNodeNumber = 0;
                    }

                    StartRace(); ///$ree
                    //});

                    //Wait for 1 second and hide the text.
                    yield return new WaitForSeconds(2);

                    RaceUI.instance.SetCountDownText(string.Empty);

                    if (GameController.instance/* && totalLaps == 1*/)
                    {
                        if (GameController.instance.levelToLoad == LevelToLoad.Futuristic ||
                            GameController.instance.levelToLoad == LevelToLoad.Lava ||
                                GameController.instance.levelToLoad == LevelToLoad.Shoefy)
                        {
                            if (totalLaps == 1)
                                GameController.instance.ChangeStartPointText();
                            else
                                GameController.instance.HideStartPointText();
                        }
                    }
                }

                yield return null;
            }
        }

        void Update()
        {
            //Handle Elimination race times
            if (_raceType == RaceType.Elimination)
                CalculateEliminationTime();
        }

        public void StartRace()
        {
            //enable cars to start racing
            List<Statistics> racers = RankManager.instance.racersStats;

            foreach (Statistics go in racers)
            {
                if (go.GetComponent<Car_Controller>())
                    go.GetComponent<Car_Controller>().controllable = true;

                if (go.GetComponent<Motorbike_Controller>())
                    go.GetComponent<Motorbike_Controller>().controllable = true;

                if (go.GetComponent<RCC_CarControllerV3>())
                    go.GetComponent<RCC_CarControllerV3>().canControl = true;

                if (_raceType == RaceType.Elimination)
                    eliminationList.Add(go);
                if (GameController.instance)
                    GameController.instance.StartGamePlay();
            }

            //Start replay recording
            if (enableReplay && GetComponent<ReplayManager>())
                GetComponent<ReplayManager>().GetRacersAndStartRecording(racers);

            //Check whether music should be played now
            if (SoundManager.instance && SoundManager.instance.musicStart == SoundManager.MusicStart.AfterCountdown)
                SoundManager.instance.StartMusic();

            raceStarted = true;

            GameController.instance.gameState = GameState.Start;
        }

        Coroutine endRaceCoroutine;
        public void EndRace(int rank)
        {
            GameController.instance.gameState = GameState.End;
            Debug.Log("end race called");
            endRaceCoroutine = StartCoroutine(EndRaceRoutine());

            raceCompleted = true;

            if (ReplayManager.instance)
                ReplayManager.instance.StopRecording();
        }

        IEnumerator EndRaceRoutine()
        {
            Debug.Log("completed called");
            RaceUI.instance.DisableRacePanelChildren();

            RaceUI.instance.SetFinishedText("RACE COMPLETED");

            if (autoStartReplay)
                AutoStartReplay();

            yield return new WaitForSecondsRealtime(2f);
            SwitchRaceState(RaceState.Complete);
            if (endRaceCoroutine != null)
                StopCoroutine(endRaceCoroutine);
            //GSF_AdsManager.ShowInterstitial (gameplaySequenceId, "Gameplay");
        }

        public void PauseRace()
        {
            //No point for pausing in completed or starting grid states
            if (raceCompleted || _raceState == RaceState.StartingGrid) return;

            if (_raceState == RaceState.Paused)
            {
                //Handle un-pausing
                SwitchRaceState(RaceState.Racing);

                // clear the countdown text (Bug Fix)
                RaceUI.instance.racingUI.countdown.text = string.Empty;

                Time.timeScale = 1.0f;

                SoundManager.instance.SetVolume();

            }
            else
            {

                //Handle pausing
                SwitchRaceState(RaceState.Paused);

                Time.timeScale = 0.0f;

               // AudioListener.volume = 0.0f; //$ree

                //GSF_AdsManager.ShowInterstitial (gameplaySequenceId, "Gameplay");
            }
        }

        void CalculateEliminationTime()
        {
            if (!raceStarted || _raceState == RaceState.Complete) return;

            eliminationCounter -= Time.deltaTime;

            if (eliminationCounter <= 0)
            {
                eliminationCounter = eliminationTime;

                if (RankManager.instance.TotalRacers > 1) { KnockoutRacer(GetLastPlace()); }

                //end the race after all opponent racers have been eliminated
                AllOpponentsEliminated();
            }
        }

        //Used to knockout a racer
        public void KnockoutRacer(Statistics racer)
        {

            racer.knockedOut = true;

            if (racer.CompareTag("Player"))
            {
                SwitchRaceState(RaceState.KnockedOut);

                racer.AIMode();

                //RaceUI Fail Race Panel Config
                string title = (_raceType == RaceType.Elimination) ? "ELIMINATED" : (_raceType == RaceType.LapKnockout) ? "KNOCKED OUT" : "TIMED OUT";
                string reason = (_raceType == RaceType.Elimination) ? "You were eliminated from the race." : (_raceType == RaceType.LapKnockout) ? "You were knocked out of the race." : "You ran out of time.";
                RaceUI.instance.SetFailRace(title, reason);

                //FirebaseAnalytics.Event ("level_failed_" + (PlayerPrefs.GetInt ("TrackIndex") + 1), "level_failed_" + (PlayerPrefs.GetInt ("TrackIndex") + 1), "failed");

                //Stop Recording
                if (ReplayManager.instance) { ReplayManager.instance.StopRecording(); }
            }

            if (showRaceInfoMessages)
            {
                string keyword = (_raceType == RaceType.Elimination) ? " eliminated." : (_raceType == RaceType.LapKnockout) ? " knocked out." : " timed out.";

                RaceUI.instance.ShowRaceInfo(racer.RacerDetail.racerName + keyword, 2.0f, Color.white);
            }

            ChangeLayer(racer.transform, "IgnoreCollision");

            RankManager.instance.RefreshRacerCount();
        }

        //Creates an active ghost car
        public void CreateGhostVehicle(GameObject racer)
        {
            //Destroy any active ghost
            if (activeGhostCar)
            {
                Destroy(activeGhostCar);
            }

            //Create a duplicate ghost car
            GameObject ghost = (GameObject)Instantiate(racer, Vector3.zero, Quaternion.identity);

            ghost.name = "Ghost";

            ghost.tag = "Untagged";

            activeGhostCar = ghost;

            ChangeLayer(ghost.transform, "IgnoreCollision");

            ChangeMaterial(ghost.transform);

            DisableRacerInput(ghost);

            ghost.GetComponent<GhostVehicle>().StartGhost();
        }

        //Format a float to a time string
        public string FormatTime(float time)
        {
            int minutes = (int)Mathf.Floor(time / 60);
            int seconds = (int)time % 60;
            int milliseconds = (int)(time * 100) % 100;

            return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }

        //used to respawn a racer
        public void RespawnRacer(Transform racer, Transform node, float ignoreCollisionTime)
        {
            if (raceStarted)
                StartCoroutine(Respawn(racer, node, ignoreCollisionTime));
        }

        IEnumerator Respawn(Transform racer, Transform node, float ignoreCollisionTime)
        {
            //Flip the car over and place it at the last passed node
            racer.rotation = Quaternion.LookRotation(racer.forward);
            racer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            racer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            racer.position = new Vector3(node.position.x, node.position.y + 0.1f, node.position.z);
            racer.rotation = node.rotation;

            ChangeLayer(racer, "IgnoreCollision");
            yield return new WaitForSeconds(ignoreCollisionTime);
            ChangeLayer(racer, "Default");
        }

        //used to change a racers layer to "ignore collision" after being knocked out & on respawn
        public void ChangeLayer(Transform racer, string LayerName)
        {
            for (int i = 0; i < racer.childCount; i++)
            {
                racer.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerName);
                ChangeLayer(racer.GetChild(i), LayerName);
            }
        }

        //used to change a racers material when creating a ghost car
        public void ChangeMaterial(Transform racer)
        {
            Transform[] m = racer.GetComponentsInChildren<Transform>();

            foreach (Transform t in m)
            {
                if (t.GetComponent<Renderer>())
                {
                    //If the vehicle only uses one material
                    if (t.GetComponent<Renderer>().materials.Length == 1)
                    {
                        if (!useGhostMaterial)
                        {
                            Material instance = t.gameObject.GetComponent<Renderer>().material;
                            instance.shader = (ghostShader) ? ghostShader : Shader.Find("Transparent/Diffuse");
                            Color col = instance.color;
                            col.a = ghostAlpha;
                            instance.color = col;
                            t.gameObject.GetComponent<Renderer>().material = instance;
                        }
                        else
                        {
                            t.gameObject.GetComponent<Renderer>().material = ghostMaterial;
                        }

                    }
                    else
                    {
                        //If the vehicle uses more than one material
                        Material[] instances = new Material[t.GetComponent<Renderer>().materials.Length];
                        Color[] col = new Color[t.GetComponent<Renderer>().materials.Length];

                        for (int i = 0; i < instances.Length; i++)
                        {
                            if (!useGhostMaterial)
                            {
                                instances[i] = t.gameObject.GetComponent<Renderer>().materials[i];
                                instances[i].shader = ghostShader;
                                col[i] = instances[i].color;
                                col[i].a = ghostAlpha;
                                instances[i].color = col[i];
                                t.gameObject.GetComponent<Renderer>().materials[i] = instances[i];
                            }
                            else
                            {
                                instances[i] = ghostMaterial;
                                t.gameObject.GetComponent<Renderer>().materials = instances;
                            }
                        }
                    }
                }
            }
        }

        //Used to disable input for when viewing a replay or for a ghost car
        public void DisableRacerInput(GameObject racer)
        {
            if (racer.GetComponent<PlayerControl>())
                racer.GetComponent<PlayerControl>().enabled = false;

            //if (racer.GetComponent<OpponentControl>())
            //    racer.GetComponent<OpponentControl>().enabled = false;

            if (racer.GetComponent<Car_Controller>()) //
                racer.GetComponent<Car_Controller>().enabled = false;

            if (!racer.GetComponent<Statistics>().finishedRace)
                racer.GetComponent<Statistics>().finishedRace = true;
        }

        public void SwitchRaceState(RaceState state)
        {
            _raceState = state;

            //Update UI
            RaceUI.instance.UpdateUIPanels();

        }

        /// <summary>
        /// Automatically starts the reply by manually setting the appropriate values
        /// </summary>
        void AutoStartReplay()
        {
            if (ReplayManager.instance.TotalFrames <= 0) return;

            StartCoroutine(RaceUI.instance.ScreenFadeOut(0.5f));

            _raceState = RaceState.Replay;

            ReplayManager.instance.replayState = ReplayManager.ReplayState.Playing;

            CameraManager.instance.ActivateCinematicCamera();

            for (int i = 0; i < ReplayManager.instance.racers.Count; i++)
            {
                DisableRacerInput(ReplayManager.instance.racers[i].racer.gameObject);
            }
        }

        // Checks if all racers have finished
        public bool AllRacersFinished()
        {
            bool allFinished = false;

            Statistics[] allRacers = GameObject.FindObjectsOfType(typeof(Statistics)) as Statistics[];

            for (int i = 0; i < allRacers.Length; i++)
            {
                if (allRacers[i].finishedRace)
                    allFinished = true;
                else
                    allFinished = false;
            }

            return allFinished;
        }

        void AllOpponentsEliminated()
        {

            for (int i = 0; i < eliminationList.Count; i++)
            {

                if (eliminationList[i].knockedOut)
                {
                    eliminationList.Remove(eliminationList[i]);
                }

                if (eliminationList.Count == 1 && eliminationList[0].gameObject.tag == "Player")
                {
                    eliminationList[0].FinishRace();
                }
            }
        }

        public Statistics GetLastPlace()
        {
            return RankManager.instance.racersStats[RankManager.instance.racersStats.Count - 1];
        }

        string ReplaceString(string stringValue, string toRemove)
        {
            return stringValue.Replace(toRemove, "");
        }

        public void SetPlayerNitro(float value)
        {
            if (player)
            {
                //player.GetComponent<BikeControl> ().powerShift = value;
            }
        }

        public void ShowTournamentWinCutscene()
        {
            tournamentWinCutscene.SetActive(true);
        }
    }
}