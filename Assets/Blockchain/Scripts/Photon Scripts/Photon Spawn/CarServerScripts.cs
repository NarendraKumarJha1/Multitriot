 using Photon.Pun;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CarServerScripts : MonoBehaviour
{
    List<MonoBehaviour> monoBehaviours;
    public PhotonView pv = null;

    private void OnEnable()
    {
        PhotonManager.OnPowerUsed += OnPowerUsedCallback;
        //PhotonManager.OnEmitSkid += AddSkid;
    }

    private void OnDisable()
    {
        PhotonManager.OnPowerUsed -= OnPowerUsedCallback;
        //PhotonManager.OnEmitSkid -= AddSkid;
    }

    private void Start()
    {
        pv = this.GetComponent<PhotonView>();
        if (!pv.IsMine) return;

        monoBehaviours = new List<MonoBehaviour>();
        monoBehaviours = this.GetComponents<MonoBehaviour>().ToList();

        foreach (MonoBehaviour m in monoBehaviours)
        {
            m.enabled = true;
        }
    }

    private void OnPowerUsedCallback(bool active, int actorNo, PowerUpsHandler.PowerUpType pwType)
    {
        if (pv.Controller.ActorNumber != actorNo) return;

        switch (pwType)
        {
            case PowerUpsHandler.PowerUpType.Boost:
                VehicleNitro vn = this.GetComponent<VehicleNitro>();
                vn.ActiveNosEffect(active);
                break;
        }
    }

    /* private void AddSkid(int actorNo, Vector3 pos, Vector3 normal, float intensity, int lastIndex)
     {
         if (pv.Controller.ActorNumber != actorNo) return;

         RGSK.Skidmark.instance.AddSkidMark(pos, normal, intensity, lastIndex);
     }*/
}