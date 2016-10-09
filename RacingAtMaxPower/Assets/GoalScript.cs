using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	 void OnTriggerEnter(Collider other){
		if (other.name == "car") {
			Debug.Log ("You WIN");
		}

	}
}
