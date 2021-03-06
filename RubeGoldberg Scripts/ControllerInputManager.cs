using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj;
	public SteamVR_Controller.Device myDevice;  //map to my device controller, both
	
	//MyTeleporter
	private LineRenderer laser;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;
	public float teleportIndicatorHeight;

	//Dash
	public float dashSpeed = 20f;
	private bool isDashing;
	private float lerpTime;
	private Vector3 dashStartPosition;
	public float throwForce = 2.0f;

	//object menu with swipe
	private float swipeSum;
	private float touchLast;
	private float touchCurrent;
	private float touchLength;
	private bool hasSwipedLeft;
	private bool hasSwipeRight;
	private bool isMenuActive = false; //active menu check
	public ObjectMenuManager objectMenuManager; //linked to Object Menu Manager Class
	//check ball status
	public BallReset checkPlayArea; //check if ball inside play area， set in editor for BallReset object

	// Use this for initialization
	void Start () {
	// for using controllers for teleporting: 
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
		
		myDevice = SteamVR_Controller.Input ((int)trackedObj.index); // get the index no. for both controllers
	
		laser = GetComponentInChildren<LineRenderer> ();
		
		//fix teleport 0817
		laser.gameObject.SetActive (false);
		teleportAimerObject.SetActive (false);

	}
	

	void Update () {
	//check left or right controller by device index, comparing with myDevice.index value
		var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);  
		var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);  
			
		// Dashing locomotion for more immersive
		// Check the Left Controller index by SteamVR, only left controller for teleporting
		if (myDevice.index == deviceIndex1){
			
			if (isDashing) 
			{
				lerpTime += Time.deltaTime * dashSpeed;
				player.transform.position = Vector3.Lerp (dashStartPosition, teleportLocation, lerpTime);
				if (lerpTime >= 1) 
				{
					isDashing = false;
					lerpTime = 0;
				}
			} 
			else 
			{
				//if using trigger (myDevice.GetPress (SteamVR_Controller.ButtonMask.Trigger)) 
				if (myDevice.GetTouch (SteamVR_Controller.ButtonMask.Touchpad))
				{
					laser.gameObject.SetActive (true);
					//teleportAimerObject.SetActive (true);
					laser.SetPosition (0, gameObject.transform.position);
					RaycastHit hit;
					if (Physics.Raycast (transform.position, transform.forward, out hit, 10, laserMask)) {
						teleportAimerObject.SetActive (true);
						teleportLocation = hit.point;
						laser.SetPosition (1, teleportLocation);
						//towards aimer position to teleport
						teleportAimerObject.transform.position = new Vector3 (teleportLocation.x, teleportLocation.y + teleportIndicatorHeight, teleportLocation.z); 
					}
					//fix teleport 0817
					else
					{
						teleportAimerObject.SetActive (false);
						laser.gameObject.SetActive (false);				
					}
					//fix teleport 0817, below not used
					/*else {
							teleportLocation = new Vector3 (transform.forward.x * 10 + transform.position.x, transform.forward.y * 10 + transform.position.y, transform.forward.z * 15 + transform.position.z);
							RaycastHit groundRay;
							if (Physics.Raycast (teleportLocation, -Vector3.up, out groundRay, 20, laserMask)) {
								teleportLocation = new Vector3 (transform.forward.x * 10 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
							}
						laser.SetPosition (1, transform.forward * 10 + transform.position);

						//aimer transport position
						teleportAimerObject.transform.position = teleportLocation + new Vector3 (0, teleportIndicatorHeight, 0);
					}*/
				}

				//if using trigger (myDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) 
				if (myDevice.GetTouchUp (SteamVR_Controller.ButtonMask.Touchpad)) 
				{
					laser.gameObject.SetActive (false);
					teleportAimerObject.SetActive (false);
					//if using trigger: player.transform.position = teleportLocation; // only move the player to aimer position when trigger released
					// dashing trigger
					dashStartPosition = player.transform.position;
					isDashing = true;
				}
			}
		}

		// object menu swipe
		// Check the Right Controller index for swiping and spawning
		if(myDevice.index == deviceIndex2){
			if(myDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)){
				touchLast = myDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;			
			}

			if (myDevice.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){
				if(!isMenuActive){
					showMenu(); // call show menu method
				}
				touchCurrent = myDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
				touchLength = touchCurrent - touchLast;
				touchLast = touchCurrent;
				swipeSum += touchLength;
			}

			if(!hasSwipeRight){
				if(swipeSum > 0.5f){
					swipeSum = 0;
					SwipeRight();//call swipe right method
					hasSwipeRight = true;
					hasSwipedLeft = false;
				}
			}

			if(!hasSwipedLeft){
				if(swipeSum < -0.5f){
					swipeSum = 0;
					SwipedLeft(); //call swipe left function
					hasSwipeRight = false;
					hasSwipedLeft = true;
				}
			}

			//reset all touchpad after touchup
			if(myDevice.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)){
				swipeSum = 0;
				touchCurrent = 0;
				touchLast = 0;
				hasSwipedLeft = false;
				hasSwipeRight = false;
				hideMenu();
			}
			//spawn objects press trigger (or touchpad)
			//if(myDevice.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
			if(isMenuActive && myDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
				SpawnObject(); //spawn object currently selected in menu
				hideMenu();
			}
		}

	}
	//touch pad to activate menu, disable menu after spawn
	void showMenu(){
		objectMenuManager.ActivateMenu();
		isMenuActive = true;
	}
	void hideMenu(){
		objectMenuManager.DisableMenu();
		isMenuActive = false;
	}


	//spawn objects by Object Menu Manager
	void SpawnObject(){
		objectMenuManager.SpawnCurrentObject();
	}

	//swipe functions and linked to menu manager
	void SwipedLeft(){
		objectMenuManager.MenuLeft();
	}
	void SwipeRight(){
		objectMenuManager.MenuRight();
	}


	// for hand interaction and Rube Goldburg obj placing
	void OnTriggerStay(Collider col) {

		if (col.gameObject.CompareTag ("Throwable")) {
			Renderer renderMaterial = col.GetComponent<Renderer> ();

			if(checkPlayArea.isPlayArea == true){	//check if ball inside play area
				
				//Debug.Log ("Grab the ball and will score, ball inside playarea");
			}

			if(checkPlayArea.isPlayArea == false){ //check if ball outside play area
				renderMaterial.material.color = Color.blue;
				col.gameObject.layer = 11;
				//Debug.Log ("Grab the ball and will NOT score, ball outside playarea");
			}	

			if (myDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				ThrowObject (col);
			} 
			else if (myDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {	
				GrabObject (col);
			}
		}
		//rube goldburg objects
		if (col.gameObject.CompareTag ("Structure")) {
			if (myDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				PutRubeObject (col);
			} 
			else if (myDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {	
				GrabRubeObject (col);
			}
		}
	}

	void ThrowObject (Collider coli) {
		coli.transform.SetParent (null);
		Rigidbody rigidBody = coli.GetComponent<Rigidbody> ();//
		rigidBody.isKinematic = false; //set ball throwable
		rigidBody.velocity = myDevice.velocity * throwForce;
		rigidBody.angularVelocity = myDevice.angularVelocity;		
	}

	void GrabObject (Collider coli) {
		//Renderer renderMaterial = coli.GetComponent<Renderer> ();
		coli.transform.SetParent (gameObject.transform);
		coli.GetComponent<Rigidbody> ().isKinematic = true;
		myDevice.TriggerHapticPulse (5000); // pulse may be too short to feel it
		
	/*	if(checkPlayArea.isPlayArea == true){	//check if ball inside play area
			Debug.Log ("Grab the ball and will score, ball inside playarea");
		}

		if(checkPlayArea.isPlayArea == false){ //check if ball outside play area
			//renderMaterial.material.color = Color.blue;
			//coli.gameObject.layer = 11;
			Debug.Log ("Grab the ball and will NOT score, ball outside playarea");
		}			
	*/	
	}	

// for Rube Goldburg objects interaction
	void PutRubeObject (Collider coli) {
		coli.transform.SetParent (null);
		//Debug.Log ("Released the Rube Object");
	}

	void GrabRubeObject (Collider coli) {
		coli.transform.SetParent (gameObject.transform);
		myDevice.TriggerHapticPulse (5000); // pulse may be too short to feel it
		//Debug.Log ("Touching down Rube Object");
	}	
}
