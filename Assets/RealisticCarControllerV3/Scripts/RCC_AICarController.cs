//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RGSK;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/AI/AI Controller")]
public class RCC_AICarController : MonoBehaviour {

    #region Variables
    private RCC_CarControllerV3 carController;
	private Rigidbody rigid;	
	// Waypoint Container.
	public WaypointCircuit waypointsContainer;
	public int currentWaypoint = 0;
	// AI Type
	public AIType _AIType;
	public enum AIType {FollowWaypoints, ChasePlayer}	
	// Raycast distances.
	public LayerMask obstacleLayers = -1;
	public int wideRayLength = 20;
	public int tightRayLength = 20;
	public int sideRayLength = 3;
	private float rayInput = 0f;
	private bool  raycasting = false;
	private float resetTime = 0f; 	
	// Steer, motor, and brake inputs.
	private float steerInput = 0f;
	private float gasInput = 0f;
	private float brakeInput = 0f;
	public bool limitSpeed = false;
	public float maximumSpeed = 100f;
	public bool smoothedSteer = true;	
	// Brake Zone.
	private float maximumSpeedInBrakeZone = 0f;
	private bool inBrakeZone = false;	
	// Counts laps and how many waypoints passed.
	public int lap = 0;
	public int totalWaypointPassed = 0;
	public int nextWaypointPassRadius = 40;
    public int initialWaypoint;
    public bool ignoreWaypointNow = false;
	// Unity's Navigator.
	private UnityEngine.AI.NavMeshAgent navigator;
	private GameObject navigatorObject;
    private bool isLeft, isRight, isMiddle, isAvoiding;
    private float avoidMultiplier, nextDetectionTime;
    #endregion

    #region Awake
    void Awake()
    {
		carController = GetComponent<RCC_CarControllerV3>();
		rigid = GetComponent<Rigidbody>();
		carController.AIController = true;
		waypointsContainer = FindObjectOfType(typeof(WaypointCircuit)) as WaypointCircuit;
		navigatorObject = new GameObject("Navigator");
		navigatorObject.transform.parent = transform;
		navigatorObject.transform.localPosition = Vector3.zero;
		navigatorObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
		navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = 1;
		navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 1;
		navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>().angularSpeed = 1000f;
		navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>().height = 1;
		navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>().avoidancePriority = 50;
		navigator = navigatorObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        currentWaypoint = initialWaypoint;
	}
    #endregion

    #region Update
    void Update()
    {	     
		navigator.transform.localPosition = new Vector3(0, carController.FrontLeftWheelCollider.transform.localPosition.y, carController.FrontLeftWheelCollider.transform.localPosition.z);		
	}
    #endregion

    #region FixedUpdate
    void FixedUpdate ()
    {
		if(!carController.canControl)
			return;
		Navigation();
        RayCasting();
        ApplyTorques();
		Resetting();
	}
    #endregion

    #region Navigation
    void Navigation ()
    {		
		if(!waypointsContainer){
			Debug.LogError("Waypoints Container Couldn't Found!");
			enabled = false;
			return;
		}
		if(_AIType == AIType.FollowWaypoints && waypointsContainer && waypointsContainer.waypointList.items.Length < 1){
			Debug.LogError("Waypoints Container Doesn't Have Any Waypoints!");
			enabled = false;
			return;
		}
		
		// Next waypoint's position.
		Vector3 nextWaypointPosition = transform.InverseTransformPoint( new Vector3(waypointsContainer.waypointList.items[currentWaypoint].position.x, transform.position.y, waypointsContainer.waypointList.items[currentWaypoint].position.z));
        //float navigatorInput = Mathf.Clamp(transform.InverseTransformDirection(navigator.desiredVelocity).x * 1.5f, -1f, 1f);

        float navigatorInput = 0f;

        if (_AIType == AIType.FollowWaypoints)
        {
			Debug.LogError("Follow Waypoints!");
            if (navigator.isOnNavMesh)
            {
                navigatorInput = Mathf.Clamp(transform.InverseTransformDirection(navigator.desiredVelocity).x * 1.5f, 1f, 1f);
                navigator.SetDestination(waypointsContainer.waypointList.items[currentWaypoint].position);
            }
            else
            {
                navigatorInput = Mathf.Clamp(nextWaypointPosition.normalized.x * 1.5f, -0.5f, 0.5f);
            }
        }
        else
        {
            if (navigator.isOnNavMesh)
            {
                navigator.SetDestination(waypointsContainer.target.position);
            }
        }
        //Steering Input.
        if (carController.direction == 1){
			if(!ignoreWaypointNow)
            {
                if (isAvoiding)
                {
                    steerInput = Mathf.Clamp(rayInput, -2f, 2f);
                }
                else
                {
                    steerInput = Mathf.Clamp(navigatorInput, -2f, 2f);
                }
            }
            else
            {
                steerInput = Mathf.Clamp(rayInput, -2f, 2f);
            }
        }
        else{
			steerInput = Mathf.Clamp((-navigatorInput - rayInput), -2f, 2f);
		}
		
		if(!inBrakeZone){
			if(carController.speed >= 25){
				brakeInput = Mathf.Lerp(0f, .85f, (Mathf.Abs(steerInput)));
			}else{
				brakeInput = 0f;
			}
		}else{
			brakeInput = Mathf.Lerp(0f, 1f, (carController.speed - maximumSpeedInBrakeZone) / maximumSpeedInBrakeZone);
		}

		if(!inBrakeZone){
			
			if(carController.speed >= 10){
				if(!carController.changingGear)
					gasInput = Mathf.Clamp(1f - (Mathf.Abs(navigatorInput / 10f)  - Mathf.Abs(rayInput / 10f)), .75f, 1f);
				else
					gasInput = 0f;
			}else{
				if(!carController.changingGear)
					gasInput = 1f;
				else
					gasInput = 0f;
			}

		}else{
			
			if(!carController.changingGear)
				gasInput = Mathf.Lerp(1f, 0f, (carController.speed) / maximumSpeedInBrakeZone);
			else
				gasInput = 0f;
		}
		if (_AIType == AIType.FollowWaypoints)
        {		
			// Checks for the distance to next waypoint. If it is less than written value, then pass to next waypoint.
			if (nextWaypointPosition.magnitude < nextWaypointPassRadius)
            {             
                currentWaypoint++;
				totalWaypointPassed++;			
				// If all waypoints are passed, sets the current waypoint to first waypoint and increase lap.
				if (currentWaypoint >= waypointsContainer.waypointList.items.Length) {
					currentWaypoint = 0;
					lap++;
				}
			}
		}		
	}
    #endregion

    #region Resetting
    void Resetting (){
		
		if(carController.speed <= 5 && transform.InverseTransformDirection(rigid.velocity).z < 1f)
			resetTime += Time.deltaTime;
		
		if(resetTime >= 2)
			carController.direction = -1;

		if(resetTime >= 4 || carController.speed >= 25){
			carController.direction = 1;
			resetTime = 0;
		}	
	}
    #endregion

    public void ResetLapVariables()
    {
        currentWaypoint = 0;
        totalWaypointPassed = 0;
    }

    #region ApplyTorques
    void ApplyTorques()
    {
		if(carController.direction == 1){
			if(!limitSpeed){
                carController.gasInput = gasInput;
			}else{
                
                carController.gasInput = gasInput * Mathf.Clamp01(Mathf.Lerp(10f, 0f, (carController.speed) / maximumSpeed));
			}
		}else{
            carController.gasInput = 0f;
		}
		if(smoothedSteer)
			carController.steerInput = Mathf.Lerp(carController.steerInput, steerInput, Time.deltaTime * 20f);
		else
			carController.steerInput = steerInput;

		if(carController.direction == 1)
			carController.brakeInput = brakeInput;
		else
			carController.brakeInput = gasInput;
	}
    #endregion

    #region OnTriggerEnter
    void OnTriggerEnter (Collider col){

        if (col.gameObject.GetComponent<RCC_AIBrakeZone>() || col.gameObject.CompareTag("BreakZone"))
        {
            inBrakeZone = true;
            maximumSpeedInBrakeZone = col.gameObject.GetComponent<RCC_AIBrakeZone>().targetSpeed;
            Debug.Log("Set Target speed " + this.gameObject);
            //GetComponent<RCC_CarControllerV3>().maxspeed = maximumSpeedInBrakeZone;
        /*    if (maximumSpeedInBrakeZone > 300)
            {
                try
                {
                    VehicleNitro.instance.ActivateNitro(1f, 25f, 1000f);
                }
                catch (Exception e)
                {
                    Debug.Log(" calling AI nitro failed " + this.gameObject + e);
                }
            }*/
        }
        if (col.gameObject.CompareTag("Hell"))
        {
            ResetLapVariables();
        }
    }
    #endregion

    #region OnTriggerExit
    void OnTriggerExit (Collider col){
		
		if(col.gameObject.GetComponent<RCC_AIBrakeZone>()){
			inBrakeZone = false;
			maximumSpeedInBrakeZone = 0;
		}	
	}
    #endregion

    #region RayCasting
    private void RayCasting()
    {/*
        if (!isAvoiding)
        {
            isRight = false;
            isLeft = false;
            isMiddle = false;
            rayInput = 0;
        }
        RaycastHit hit;
        Debug.DrawRay(transform.position + transform.forward + transform.right, Quaternion.AngleAxis(10, transform.up) * transform.forward * wideRayLength, Color.blue);
        Debug.DrawRay(transform.position + transform.forward - transform.right, Quaternion.AngleAxis(-10, transform.up) * transform.forward * wideRayLength, Color.blue);
        Debug.DrawRay(transform.position + transform.forward, transform.forward * wideRayLength, Color.blue);

        if (Physics.Raycast(transform.position + transform.forward + transform.right, Quaternion.AngleAxis(10, transform.up) * transform.forward, out hit, wideRayLength))
        {
            if (hit.collider.tag == "Avoid" && !isRight && !isMiddle)
            {
                Debug.DrawRay(transform.position + transform.forward + transform.right, Quaternion.AngleAxis(10, transform.up) * transform.forward * wideRayLength, Color.red);
                isLeft = true;
                rayInput = Mathf.Lerp(-2, 0, hit.distance / wideRayLength);
                if (!isAvoiding)
                {
                    isAvoiding = true;
                    StartCoroutine(ResetIsAvoiding());
                }
            }
        }

        if (Physics.Raycast(transform.position + transform.forward - transform.right, Quaternion.AngleAxis(-10, transform.up) * transform.forward, out hit, wideRayLength))
        {
            if (hit.collider.tag == "Avoid" && !isLeft && !isMiddle)
            {
                Debug.DrawRay(transform.position + transform.forward - transform.right, Quaternion.AngleAxis(-10, transform.up) * transform.forward * wideRayLength, Color.red);
                isRight = true;
                rayInput = Mathf.Lerp(2, 0, hit.distance / wideRayLength);
                if (!isAvoiding)
                {
                    isAvoiding = true;
                    StartCoroutine(ResetIsAvoiding());
                }
            }
        }

        if (Physics.Raycast(transform.position + transform.forward, transform.forward, out hit, wideRayLength))
        {
            if (hit.collider.tag == "Avoid" && !isLeft && !isRight)
            {
                Debug.DrawRay(transform.position + transform.forward, transform.forward * wideRayLength, Color.red);
                if (hit.normal.x < 0)
                {
                    rayInput = Mathf.Lerp(-2, 0, hit.distance / wideRayLength);
                }
                else
                {
                    rayInput = Mathf.Lerp(2, 0, hit.distance / wideRayLength);
                }
                if (!isAvoiding)
                {
                    isAvoiding = true;
                    StartCoroutine(ResetIsAvoiding());
                }
            }
        }*/
    }
    #endregion

    #region ResetIsAvoiding 
    IEnumerator ResetIsAvoiding()
    {
        yield return new WaitForSeconds(0.5f);
        isAvoiding = false;
    }
    #endregion

}