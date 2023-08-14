using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK;
using UnityEngine.SceneManagement;

public class SceneActive : MonoBehaviour
{
    public CheckpointContainer checkpointContainer = null;
    public string sceneName = "None";
    private void Awake()
    {
        //if (GameController.instance)
        //    GameController.instance.checkpointContainer = checkpointContainer;
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
