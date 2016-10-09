using UnityEngine;
using System.Collections;

public class TrackPieceScript : MonoBehaviour {

	public bool isTurn = false;

	private Color startingColor;
	private bool transitionEvent;
	private float transitionSpeed = 1f;

	public void Start() {
		startingColor = GetComponent<Renderer> ().material.color;
		GetComponent<Renderer> ().material.color = new Color (startingColor.r, startingColor.g, startingColor.b, 0f);
		transitionEvent = true;
	}

	public void Update() {
		if (transitionEvent) {
			startingColor = GetComponent<Renderer> ().material.color;
			GetComponent<Renderer> ().material.color = new Color (startingColor.r,startingColor.g, startingColor.b, Mathf.MoveTowards(startingColor.a, 1f, transitionSpeed * Time.deltaTime));
		} else {
			startingColor = GetComponent<Renderer> ().material.color;
			GetComponent<Renderer> ().material.color = new Color (startingColor.r,startingColor.g, startingColor.b, Mathf.MoveTowards(startingColor.a, 0f, transitionSpeed * Time.deltaTime));
			if (GetComponent<Renderer> ().material.color.a < 0.05f) {
				Destroy (this.gameObject);
			}
		}

		//Debug.Log(Mathf.MoveTowards(startingColor.a, 1f, 0f * Time.deltaTime));
	}

	public void Appear(){
		transitionEvent = true;
	}

	public void Dissapear(){
		transitionEvent = false;
	}
}
