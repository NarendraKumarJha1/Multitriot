using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMLeaderBoardManager : MonoBehaviour
{
    public static MMLeaderBoardManager instance;

    [SerializeField] public GameObject _spawnPoint;
    [SerializeField] public GameObject _playerStats;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
