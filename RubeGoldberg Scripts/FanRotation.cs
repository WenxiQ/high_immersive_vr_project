using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotation : MonoBehaviour {

	public float rotationSpeed = 200f;

	void Update () {
		transform.Rotate(Vector3.forward * Time.deltaTime*rotationSpeed); //fan rotation
	}
	

}
