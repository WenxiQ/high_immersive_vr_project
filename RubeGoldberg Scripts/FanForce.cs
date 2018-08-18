using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanForce : MonoBehaviour {

	public float addForce = 1000f;
	void OnCollisionEnter (Collision colision) {
		
		Rigidbody rb = colision.gameObject.GetComponent<Rigidbody>();
	
		rb.AddForce(transform.forward*addForce);
	}
}
