using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections;
using Photon.Pun;

public class WR_Player : MonoBehaviour
{
    [SerializeField] Text playerName;
    [SerializeField] Image avatar;
    Player playerInfo;
    int count = 0;
    private void OnEnable()
    {
        PhotonManager.PlayerEnterExit += OnPlayerCountChange;
    }
    private void OnDisable()
    {
        PhotonManager.PlayerEnterExit -= OnPlayerCountChange;
    }

    public void Init(Player _p)
    {
        playerInfo = _p;
        playerName.text = playerInfo.NickName;
        int avatarIndex = int.Parse((string)playerInfo.CustomProperties["avt"]);
        avatar.sprite = GSF_PlayerSelection.Instance.avaterImages[Random.Range(0,3)];
        //StartCoroutine(Delay(playerInfo));
    }


    //IEnumerator Delay(Player p)
    //{
    //    yield return new WaitForSeconds(2f);
    //    int avatarIndex = int.Parse((string)p.CustomProperties[PhotonManager.pAvt]);
    //    avatar.sprite = GSF_PlayerSelection.Instance.avaterImages[avatarIndex];
    //}
    void OnPlayerCountChange(Player p, bool enter)
    {
        if (p != playerInfo) return;

        Destroy(this.gameObject);
    }
}