using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentWindow : MonoBehaviour
{

    public string _id;
    public string userId;
    public string joiningamount;
    public string roomId;
    public string status;
    public string tournamentId;
    public string numberofplayers;
    public string nameoftournament;
    public string starttiming;
    public string Endtiming;
    public string MapName;


    public Text T_Name;
    public Text T_map_Name;
    public Text T_players;
    public Image T_Image;
    public Text T_Time;
    public Button PlayButton;
    public bool startTimer;


    //public string StartTime = "2022/11/12 01:24:00";
    //public string EndTime = "2022/11/12 02:24:00";
    public void SetInfo()
    {
        T_Name.text = "Tournament ID: " + tournamentId; // nameoftournament;
        T_Time.text = "Time: " + starttiming;
        T_map_Name.text = "Map name: " + MapName;
        T_players.text = "Players: " + numberofplayers;
        startTimer = true;

        PlayButton.onClick.AddListener(() => setButton());
    }

    public TimeSpan ts;
    //public TimeSpan Endts;
    private void Update()
    {
        if (startTimer)
        {
            DateTime st = DateTime.Parse(starttiming);    //DateTime.Parse(starttiming);
            DateTime et = DateTime.Parse(Endtiming);  //DateTime.Parse(Endtiming);

            DateTime now = DateTime.Now;
            // Calculate the interval between the two dates.
            // 

            ts = st - now;
            //Endts = et - now;

            T_Time.text = "Game starts in : " + ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");

            TimeSpan runningTime = et.Subtract(now);


            if (st < DateTime.Now && et > DateTime.Now)
            {
                //game is in progress
                T_Time.text = "Game Remaining time : " + runningTime.Hours.ToString("00") + ":" + runningTime.Minutes.ToString("00") + ":" + runningTime.Seconds.ToString("00");
                PlayButton.GetComponentInChildren<Text>().text = "Play now!";
                PlayButton.interactable = true;

            }
            else if (et < DateTime.Now)
            {
                //game ended
                PlayButton.GetComponentInChildren<Text>().text = "Ended";
                T_Time.text = "Game starts in : " + "--" + ":" + "--" + ":" + "--";
                PlayButton.interactable = false;
            }
            else
            {
                //starting soon
                PlayButton.GetComponentInChildren<Text>().text = "Starts soon!";
                PlayButton.interactable = false;
            }

        }
    }


    private void setButton()
    {
        // PhotonManager.RoomAmt = _id;
        // PhotonManager.RoomAmtT = Convert.ToInt32(joiningamount);
        PhotonManager.TournamentRoomId = tournamentId;

        switch (MapName)
        {
            case "futuristic":
                EnvironmentSelectionHandler.Instance.SelectLevel(0);
                break;

            case "spacelava":
                EnvironmentSelectionHandler.Instance.SelectLevel(2);
                break;

            case "neon":
                EnvironmentSelectionHandler.Instance.SelectLevel(1);
                break;

            default:
                EnvironmentSelectionHandler.Instance.SelectLevel(0);
                break;

        }
        EnvironmentSelectionHandler.Instance.PlayLevel(1);
    }



}
