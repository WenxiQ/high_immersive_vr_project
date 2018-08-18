using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BallReset : MonoBehaviour {
	public SteamVR_LoadLevel resetLevel;
	public SteamVR_LoadLevel nextLevel;

	//private Transform startTrans; //ball initial position
	public int collectTargets; //set in inspector for different levels
	private int sumCollect;
	private bool isGoal;
	public Text scoreText;
	public Text winText;
	//bool isHitGround = false;
	public bool isPlayArea; // check if inside play area


	void Start () { 
		//startTrans = GetComponent<Transform>(); // initial position of ball
		isPlayArea = true;
		sumCollect = 0;
		SetGoalText();
		gameObject.layer = 10;
		Physics.IgnoreLayerCollision(10, 11);
		isGoal = false;
	}

	void OnTriggerEnter (Collider colliBall){
	
		if(colliBall.gameObject.CompareTag("PlayArea")){  //check collider obj's tag
			isPlayArea = true;
			gameObject.layer = 10;
			//Debug.Log("ball inside play area " + isPlayArea);
		}		
				
		if (colliBall.gameObject.CompareTag("Collectables")){
			sumCollect = sumCollect + 1;
			SetGoalText();
			//Debug.Log("sum of collect = " + sumCollect);
		}

		if (colliBall.gameObject.CompareTag("Goal") && sumCollect >= collectTargets){
			isGoal = true;
			SetGoalText();
		}

		
		if (colliBall.gameObject.CompareTag("Ground") && isGoal==false){
			//isHitGround = true;
			if (!colliBall.GetComponent<AudioSource>().isPlaying) {
				colliBall.GetComponent<AudioSource>().Play();
			}
			resetLevel.Trigger();
			//Debug.Log("reset level");
		}

	}

	void OnTriggerExit (Collider colliBall){

		if(isPlayArea){
			if(colliBall.gameObject.CompareTag("PlayArea")){
				isPlayArea = false;
				//Debug.Log("outside play area " + isPlayArea);
			}
		}
	}

	void SetGoalText()
	{
		// Update the text field of score and targets
		winText.text = "Collect Targets: " + collectTargets.ToString(); //set in inspector for different levels
		scoreText.text = "Score: " + sumCollect.ToString ();

		if (isGoal) 
		{
			// Set the text value of 'winText'
			winText.text = "Goal! You Win!";
			nextLevel.Trigger(); //load next level
		}
	}
}