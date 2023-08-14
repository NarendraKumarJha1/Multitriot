using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MMPlayerStatsManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _rank;
    [SerializeField] public RawImage _profile;
    [SerializeField] public TextMeshProUGUI _name;
    [SerializeField] public Image _carStats;
    [SerializeField] public TextMeshProUGUI _lvl;
    [SerializeField] public TextMeshProUGUI _time;
    [SerializeField] public GameObject _spellDeck;
    [SerializeField] public GameObject[] _spellSprites; 
}