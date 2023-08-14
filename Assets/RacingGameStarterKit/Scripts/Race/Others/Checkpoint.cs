using System.Collections;
using UnityEngine;

namespace RGSK {
    public class Checkpoint : MonoBehaviour {

        public enum CheckpointType { Speedtrap, TimeCheckpoint }
        public CheckpointType checkpointType;
        // [HideInInspector]
        public float timeToAdd = 15.0f; //time to add (Checkpoints Race Only)

        public AudioSource sfxPlayer;

        private void Start () {
            if (!sfxPlayer)
                sfxPlayer = this.GetComponent<AudioSource> ();
        }

        private void OnTriggerEnter (Collider other) {
            if (other.transform.root.CompareTag ("Player")) {
                if (sfxPlayer) sfxPlayer.Play ();
            }
        }

    }
}