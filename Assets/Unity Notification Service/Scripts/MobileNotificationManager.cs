/*using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

[System.Serializable]
public class NotificationData {
    public string Title = "Test Notification";
    public string Text = "This is a test notification!";
    public string SmallIconIdentifier = "app_icon_small";
    public string LargeIconIdentifier = "app_icon_large";
}

public class MobileNotificationManager : MonoBehaviour {

    public AndroidNotificationChannel defaultNotificationChannel;
    [Header ("Deafult Notification")]
    public NotificationData[] defaultNotifications;
    public int fireRateInSec = 86400;
    [Header ("Game Paused")]
    public NotificationData pauseGameNotification;

    //private int identifier;

    private void Start () {

        ScheduleNewNotification ();
        // defaultNotificationChannel = new AndroidNotificationChannel () {
        //     Id = "default_channel",
        //     Name = "Default Channel",
        //     Description = "For Generic notifications",
        //     Importance = Importance.Default,
        // };

        // AndroidNotificationCenter.RegisterNotificationChannel (defaultNotificationChannel);

        // int i = Random.Range (0, defaultNotifications.Length);

        // AndroidNotification notification = new AndroidNotification () {
        //     Title = defaultNotifications[i].Title,
        //     Text = defaultNotifications[i].Text,
        //     SmallIcon = defaultNotifications[i].SmallIconIdentifier,
        //     LargeIcon = defaultNotifications[i].LargeIconIdentifier,
        //     FireTime = System.DateTime.Now.AddSeconds (fireRateInSec),
        // };

        // if (PlayerPrefs.HasKey ("NOTIF_ID")) {
        //     // Replace the currently scheduled notification with a new notification.
        //     AndroidNotificationCenter.UpdateScheduledNotification (PlayerPrefs.GetInt ("NOTIF_ID"), notification, "default_channel");
        //     return;
        // }

        // int identifier = AndroidNotificationCenter.SendNotification (notification, "default_channel");
        // PlayerPrefs.SetInt ("NOTIF_ID", identifier);
        // PlayerPrefs.Save ();

        // AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler = delegate (AndroidNotificationIntentData data) {
        //     var msg = "Notification received : " + data.Id + "\n";
        //     msg += "\n Notification received: ";
        //     msg += "\n .Title: " + data.Notification.Title;
        //     msg += "\n .Body: " + data.Notification.Text;
        //     msg += "\n .Channel: " + data.Channel;
        //     Debug.Log (msg);
        // };

        // AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;

        // var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent ();

        // if (notificationIntentData != null) {
        //     Debug.Log ("App was opened with notification!");
        // }

    }

    private void OnApplicationPause (bool pause) {
        // int id = PlayerPrefs.GetInt ("NOTIF_ID");
        // if (pause && AndroidNotificationCenter.CheckScheduledNotificationStatus (id) == NotificationStatus.Scheduled) {
        //     //If the player has left the game and the game is not running. Send them a new notification
        //     AndroidNotification newNotification = new AndroidNotification () {
        //     Title = pauseGameNotification.Title,
        //     Text = pauseGameNotification.Text,
        //     SmallIcon = pauseGameNotification.SmallIconIdentifier,
        //     LargeIcon = pauseGameNotification.LargeIconIdentifier,
        //     FireTime = System.DateTime.Now
        //     };

        //     // Replace the currently scheduled notification with a new notification.
        //     AndroidNotificationCenter.UpdateScheduledNotification (id, newNotification, "default_channel");
        // } else if (AndroidNotificationCenter.CheckScheduledNotificationStatus (id) == NotificationStatus.Delivered) {
        //     //Remove the notification from the status bar
        //     AndroidNotificationCenter.CancelNotification (id);
        //     PlayerPrefs.DeleteKey ("NOTIF_ID");

        //     // int i = Random.Range (0, defaultNotifications.Length);

        //     // AndroidNotification notification = new AndroidNotification () {
        //     //     Title = defaultNotifications[i].Title,
        //     //     Text = defaultNotifications[i].Text,
        //     //     SmallIcon = defaultNotifications[i].SmallIconIdentifier,
        //     //     LargeIcon = defaultNotifications[i].LargeIconIdentifier,
        //     //     FireTime = System.DateTime.Now.AddSeconds (fireRateInSec),
        //     // };

        //     ScheduleNewNotification ();

        // } else if (AndroidNotificationCenter.CheckScheduledNotificationStatus (id) == NotificationStatus.Unknown) {
        //     // AndroidNotification notification = new AndroidNotification () {
        //     // Title = defaultNotification.Title,
        //     // Text = defaultNotification.Text,
        //     // SmallIcon = defaultNotification.SmallIconIdentifier,
        //     // LargeIcon = defaultNotification.LargeIconIdentifier,
        //     // FireTime = System.DateTime.Now.AddSeconds (fireRateInSec),
        //     // };

        //     // identifier = AndroidNotificationCenter.SendNotification (notification, "default_channel");

        //     //Try sending it again
        //     // int i = Random.Range (0, defaultNotifications.Length);

        //     // AndroidNotification notification = new AndroidNotification () {
        //     //     Title = defaultNotifications[i].Title,
        //     //     Text = defaultNotifications[i].Text,
        //     //     SmallIcon = defaultNotifications[i].SmallIconIdentifier,
        //     //     LargeIcon = defaultNotifications[i].LargeIconIdentifier,
        //     //     FireTime = System.DateTime.Now.AddSeconds (fireRateInSec),
        //     // };
        //     ScheduleNewNotification ();
        // }

    }

    public void ScheduleNewNotification () {
        defaultNotificationChannel = new AndroidNotificationChannel () {
            Id = "default_channel",
            Name = "Default Channel",
            Description = "For Generic notifications",
            Importance = Importance.Default,
        };

        AndroidNotificationCenter.RegisterNotificationChannel (defaultNotificationChannel);

        int i = Random.Range (0, defaultNotifications.Length);

        AndroidNotification notification = new AndroidNotification () {
            Title = defaultNotifications[i].Title,
            Text = defaultNotifications[i].Text,
            SmallIcon = defaultNotifications[i].SmallIconIdentifier,
            LargeIcon = defaultNotifications[i].LargeIconIdentifier,
            FireTime = System.DateTime.Now.AddSeconds (fireRateInSec),
        };

        if (PlayerPrefs.HasKey ("NOTIF_ID") && AndroidNotificationCenter.CheckScheduledNotificationStatus (PlayerPrefs.GetInt ("NOTIF_ID")) == NotificationStatus.Scheduled) {
            // Replace the currently scheduled notification with a new notification.
            AndroidNotificationCenter.UpdateScheduledNotification (PlayerPrefs.GetInt ("NOTIF_ID"), notification, "default_channel");
            return;
        }

        int identifier = AndroidNotificationCenter.SendNotification (notification, "default_channel");
        PlayerPrefs.SetInt ("NOTIF_ID", identifier);
        PlayerPrefs.Save ();

        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler = delegate (AndroidNotificationIntentData data) {
            var msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            Debug.Log (msg);
            Debug.Log ("SCHEDULING NEW NOTIFICATION");

            ScheduleNewNotification ();

        };

        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;

        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent ();

        if (notificationIntentData != null) {
            Debug.Log ("App was opened with notification!");
        }
    }

}*/