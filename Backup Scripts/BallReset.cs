using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {
	public SteamVR_LoadLevel resetLevel;
	public SteamVR_LoadLevel nextLevel;
	//public GoalTrigger goalTrigger;
	private Transform startTrans; //ball initial position
	public int goalTargets = 2;
	private int sumOfGoal = 0;
	bool isHitGround = false;

	void Start () { 
		startTrans = GetComponent<Transform>(); // initial position of ball
	}
	void Update(){
		
		if(sumOfGoal == goalTargets){
			nextLevel.Trigger();
		}
	}
	void OnTriggerEnter (Collider colliBall){
		if (colliBall.gameObject.CompareTag("Goal")){
			sumOfGoal = sumOfGoal + 1;
			Debug.Log("sum of goal = " + sumOfGoal);
		}
		//colliPos = colliBall.GetComponent<Transform>().transform.position;  //get collider obj transform component and the position
		if (colliBall.gameObject.CompareTag("Ground")){
			isHitGround = true;
			Destroy(gameObject); //destory this ball object
			//Instantiate(gameObject,startTrans.transform.position, Quaternion.identity);//reset this ball object
			sumOfGoal = 0;
			resetLevel.Trigger();
		}
	}
}