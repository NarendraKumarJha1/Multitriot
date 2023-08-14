using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField]
    public float duration=1f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("CarIndex", -1);

        Invoke("LoadMainMenu", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
