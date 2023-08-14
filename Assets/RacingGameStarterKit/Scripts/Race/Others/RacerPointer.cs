//racerpointer.cs is used to display a minimap pointer above a racer

using UnityEngine;
using System.Collections;

namespace RGSK {

    public class RacerPointer : MonoBehaviour
    {

        public Transform target;
        public float height = 25.0f;
        private float wantedHeight;
        public int markerSize = 25;
        private void Awake()
        {
            if (GameController.instance)
                markerSize = GameController.instance.GetMarkerSize();
            this.transform.localScale = new Vector3(markerSize, 0.1f, markerSize);
        }
        void Start()
        {
            wantedHeight = target.position.y + height;
        }

        void Update()
        {
            if (!target) return;

            //follow the racer
            transform.position = new Vector3(target.position.x, wantedHeight, target.position.z);

            //Rotate in the direction of the racer
            Quaternion rot = transform.rotation;
            rot = target.rotation;
            rot.x = 0;
            rot.z = 0;
            transform.rotation = rot;
        }
    }
}
