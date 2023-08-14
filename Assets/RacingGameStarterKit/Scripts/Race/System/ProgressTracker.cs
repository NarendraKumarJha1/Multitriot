using UnityEngine;

namespace RGSK
{
    public class ProgressTracker : MonoBehaviour
    {
        [SerializeField] private WaypointCircuit circuit; // A reference to the waypoint-based route we should follow

        public float lookAheadForTargetOffset = 20f;

        public float lookAheadForTargetFactor = 0.1f;

        public float lookAheadForSpeedOffset = 20;

        public float lookAheadForSpeedFactor = 0.5f;

        public Transform target;

        private Statistics _statistics;

        public float progressDistance;

        public float raceCompletion;

        public Vector3 lastPosition = Vector3.zero;

        private float speed;

        public WaypointCircuit.RoutePoint progressPoint { get; private set; }

        public bool init = false;

        public void Init()
        {
            if (init) return;

            target = new GameObject("pt").transform;
            _statistics = GetComponent<Statistics>();

            _statistics.target = target;
            target.name = _statistics.RacerDetail.racerName + " Progress Tracker";

            circuit = RaceManager.instance.pathContainer.GetComponent<WaypointCircuit>();
            init = true;
        }

        void Update()
        {
            if (!init) return;

            if (!RaceManager.instance) return;

            if (circuit == null)
                circuit = RaceManager.instance.pathContainer.GetComponent<WaypointCircuit>();

            if (Time.deltaTime > 0) speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude / Time.deltaTime, Time.deltaTime);

            target.SetPositionAndRotation(
                circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed).position,
                Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed).direction));

            progressPoint = circuit.GetRoutePoint(progressDistance);

            Vector3 progressDelta = progressPoint.position - transform.position;

            if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                progressDistance += progressDelta.magnitude * 0.5f;

            if (Vector3.Dot(progressDelta, progressPoint.direction) > 5.0f)
                progressDistance -= progressDelta.magnitude * 0.5f;

            lastPosition = transform.position;

            if (!_statistics.finishedRace)
            {
                if (!_statistics.knockedOut)
                    raceCompletion = (progressDistance / (circuit.Length * RaceManager.instance.totalLaps)) * 100;
            }
            else
                raceCompletion = 100;

            raceCompletion = Mathf.Round(raceCompletion * 100) / 100;
        }

        void OnDestroy()
        {
            if (target)
                Destroy(target.gameObject);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Application.isPlaying && circuit != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
#endif
    }
}