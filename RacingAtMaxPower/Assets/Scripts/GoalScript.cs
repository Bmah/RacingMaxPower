using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	 void OnTriggerEnter(Collider other){
		Debug.Log ("trigger entered");
		if (other.tag == "Player") {
			Debug.Log ("You WIN");
		}

	}
}
