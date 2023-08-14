using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReviveOfferWall : MonoBehaviour {
    public GSF_GameController gameController;
    public Image timerProgressBar;
    public Text timerCountDown;
    public float lifetimeOfOfferwall = 5f;

    [Header ("UI Panels")]
    public GameObject OfferWall;

    [Header ("Sounds")]
    public AudioSource btnClick;

    [Header ("Ad Sequence ID")]
    public int SequenceID;
    public UnityEvent OnRewardedVideoComplete;

    void OnEnable () {
        //ConsoliAds.onRewardedVideoAdCompletedEvent += RewardedVideoCompleted;
    }

    void OnDisable () {
        //ConsoliAds.onRewardedVideoAdCompletedEvent -= RewardedVideoCompleted;
    }

    void Start () {
        //ConsoliAds.Instance.LoadRewarded (SequenceID);
    }

    private void StartLifetimer () {
        // isSkipped = false;
        timerProgressBar.fillAmount = 1f;
        Tweener t = timerProgressBar.DOFillAmount (0f, lifetimeOfOfferwall);
        t.OnUpdate (() => {
            timerCountDown.text = (lifetimeOfOfferwall - (int) t.Elapsed ()).ToString ();
        });
        t.OnComplete (() => {
            Time.timeScale = 1;
            gameObject.SetActive (false);
            GameManager.Instance.GameLoose (1);

        });
    }

    public void ShowRewardedVideo () {
        timerProgressBar.DOKill (false); // destroy dotween...
        //GSF_AdsManager.ShowRewardedVideo (SequenceID, "Rewarded Video");
        //ConsoliAds.Instance.LoadRewarded (SequenceID);
    }

    void RewardedVideoCompleted () {
        Time.timeScale = 1;
        OnRewardedVideoComplete.AddListener (() => {
            gameController.RepositionPlayer ();
        });
        OnRewardedVideoComplete.Invoke ();
        OfferWall.SetActive (false);
        //ConsoliAds.Instance.LoadRewarded (SequenceID);
    }

    public bool ShowOfferWall () {
        //if (ConsoliAds.Instance.IsRewardedVideoAvailable (SequenceID)) {
        //    // print("Video Call");
        //    Time.timeScale = 0.0001f;
        //    OfferWall.SetActive (true);
        //    // isRewardedVideoShowed = true;
        //    StartLifetimer ();
        //    return true;
        //} else {
        //    Debug.Log ("Rewarded Video Not Available !");
        //    ConsoliAds.Instance.LoadRewarded (SequenceID);
        //}
        return false;
    }

    public bool IsRewardedVideoAvailable () {
        //if (ConsoliAds.Instance.IsRewardedVideoAvailable (SequenceID))
        //    return true;
        //else {
        //    ConsoliAds.Instance.LoadRewarded (SequenceID);
        //    return false;
        //}
        return false;
    }
}