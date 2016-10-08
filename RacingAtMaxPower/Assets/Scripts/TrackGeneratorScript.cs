using UnityEngine;
using System.Collections;

public class TrackGeneratorScript : MonoBehaviour {

	public enum Directions
	{
		North, East, South, West
	};



	public int tileSize = 50;

	public float timePerTile = 1f;

	public GameObject[] StraightTilePool;
	public GameObject[] TurnTilePool;

	// Use this for initialization
	void Start () {
		PlaceTile ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlaceTile(){
		Vector3 TrackLocation = this.transform.position;

		GameObject newTrackPiece = Instantiate (StraightTilePool [Random.Range (0, StraightTilePool.Length)],TrackLocation,Quaternion.identity) as GameObject;
	}
}
