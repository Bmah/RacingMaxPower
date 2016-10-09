using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour {

	 void OnTriggerEnter(Collider other){
		Debug.Log ("trigger entered");
		if (other.tag == "Player") {
			SceneManager.LoadScene("TitleScene");
		}

	}
}
