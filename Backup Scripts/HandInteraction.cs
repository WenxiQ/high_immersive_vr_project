using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
	public SteamVR_TrackedObject trackedHandObj;
	public SteamVR_Controller.Device deviceHand;
	public float throwForce = 2.0f;
	// Use this for initialization
	void Start () {
		trackedHandObj = GetComponent <SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		deviceHand = SteamVR_Controller.Input ((int)trackedHandObj.index);
	}

	void OnTriggerStay(Collider col) {
		if (col.gameObject.CompareTag ("Throwable")) {
			if (deviceHand.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				ThrowObject (col);

			} 
			else if (deviceHand.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				
				GrabObject (col);
			}
		}
	}

	void ThrowObject (Collider coli) {
		coli.transform.SetParent (null);
		Rigidbody rigidBody = coli.GetComponent<Rigidbody> ();//
		rigidBody.isKinematic = false;
		rigidBody.velocity = deviceHand.velocity * throwForce;
		rigidBody.angularVelocity = deviceHand.angularVelocity;
		Debug.Log ("Released the trigger");
	}

	void GrabObject (Collider coli) {
		coli.transform.SetParent (gameObject.transform);
		coli.GetComponent<Rigidbody> ().isKinematic = true;
		deviceHand.TriggerHapticPulse (durationMicroSec:2000);
		Debug.Log ("Touching down the trigger to grab an object");
	}

}

