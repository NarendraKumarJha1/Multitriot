using System.Collections;
using UnityEngine;

[System.Serializable]
public class SaveData {

	public static SaveData Instance;

	public bool RemoveAds = false;
	public int Level = 1;
	public int Coins = 0;
	// public bool push = false;
	public PlayerSaveableAttributes[] players;
	public string hashOfSaveData;

	//Constructor to save actual GameData
	public SaveData () { }

	//Constructor to check any tampering with the SaveData
	public SaveData (bool ads, int levels, int coins, PlayerSaveableAttributes[] players) {
		RemoveAds = ads;
		Level = levels;
		Coins = coins;
		this.players = players;
	}

}