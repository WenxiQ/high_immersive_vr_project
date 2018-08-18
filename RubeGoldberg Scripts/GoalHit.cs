using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHit : MonoBehaviour {
public GameObject goalEffect;
private Vector3 goalPos;
	void Start(){
		
		goalPos = GetComponent<Transform>().transform.position;
		Physics.IgnoreLayerCollision(10, 11);
	}
	void OnTriggerEnter (Collider goalCol){

		if (goalCol.gameObject.CompareTag("Throwable")){
			Instantiate(goalEffect, goalPos, Quaternion.identity);
			gameObject.SetActive(false); 
		}
	}
}
