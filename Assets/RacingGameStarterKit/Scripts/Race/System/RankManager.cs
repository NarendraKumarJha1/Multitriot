//RankManager.cs handles setting each racer's position/rank
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class RankManager : MonoBehaviour
    {
        public static RankManager instance;

        public List<Statistics> racersStats = new List<Statistics>();

        public List<string> racersOnlineStats = new List<string>();
        int totalRacers;
        public int TotalRacers
        {
            get => totalRacers;
            set
            {
                totalRacers = value;
                OnTotalRacersChanged();
            }
        }

        public bool allRacersReady = false;

        void Awake()
        {
            instance = this;
        }

        void OnTotalRacersChanged()
        {
            Debug.Log("TotalRacers: " + totalRacers);
            if (totalRacers == 1)
            {
                //for (int i = 0; i < racersStats[0].passednodes.Count; i++)
                //    racersStats[0].passednodes[i] = true;

                //turning off for testing
                //racersStats[0].CurrentNodeNumber = racersStats[0].passednodes.Count;
            }
        }

        //Finds the number of racers in the race.
        public void RefreshRacerCount()
        {
            Debug.LogError("To be fixed for knockout match");
            /*totalRacers = racersStats.Count;

            foreach (Statistics _s in racersStats)
            {
                ProgressTracker _pt = _s.GetComponent<ProgressTracker>();
                if (_s.knockedOut == false)
                {
                    ProgressTracker pt = racersProgress.Find(x => x == _pt);

                    if (pt == null)
                        racersProgress.Add(_pt);
                }
                else
                    racersProgress.Remove(_pt);
            }


            //Resize the list
            racerRanks.RemoveRange(totalRacers, racerRanks.Count - totalRacers);

            currentRacers = racersProgress.Count;*/
        }

        public void OnNodeChange()
        {
            if (racersStats.Count <= 0) return;

            bool allFinished = true;
            foreach (Statistics _s in racersStats)
            {
                if (!_s.finishedRace)
                {
                    allFinished = false;
                    break;
                }
            }

            if (allFinished)
            {
                foreach (Statistics _s in racersStats)
                {
                    if (!_s)
                        if (_s.raceEnded != null)
                            _s.StopRaceEndNodeInc();
                }
            }

            racersStats = racersStats.OrderBy(x => x.CurrentNodeNumber).ToList();
            racersStats.Reverse();

            for (int i = 0; i < racersStats.Count; i++)
            {
                if (racersStats[i].finishedRace) continue;
                //changed ranks
                racersStats[i].rank = i + 1;
            }
        }
    }
}
