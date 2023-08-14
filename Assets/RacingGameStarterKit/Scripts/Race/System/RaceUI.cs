//Race_UI.cs handles displaying all UI in the race.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace RGSK
{
    public class RaceUI : MonoBehaviour
    {
        #region Grouped UI Classes

        [System.Serializable]
        public class RacingUI
        {
            public Text rank;
            public Text lap;
            public Text currentLapTime;
            public Text previousLapTime;
            public Text bestLapTime;
            public Text totalTime;
            public Text countdown;
            public Text raceInfo;
            public Text finishedText;
            [Header("Race Start Lights")]
            public GameObject redLight;
            public GameObject yellowLight;
            public GameObject greenLight;

            [Header("In Race Standings")]
            public Transform racersContainer;
            public GameObject racersInfoPrefab;

            public Color playerColor = Color.green;
            public Color normalColor = Color.white;

            [Header("Wrongway Indication")]
            public Image wrongwayImage;
        }

        [System.Serializable]
        public class DriftingUI
        {
            public GameObject driftPanel;
            public Text totalDriftPoints;
            public Text currentDriftPoints;
            public Text driftMultiplier;
            public Text driftStatus;
            public Text goldPoints, silverPoints, bronzePoints;
        }

        [System.Serializable]
        public class DriftResults
        {
            public Text totalPoints;
            public Text driftRaceTime;
            public Text bestDrift;
            public Text longestDrift;
            public Image gold, silver, bronze;
        }

        [System.Serializable]
        public class VehicleUI
        {
            public Text currentSpeed;
            public Text currentGear;
            public Image nitroBar;
            public TextMesh speedText3D, gearText3D;
            private string speedUnit;

            [Header("Speedometer")]
            public RectTransform needle;
            public float minNeedleAngle = -20.0f;
            public float maxNeedleAngle = 220.0f;
            public float rotationMultiplier = 0.85f;
            [HideInInspector]
            public float needleRotation;
        }

        [System.Serializable]
        public class Rewards
        {
            public Text rewardCurrency;
            public Text rewardVehicle;
            public Text rewardTrack;
        }
        #endregion

        public static RaceUI instance;
        [SerializeField] private Statistics player;

        private DriftPointController driftpointcontroller;

        [Header("Starting Grid UI")]
        public GameObject startingGridPanel;
        public Transform startingGridPlayerContainer;
        public GameObject racePlayerInfoPrefab;
        public Text raceWaitTimeText;

        [Header("Racing UI")]
        public GameObject racePanel;
        public GameObject pausePanel;
        public RacingUI racingUI;
        public DriftingUI driftUI;
        public VehicleUI vehicleUI;
        public GameObject VehicleDestroyedMessage;

        [Header("Fail Race UI")]
        public GameObject failRacePanel;
        public Text failTitle;
        public Text failReason;

        [Header("Race Finished UI")]
        public GameObject unlockEverything;
        public GameObject raceCompletePanel;
        public GameObject raceResultsPanel, driftResultsPanel;
        public Text playerRankText;
        public Text playerRankPostfixText;

        public List<RacersResultPrefab> racersResult = new List<RacersResultPrefab>();

        //public List<RacerInfoUI> raceResults = new List<RacerInfoUI>();
        public Text PlayerBestlap;
        public Text PlayerTotaltime;
        public DriftResults driftResults;
        public Rewards rewardTexts;

        [Header("Tournament UI")]
        //public TournamentTree tournamentTreePanel;
        public GameObject tournamentCompletePanel;
        public GameObject tournamentRaceCompletePanel;

        [Header("Replay UI")]
        public GameObject replayPanel;
        public Image progressBar;

        [Header("ScreenFade")]
        public Image screenFade;
        public float fadeSpeed = 0.5f;
        public bool fadeOnStart = true;
        public bool fadeOnExit = true;

        [Header("Scene Ref")]
        public string menuScene = "Menu";

        [HideInInspector]
        public List<string> raceInfos = new List<string>();

        void Awake()
        {
            instance = this;

            ClearUI();
        }

        public void Start()
        {
            if (fadeOnStart && screenFade) StartCoroutine(ScreenFadeOut(fadeSpeed));

            if (GameController.instance && GameController.instance.CurrentPlayer.GetComponent<Statistics>())
                player = GameController.instance.CurrentPlayer.GetComponent<Statistics>();

            ConfigureUiBasedOnRaceType();
            UpdateUIPanels();
        }

        void ClearUI()
        {
            foreach (Transform t in startingGridPlayerContainer) Destroy(t.gameObject);
            foreach (Transform t in racingUI.racersContainer) Destroy(t.gameObject);
            foreach (RacersResultPrefab t in racersResult)
                t.EmptyPrefab();

            PlayerBestlap.text = string.Empty;
            PlayerTotaltime.text = string.Empty;

            //Clear other texts
            if (racingUI.raceInfo) racingUI.raceInfo.text = string.Empty;
            if (racingUI.countdown) racingUI.countdown.text = string.Empty;
            if (racingUI.finishedText) racingUI.finishedText.text = string.Empty;
            if (rewardTexts.rewardCurrency) rewardTexts.rewardCurrency.text = string.Empty;
            if (rewardTexts.rewardVehicle) rewardTexts.rewardVehicle.text = string.Empty;
            if (rewardTexts.rewardTrack) rewardTexts.rewardTrack.text = string.Empty;
        }

        void ConfigureUiBasedOnRaceType()
        {
            if (!RaceManager.instance) return;

            if (driftUI.driftPanel) driftUI.driftPanel.SetActive(RaceManager.instance._raceType == RaceManager.RaceType.Drift);
            if (raceResultsPanel) raceResultsPanel.SetActive(RaceManager.instance._raceType != RaceManager.RaceType.Drift);
            if (driftResultsPanel) driftResultsPanel.SetActive(RaceManager.instance._raceType == RaceManager.RaceType.Drift);

            if (RaceManager.instance._raceType == RaceManager.RaceType.Drift)
            {
                if (driftUI.goldPoints) driftUI.goldPoints.text = RaceManager.instance.goldDriftPoints.ToString("N0");
                if (driftUI.silverPoints) driftUI.silverPoints.text = RaceManager.instance.silverDriftPoints.ToString("N0");
                if (driftUI.bronzePoints) driftUI.bronzePoints.text = RaceManager.instance.bronzeDriftPoints.ToString("N0");
            }
        }

        void Update()
        {
            if (!player) return;

            UpdateUI();
            VehicleGUI();
        }

        void UpdateUI()
        {
            if (!RaceManager.instance) return;

            switch (RaceManager.instance._raceType)
            {
                case RaceManager.RaceType.Circuit:
                case RaceManager.RaceType.LapKnockout:
                case RaceManager.RaceType.SpeedTrap: DefaultUI(); break;

                case RaceManager.RaceType.TimeTrial: TimeTrialUI(); break;

                case RaceManager.RaceType.Checkpoints: CheckpointRaceUI(); break;

                case RaceManager.RaceType.Elimination: EliminationRaceUI(); break;

                case RaceManager.RaceType.Drift: DriftRaceUI(); break;
            }

            switch (RaceManager.instance._raceState)
            {
                case RaceManager.RaceState.StartingGrid:
                    break;

                case RaceManager.RaceState.Racing:
                    ShowInRaceStandings();
                    WrongwayUI();
                    break;

                case RaceManager.RaceState.Complete:
                    if (RaceManager.instance._raceType != RaceManager.RaceType.Drift)
                        ShowRaceResults();
                    else
                        ShowDriftResults();
                    break;

                case RaceManager.RaceState.Replay:
                    ShowReplayUI();
                    break;
            }


            if(Input.GetKeyDown(KeyCode.Q))
            {
                Debug.LogError(AudioListener.pause + " audio listner volume is- " + AudioListener.volume);
                AudioListener.volume = 1;
            }
        }

        #region RaceTypes UI

        void DefaultUI()
        {
            //POS
            if (racingUI.rank)
                racingUI.rank.text = "Pos " + player.rank + "/" + RankManager.instance.TotalRacers;

            //LAP
            if (racingUI.lap)
                racingUI.lap.text = "Lap " + player.lap + "/" + RaceManager.instance.totalLaps;

            //LAP TIME
            if (racingUI.currentLapTime)
                racingUI.currentLapTime.text = "Current " + player.currentLapTime;

            //TOTAL TIME
            if (racingUI.totalTime)
            {
                racingUI.totalTime.text = "Total " + player.totalRaceTime;
                PlayerTotaltime.text = "Total " + player.totalRaceTime; //hasnain
            }

            //LAST LAP TIME
            if (racingUI.previousLapTime)
                racingUI.previousLapTime.text = GetPrevLapTime();

            //BEST LAP TIME
            if (racingUI.bestLapTime)
            {
                racingUI.bestLapTime.text = GetBestLapTime();
                PlayerBestlap.text = GetBestLapTime();
            }
        }

        void TimeTrialUI()
        {
            //POS
            if (racingUI.rank)
                racingUI.rank.text = "Pos " + player.GetComponent<Statistics>().rank + "/" + RankManager.instance.TotalRacers;

            //LAP
            if (racingUI.lap)
                racingUI.lap.text = "Lap " + player.lap;

            //LAP TIME
            if (racingUI.currentLapTime)
                racingUI.currentLapTime.text = "Current " + player.currentLapTime;

            //TOTAL TIME
            if (racingUI.totalTime)
                racingUI.totalTime.text = "Total " + player.totalRaceTime;

            //LAST LAP TIME
            if (racingUI.previousLapTime)
                racingUI.previousLapTime.text = GetPrevLapTime();

            //BEST LAP TIME
            if (racingUI.bestLapTime)
                racingUI.bestLapTime.text = GetBestLapTime();

        }

        void CheckpointRaceUI()
        {
            //POS
            if (racingUI.rank)
                racingUI.rank.text = "Pos " + player.GetComponent<Statistics>().rank + "/" + RankManager.instance.TotalRacers;

            //CHECKPOINTS
            if (racingUI.lap)
                racingUI.lap.text = "CP " + player.checkpoint + "/" + player.checkpoints.Count * RaceManager.instance.totalLaps;

            //TIMER
            if (racingUI.currentLapTime)
                racingUI.currentLapTime.text = "Time : " + player.currentLapTime;

            //BEST LAP TIME
            if (racingUI.bestLapTime)
                racingUI.bestLapTime.text = GetBestLapTime();

            //EMPTY strings
            if (racingUI.previousLapTime)
                racingUI.previousLapTime.text = "";

            if (racingUI.totalTime)
                racingUI.totalTime.text = "";
        }

        void EliminationRaceUI()
        {
            //POS
            if (racingUI.rank)
                racingUI.rank.text = "Pos " + player.GetComponent<Statistics>().rank + "/" + RankManager.instance.TotalRacers;

            //LAP
            if (racingUI.lap)
                racingUI.lap.text = "Lap " + player.lap + "/" + RaceManager.instance.totalLaps;

            //TIMER
            if (racingUI.currentLapTime)
                racingUI.currentLapTime.text = "Time : " + RaceManager.instance.FormatTime(RaceManager.instance.eliminationCounter);

            //TOTAL TIME
            if (racingUI.totalTime)
                racingUI.totalTime.text = "Total " + player.totalRaceTime;

            //LAST LAP
            if (racingUI.previousLapTime)
                racingUI.previousLapTime.text = GetPrevLapTime();

            //BEST LAP
            if (racingUI.bestLapTime)
                racingUI.bestLapTime.text = GetBestLapTime();
        }

        void DriftRaceUI()
        {
            //DRIFT UI
            if (driftUI.totalDriftPoints)
                driftUI.totalDriftPoints.text = player.GetComponent<DriftPointController>().totalDriftPoints.ToString("N0") + " Pts";

            if (driftUI.currentDriftPoints)
                driftUI.currentDriftPoints.text = driftpointcontroller.currentDriftPoints > 0 ? "+ " + player.GetComponent<DriftPointController>().currentDriftPoints.ToString("N0") + " Pts" : string.Empty;

            if (driftUI.driftMultiplier)
                driftUI.driftMultiplier.text = driftpointcontroller.driftMultiplier > 1 ? "x " + driftpointcontroller.driftMultiplier : string.Empty;

            //POS
            if (racingUI.rank)
                racingUI.rank.text = string.Empty;

            //LAP
            if (racingUI.lap)
                racingUI.lap.text = "Lap " + player.lap + "/" + RaceManager.instance.totalLaps;

            //LAP TIME
            if (racingUI.currentLapTime)
                racingUI.currentLapTime.text = "Time " + player.currentLapTime;

            //TOTAL TIME
            if (racingUI.totalTime)
                racingUI.totalTime.text = "Total " + player.totalRaceTime;

            //LAST LAP TIME
            if (racingUI.previousLapTime)
                racingUI.previousLapTime.text = GetPrevLapTime();

            //BEST LAP TIME
            if (racingUI.bestLapTime)
                racingUI.bestLapTime.text = GetBestLapTime();
        }

        #endregion

        void VehicleGUI()
        {
            //Speed
            if (vehicleUI.currentSpeed)
            {
                if (player.GetComponent<Car_Controller>())
                    vehicleUI.currentSpeed.text = player.GetComponent<Car_Controller>().currentSpeed + player.GetComponent<Car_Controller>()._speedUnit.ToString();

                if (player.GetComponent<Motorbike_Controller>())
                    vehicleUI.currentSpeed.text = player.GetComponent<Motorbike_Controller>().currentSpeed + player.GetComponent<Motorbike_Controller>()._speedUnit.ToString();
            }

            //Gear
            if (vehicleUI.currentGear)
            {
                if (player.GetComponent<Car_Controller>())
                    vehicleUI.currentGear.text = player.GetComponent<Car_Controller>().currentGear.ToString();

                if (player.GetComponent<Motorbike_Controller>())
                    vehicleUI.currentGear.text = player.GetComponent<Motorbike_Controller>().currentGear.ToString();
            }

            //Speedometer
            if (vehicleUI.needle)
            {
                float fraction = 0;

                if (player.GetComponent<Car_Controller>())
                {
                    fraction = player.GetComponent<Car_Controller>().currentSpeed / vehicleUI.maxNeedleAngle;
                }

                if (player.GetComponent<Motorbike_Controller>())
                {
                    fraction = player.GetComponent<Motorbike_Controller>().currentSpeed / vehicleUI.maxNeedleAngle;
                }

                vehicleUI.needleRotation = Mathf.Lerp(vehicleUI.minNeedleAngle, vehicleUI.maxNeedleAngle, (fraction * vehicleUI.rotationMultiplier));
                vehicleUI.needle.transform.eulerAngles = new Vector3(vehicleUI.needle.transform.eulerAngles.x, vehicleUI.needle.transform.eulerAngles.y, -vehicleUI.needleRotation);
            }

            //Nitro Bar
            if (vehicleUI.nitroBar)
            {
                if (player.GetComponent<Car_Controller>())
                    vehicleUI.nitroBar.fillAmount = player.GetComponent<Car_Controller>().nitroCapacity;

                if (player.GetComponent<Motorbike_Controller>())
                    vehicleUI.nitroBar.fillAmount = player.GetComponent<Motorbike_Controller>().nitroCapacity;

            }

            //3D text mesh
            if (!vehicleUI.speedText3D && GameObject.Find("3DSpeedText"))
                vehicleUI.speedText3D = GameObject.Find("3DSpeedText").GetComponent<TextMesh>();

            if (!vehicleUI.gearText3D && GameObject.Find("3DGearText"))
                vehicleUI.gearText3D = GameObject.Find("3DGearText").GetComponent<TextMesh>();

            if (vehicleUI.speedText3D)
            {
                if (player.GetComponent<Car_Controller>())
                    vehicleUI.speedText3D.text = player.GetComponent<Car_Controller>().currentSpeed + player.GetComponent<Car_Controller>()._speedUnit.ToString();

                if (player.GetComponent<Motorbike_Controller>())
                    vehicleUI.speedText3D.text = player.GetComponent<Motorbike_Controller>().currentSpeed + player.GetComponent<Motorbike_Controller>()._speedUnit.ToString();
            }

            if (vehicleUI.gearText3D)
            {
                if (player.GetComponent<Car_Controller>())
                    vehicleUI.gearText3D.text = player.GetComponent<Car_Controller>().currentGear.ToString();

                if (player.GetComponent<Motorbike_Controller>())
                    vehicleUI.gearText3D.text = player.GetComponent<Motorbike_Controller>().currentGear.ToString();
            }
        }

        public void UpdateUIPanels()
        {
            if (!RaceManager.instance) return;

            if (RaceManager.instance._raceState == RaceManager.RaceState.Complete)
            {
                if (GameController.instance)
                {
                   // Debug.LogError("Idr winning coin show kena hai");
                    //GameController.instance.uiController.ShowWinningCoins(coinsValue);
                }
                //Debug.LogError("Idr ingame coin dene h racers ko");
                //GameController.instance.uiController.winningCoinsText.GetComponent<DelayedCounter>().StartCounting(0, coinsValue);
            }

            ShowPanel(RaceManager.instance._raceState);
        }

        void ShowPanel(RaceManager.RaceState panel)
        {
            startingGridPanel.SetActive(panel == RaceManager.RaceState.StartingGrid);
            racePanel.SetActive(panel == RaceManager.RaceState.Racing);
            pausePanel.SetActive(panel == RaceManager.RaceState.Paused);
            failRacePanel.SetActive(panel == RaceManager.RaceState.KnockedOut);
            raceCompletePanel.SetActive(panel == RaceManager.RaceState.Complete);
            replayPanel.SetActive(panel == RaceManager.RaceState.Replay);
        }

        public List<RacePlayerInfoPrefabScript> racersUiInfo = new List<RacePlayerInfoPrefabScript>();
        public void ShowStartingGrid()
        {
            //loop through the total number of cars & show their race standings

            foreach (Statistics _statistics in RankManager.instance.racersStats)
            {
                if (_statistics == null) return;

                RacePlayerInfoPrefabScript rpip = Instantiate(racePlayerInfoPrefab, startingGridPlayerContainer).GetComponent<RacePlayerInfoPrefabScript>();
                rpip.gameObject.SetActive(true);
                rpip.pos.text = _statistics.rank.ToString();
                rpip.rName.text = _statistics.RacerDetail.racerName.ToString();
                rpip.vehicle.text = _statistics.RacerDetail.vehicleName.ToString();

                racersUiInfo.Add(rpip);
            }
        }

        void ShowInRaceStandings()
        {
            if (!RankManager.instance.allRacersReady)
                return;

            RankManager.instance.allRacersReady = false;

            foreach (Statistics s in RankManager.instance.racersStats)
            {
                RacerInfoPrefab raceInfo = Instantiate(racingUI.racersInfoPrefab, racingUI.racersContainer).GetComponent<RacerInfoPrefab>();

                raceInfo.nameText.text = s.RacerDetail.racerName;
                raceInfo.posText.text = s.rank.ToString();

                raceInfo.nameText.color = player == s ? racingUI.playerColor : racingUI.normalColor;
                raceInfo.posText.color = player == s ? racingUI.playerColor : racingUI.normalColor;
            }
        }

        public void RefreshInRaceStandings()
        {
            if (RankManager.instance.TotalRacers <= 1) return;

            //Debug.LogError("To be fixed 19/july");
            /*for (int i = 0; i < racingUI.inRaceStandings.Count; i++)
            {
                if (i < RankManager.instance.totalRacers)
                {
                    if (racingUI.inRaceStandings[i].position.transform.parent)
                    {
                        racingUI.inRaceStandings[i].position.transform.parent.gameObject.SetActive(true);

                    }
                }
            }*/
        }

        /// <summary>
        /// Loops through the total number of racers and shows their standings
        /// This function is called for non drift races because of different UI setup
        /// </summary>
        public void ShowRaceResults()
        {
            int rankIndex = 0;
            // RankManager.instance.racersStats = RankManager.instance.racersStats.OrderBy(x => x.rank).ToList();
           // Debug.LogError("Showing results " + RankManager.instance.racersOnlineStats.Count);
            //foreach (Statistics _s in RankManager.instance.racersStats)

            //for (int i = 0; i < RankManager.instance.racersStats.Count; i++)
            //{
                //string pos = RankManager.instance.racersStats[i].rank.ToString();

                //For other Race modes
                /*string name = RaceManager.instance._raceType != RaceManager.RaceType.SpeedTrap? _s.RacerDetail.racerName:
                    _s.RacerDetail.racerName + " [" + RankManager.instance.racerRanks[i].speedRecord + " mph]";*/
                int poss = 0;
            //if (RankManager.instance.racersOnlineStats.Contains(RankManager.instance.racersStats[i].RacerDetail.racerName))
            //{
            foreach (var r in RankManager.instance.racersOnlineStats)
            {
                poss = RankManager.instance.racersOnlineStats.IndexOf(r);

                string[] st = r.Split('^');
                string driver = st[0];
                string vehicle = st[3];

                string bestLap = st[1];
                string totalTime = st[2];
                //if (RankManager.instance.racersStats[i].finishedRace && !RankManager.instance.racersStats[i].knockedOut)
                //{
                //    totalTime = RankManager.instance.racersStats[i].totalRaceTime;
                //    bestLap = totalTime;
                //}
                //else if (RankManager.instance.racersStats[i].knockedOut)
                //    totalTime = "Knocked Out";

                //string bestLap = _s.bestLapTime == string.Empty ? "--:--:--" : _s.bestLapTime;

                // racersResult[i].Init(i, pos, driver, vehicle, bestLap, totalTime);
                racersResult[rankIndex].Init(rankIndex, (poss + 1).ToString(), driver, vehicle, bestLap, totalTime);
                rankIndex++;
            }
                //}

                //else
                //{
                //    string driver = RankManager.instance.racersStats[i].RacerDetail.racerName;
                //    string vehicle = RankManager.instance.racersStats[i].RacerDetail.vehicleName;

                //    string bestLap = "--:--:--";
                //    string totalTime = "Running...";

                //    racersResult[i].Init(poss, (poss + 1).ToString(), driver, vehicle, bestLap, totalTime);
                //}
               
            //}

             for (int i = rankIndex; i < racersResult.Count; i++) racersResult[i].gameObject.SetActive(false);


            ///   int playerRank = RankManager.instance.racersOnlineStats.IndexOf(GameController.instance.CurrentPlayer.GetComponent<Statistics>().RacerDetail.racerName);
            Statistics nam = GameController.instance.CurrentPlayer.GetComponent<Statistics>();
            int playerRank = RankManager.instance.racersOnlineStats.IndexOf(nam.racerDetails.racerName + "^" + nam.bestLapTime + "^" + nam.totalRaceTime + "^" + nam.racerDetails.vehicleName);
            playerRankText.text = (playerRank+1).ToString();
            playerRank++;
            //string prefix = playerRank switch
            //{
            //    1 => "st",
            //    2 => "nd",
            //    3 => "rd",
            //    _ => "th",
            //};

            if (playerRank == 1) playerRankPostfixText.text = "st";
            else if (playerRank == 2) playerRankPostfixText.text = "nd";
            else if (playerRank == 3) playerRankPostfixText.text = "rd";
            else playerRankPostfixText.text = "th";
        }

        /// <summary>
        /// Gets drift information from the driftpointcontroller and displays them
        /// This function is only called for drift races because of different UI setup
        /// </summary>
        void ShowDriftResults()
        {

            if (driftpointcontroller)
            {
                if (driftResults.totalPoints)
                    driftResults.totalPoints.text = "Total Points : " + driftpointcontroller.totalDriftPoints.ToString("N0");

                if (driftResults.driftRaceTime)
                    driftResults.driftRaceTime.text = "Time : " + driftpointcontroller.GetComponent<Statistics>().totalRaceTime;

                if (driftResults.bestDrift)
                    driftResults.bestDrift.text = "Best Drift : " + driftpointcontroller.bestDrift.ToString("N0") + " pts";

                if (driftResults.longestDrift)
                    driftResults.longestDrift.text = "Longest Drift : " + driftpointcontroller.longestDrift.ToString("0.00") + " s";

                if (driftResults.gold)
                    driftResults.gold.gameObject.SetActive(driftpointcontroller.GetComponent<Statistics>().rank == 1);

                if (driftResults.silver)
                    driftResults.silver.gameObject.SetActive(driftpointcontroller.GetComponent<Statistics>().rank == 2);

                if (driftResults.bronze)
                    driftResults.bronze.gameObject.SetActive(driftpointcontroller.GetComponent<Statistics>().rank > 2);
            }
        }

        public void ShowReplayUI()
        {
            //Display the replay progress bar
            if (progressBar)
                progressBar.fillAmount = ReplayManager.instance.ReplayPercent;
        }

        //Used to show useful race info
        public void ShowRaceInfo(string info, float time, Color c)
        {
            StartCoroutine(RaceInfo(info, time, c));
        }

        IEnumerator RaceInfo(string info, float time, Color c)
        {
            if (!racingUI.raceInfo)
                yield break;

            if (racingUI.raceInfo.text == "")
            {
                racingUI.raceInfo.text = info;

                Color col = c;
                col.a = 1.0f;
                racingUI.raceInfo.color = col;

                yield return new WaitForSeconds(time);

                //Do Fade Out
                while (col.a > 0.0f)
                {
                    col.a -= Time.deltaTime * 2.0f;
                    racingUI.raceInfo.color = col;
                    yield return null;
                }

                if (col.a <= 0.01f)
                {
                    racingUI.raceInfo.text = string.Empty;
                }

                //Check if there are any other race infos that need to be displayed
                CheckRaceInfoList();
            }
            else
            {
                raceInfos.Add(info);
            }
        }

        public IEnumerator ShowDriftRaceInfo(string info, Color c)
        {
            if (!driftUI.driftStatus) yield break;

            driftUI.driftStatus.text = info;
            driftUI.driftStatus.color = c;

            yield return new WaitForSeconds(2.0f);

            driftUI.driftStatus.text = string.Empty;
        }

        public void CheckRaceInfoList()
        {
            if (raceInfos.Count > 0)
            {
                ShowRaceInfo(raceInfos[raceInfos.Count - 1], 2.0f, Color.white);
                raceInfos.RemoveAt(raceInfos.Count - 1);
            }
        }

        void WrongwayUI()
        {
            //Wrong way indication
            racingUI.wrongwayImage.enabled = player.GetComponent<Statistics>().goingWrongway;
        }

        string GetPrevLapTime()
        {
            if (string.IsNullOrEmpty(player.prevLapTime))
                return "Last --:--:--";
            else
                return "Last " + player.prevLapTime;
        }

        string GetBestLapTime()
        {
            if (PlayerPrefs.HasKey(string.Format(GenericStringKeys.bestTime, SceneManager.GetActiveScene().name)))
                return "Best " + PlayerPrefs.GetString(string.Format(GenericStringKeys.bestTime, SceneManager.GetActiveScene().name));
            else
                return " ";
        }

        public void SetCountDownText(string value)
        {
            if (!racingUI.countdown) return;

            racingUI.countdown.text = value;

            if (value.Equals(string.Empty))
            {
                if (racingUI.redLight) racingUI.redLight.SetActive(false);
                if (racingUI.yellowLight) racingUI.yellowLight.SetActive(false);
                if (racingUI.greenLight) racingUI.greenLight.SetActive(false);
            }
            else
            {
                if (racingUI.countdown.GetComponent<DG.Tweening.DOTweenAnimation>())
                    racingUI.countdown.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            }
        }

        public void SetFailRace(string title, string reason)
        {
            if (failTitle) failTitle.text = title;

            if (failReason) failReason.text = reason;
        }

        /// <summary>
        /// Gets rid of all other UI apart from the FinishedText to show the "Race Completed" text in the End Race Rountine
        /// </summary>
        public void DisableRacePanelChildren()
        {
            if (!racingUI.finishedText) return;

            RectTransform[] rectTransforms = racePanel.GetComponentsInChildren<RectTransform>();

            foreach (RectTransform t in rectTransforms)
            {
                if (t != racePanel.GetComponent<RectTransform>() && t != racingUI.finishedText.GetComponent<RectTransform>())
                {
                    t.gameObject.SetActive(false);
                }
            }
        }

        public void SetFinishedText(string word)
        {
            if (racingUI.finishedText)
                racingUI.finishedText.text = word;
        }

        public void SetRewardText(string currency, string vehicleUnlock, string trackUnlock)
        {
            if (currency != "" && rewardTexts.rewardCurrency)
            {

            }

            if (vehicleUnlock != "" && rewardTexts.rewardVehicle)
                rewardTexts.rewardVehicle.text = "You Unlocked : " + vehicleUnlock;

            if (trackUnlock != "" && rewardTexts.rewardTrack)
                rewardTexts.rewardTrack.text = "You Unlocked : " + trackUnlock;
        }

        #region Screen Fade
        public IEnumerator ScreenFadeOut(float speed)
        {
            Color col = screenFade.color;
            if (col.a > 0.0f) yield break;

            //Change the alpha to 1
            col.a = 1;
            screenFade.color = col;

            //Fade out
            while (col.a > 0.0f)
            {
                col.a -= Time.deltaTime * speed;
                screenFade.color = col;
                yield return null;
            }
        }

        public IEnumerator ScreenFadeIn(float speed, bool loadScene, string scene)
        {
            //Get the color
            Color col = screenFade.color;

            //Change the alpha to 0
            col.a = 0;
            screenFade.color = col;

            //Fade in
            while (col.a < 1.0f)
            {
                col.a += Time.deltaTime * speed;
                screenFade.color = col;
                yield return null;

                //Load the menu scene when fade completes
                if (col.a >= 1.0f)
                    SceneManager.LoadSceneAsync(scene);
            }

        }
        #endregion

        #region UI Button Functions

        public void PauseResume()
        {
            RaceManager.instance.PauseRace();
        }

        public void Restart()
        {
            //unpause inorder to reset timescale & audiolistener vol
            if (RaceManager.instance._raceState == RaceManager.RaceState.Paused)
            {
                PauseResume();
            }

            if (fadeOnExit && screenFade)
            {
                StartCoroutine(ScreenFadeIn(fadeSpeed * 2, true, SceneManager.GetActiveScene().name));
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void Next()
        {
            int currentLevel = PlayerPrefs.GetInt("TrackIndex");

            Debug.Log("=============================================" + currentLevel);

            if ((currentLevel == 1 || currentLevel % 3 == 0) && !unlockEverythingPopupDisplayed)
            {
                ShowUnlockEverythingPopup();
            }
            else
            {

                Exit();
            }

        }

        public void Exit()
        {
            //unpause inorder to reset timescale & audiolistener vol
            if (RaceManager.instance._raceState == RaceManager.RaceState.Paused)
            {
                PauseResume();
            }

            if (fadeOnExit && screenFade)
            {
                StartCoroutine(ScreenFadeIn(fadeSpeed * 2, true, menuScene));
            }
            else
            {
                SceneManager.LoadSceneAsync(menuScene);
            }
        }

        bool unlockEverythingPopupDisplayed = false;
        public void ShowUnlockEverythingPopup()
        {
            //if (!GSF_InAppController.Instance.CheckEverythingUnlocked ())
            //    unlockEverything.SetActive (true);
            //else
            Next();

            unlockEverythingPopupDisplayed = true;

        }

        //public bool CheckEverythingUnlocked()
        //{
        //    if (SaveData.Instance.Level > GameManager.totalLevelsCount)
        //        return false;
        //    if (!SaveData.Instance.RemoveAds)
        //        return false;

        //    // foreach (PlayerSaveableAttributes vehicle in SaveData.Instance.players) {
        //    // 	if (vehicle.Locked)
        //    // 		return false;
        //    // }

        //    return true;
        //}

        public void ShowTournamentRaceCompletePanel(float delayInSec = 0)
        {
            StartCoroutine(ShowTournamentRaceCompletePanel_Coroutine(delayInSec));
        }
        private IEnumerator ShowTournamentRaceCompletePanel_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            tournamentRaceCompletePanel.SetActive(true);
            if (GameController.instance)
                GameController.instance.SetRccCameraStatus(false);
        }

        public void ShowTournamentCompletePanel(float delayInSec = 0)
        {
            StartCoroutine(ShowTournamentCompletePanel_Coroutine(delayInSec));
        }

        private IEnumerator ShowTournamentCompletePanel_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            tournamentCompletePanel.SetActive(true);
        }

        public void ShowTournamentTreeChart(float delayInSec = 0)
        {
            StartCoroutine(ShowTournamentTreeChart_Coroutine(delayInSec));
        }

        private IEnumerator ShowTournamentTreeChart_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            //tournamentTreePanel.Show ();
        }

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        #endregion
    }
}