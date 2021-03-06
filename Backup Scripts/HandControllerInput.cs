﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerInput : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj;
	public SteamVR_Controller.Device myDevice;  //map to my device controller

	//MyTeleporter
	private LineRenderer laser;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;
	public float yNudgeAmount = 1f;


	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
		laser = GetComponentInChildren<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		myDevice = SteamVR_Controller.Input ((int)trackedObj.index);

		if (myDevice.GetPress (SteamVR_Controller.ButtonMask.Trigger))
		{
			laser.gameObject.SetActive (true);
			teleportAimerObject.SetActive (true);

			laser.SetPosition (0, gameObject.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit, 15, laserMask)) {
				teleportLocation = hit.point;
				laser.SetPosition (1, teleportLocation);
				//aimer position to teleport
				teleportAimerObject.transform.position = new Vector3 (teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z); 
			} 
			else 
			{
				teleportLocation = new Vector3 (transform.forward.x * 15 + transform.position.x, 
												transform.forward.y * 15 + transform.position.y, 
												transform.forward.z * 15 + transform.position.z);
				RaycastHit groundRay;
				if (Physics.Raycast (teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
				{
					teleportLocation = new Vector3(transform.forward.x*15+transform.position.x, 
													groundRay.point.y, 
													transform.forward.z*15+transform.position.z);
				}
				laser.SetPosition (1, transform.forward*15 + transform.position);

				//aimer transport position
				teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
			}
		}
		if (myDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) 
		{
			laser.gameObject.SetActive (false);
			teleportAimerObject.SetActive (false);
			// only move the player to aimer position when trigger released
			player.transform.position = teleportLocation;

		}
		
	}
}
