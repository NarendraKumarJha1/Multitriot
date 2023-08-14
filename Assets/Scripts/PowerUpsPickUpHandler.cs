using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpsPickUpHandler : MonoBehaviour
{
    public int powerUuid = 0;

    [SerializeField] List<GameObject> toDisableOnPickUp = null;
    [SerializeField] List<MeshRenderer> pickUpMeshes = null;
    [SerializeField] Collider mainCollider = null;

    float nitroToAdd = 25.0f;

    [SerializeField] PowerUpsHandler.PowerUpType pickUpType = PowerUpsHandler.PowerUpType.None;

    void OnEnable()
    {
        PhotonManager.OnPowerPickup += OnPowerPickupCallback;
        //PhotonManager.OnPowerPickup += OnPowerPickupCallbackFreeMode;
    }
    private void OnDisable()
    {
        PhotonManager.OnPowerPickup -= OnPowerPickupCallback;
        //PhotonManager.OnPowerPickup -= OnPowerPickupCallbackFreeMode;
    }

    private void OnTriggerEnter(Collider hit)
    {

        if (hit.transform.tag == "Player")
        {
            // Debug.LogError("trigger entered of powerups into players");
            OnPowerPickupCallbackFreeMode();
        }
        if (PhotonManager.IsGuest)
        {
          //  Debug.LogError("trigger entered of powerups");
            if (hit.transform.root.tag == "Player")
            {
               // Debug.LogError("trigger entered of powerups into players");
                OnPowerPickupCallbackFreeMode();
            }
            //    PhotonManager.FreeModePickup();


            //switch (pickUpType)
            //{

                
            //    case PowerUpsHandler.PowerUpType.Boost:
            //        NitroManager.Instance.AddNitroAmount(100);
            //        break;

            //}
            return;
        }

        PhotonView _pv = null;
        if (hit.GetComponentInParent<PowerUpsHandler>())
            _pv = hit.GetComponentInParent<PhotonView>();

        if (_pv)
        {
            /*Debug.Log("Hitting with me :" + _pv.name);
            Debug.Log("ActorNumber :" + _pv.Controller.ActorNumber);*/

            object[] _data = new object[] { _pv.Controller.ActorNumber, powerUuid };
            PhotonManager.PhotonRaiseEventsSender_All(PhotonManager.PowerPick, _data);
        }
    }

    private void OnPowerPickupCallback(int actorNo, int pwrUuid)
    {
        if (PhotonManager.IsGuest) return;
        if (pwrUuid != powerUuid) return;

        /*Debug.Log("Ac: " + actorNo + ", Pwr: " + pwrUuid);
        Debug.Log("Power Name: " + this.gameObject.name);
        Debug.Log("Power Type:" + pickUpType.ToString());*/

        if (actorNo == PhotonNetwork.LocalPlayer.ActorNumber)
            GameController.instance.GivePower(pickUpType, pickUpType == PowerUpsHandler.PowerUpType.Boost ? nitroToAdd : 0);

        EnablePowerUp(false);
        openAgainCoroutine = StartCoroutine(OpenAgain());
    }

    private void OnPowerPickupCallbackFreeMode()
    {



        /*Debug.Log("Ac: " + actorNo + ", Pwr: " + pwrUuid);
        Debug.Log("Power Name: " + this.gameObject.name);
        Debug.Log("Power Type:" + pickUpType.ToString());*/

        Debug.LogError("power pickup");
        GameController.instance.GivePower(pickUpType, pickUpType == PowerUpsHandler.PowerUpType.Boost ? nitroToAdd : 0);

        EnablePowerUp(false);
        openAgainCoroutine = StartCoroutine(OpenAgain());
    }

    void EnablePowerUp(bool isEnable)
    {
        pickUpMeshes.ForEach(x => x.enabled = isEnable);
        toDisableOnPickUp.ForEach(x => x.SetActive(isEnable));

        mainCollider.enabled = isEnable;
    }

    readonly int respawnTime = 10;
    Coroutine openAgainCoroutine;
    IEnumerator OpenAgain()
    {
        yield return new WaitForSeconds(respawnTime);

        EnablePowerUp(true);

        if (openAgainCoroutine != null)
            StopCoroutine(openAgainCoroutine);
        openAgainCoroutine = null;
    }
}