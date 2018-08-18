using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {
	public List<GameObject> objectList; //automatically set at start for this game object's transform
	public List<GameObject> objectPrefabList; //drag and drop in the inspector
	public int currentObject = 0;
	public int numOfObjUse = 1; //num of same kind of objects spawn
	
	public List<GameObject> spawnObjectList;
	public int currentSpawnObj = 0;
	
	// Use this for initialization
	void Start () {
		foreach(Transform child in transform) {
			objectList.Add(child.gameObject);
		}	
	}
	
	//touch pad to activate menu, swipe to change
	public void ActivateMenu(){
		
		objectList[currentObject].SetActive(true);
	}

	public void DisableMenu(){
		
		objectList[currentObject].SetActive(false);
	}

	public void MenuLeft() {
		objectList[currentObject].SetActive(false);
		currentObject -- ;
		if(currentObject < 0) {
			currentObject = objectList.Count - 1;
		}
		objectList[currentObject].SetActive(true);
	}
	
	public void MenuRight() {
		objectList[currentObject].SetActive(false);
		currentObject ++ ;
		if(currentObject > objectList.Count-1) {
			currentObject = 0;
		}
		objectList[currentObject].SetActive(true);
	}

	public void SpawnCurrentObject(){
		if(spawnObjectList.Count < numOfObjUse) { //check num of obj spawn
		//create a list to store spawn objects
			spawnObjectList.Add(Instantiate(objectPrefabList[currentObject], 
												objectList[currentObject].transform.position, objectList[currentObject].transform.rotation));
			currentSpawnObj++;
		}
			
	}

}
