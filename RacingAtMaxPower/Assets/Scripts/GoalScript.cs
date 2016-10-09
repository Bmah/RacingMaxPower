using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour {

	 void OnTriggerEnter(Collider other){
		SceneManager.LoadScene("YouWIN");

	}
}
