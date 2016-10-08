using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	 void OnTriggerEnter(Collider other){
		if (other.name == "Hovercar") {
			Debug.Log ("You WIN");
		}

	}
}
