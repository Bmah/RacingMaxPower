using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackGeneratorScript : MonoBehaviour {

	public enum Directions
	{
		North, East, South, West
	};

	public int tileSize = 50;

	public int liveTileCount = 10;
	public float timePerTile = 0.5f;
	public float timeOfNextTile = 0;

	public Directions trackDirection;

	public GameObject[] StraightTilePool;
	public GameObject[] TurnTilePool;
	public GameObject[] RampTilePool;
	public bool goingUp = true;

	public int[] turnsPerElevation = new int[100];

	Queue<GameObject> trackQueue = new Queue<GameObject>();

	// Use this for initialization
	void Start () {
		trackDirection = (Directions)Random.Range(0,4);
	}
	
	// Update is called once per frame
	void Update () {
		if (timeOfNextTile < Time.time) {
			timeOfNextTile = Time.time + timePerTile;
			PlaceTile();
			if (trackQueue.Count > liveTileCount) {
				GameObject deadTrack = trackQueue.Dequeue ();
				if (deadTrack.GetComponent<TrackPieceScript> ().isTurn) {
					turnsPerElevation [((int)deadTrack.transform.position.y / 10)+50]--;	
				}
				Destroy (deadTrack);
			}
		}
	}



	void PlaceTile(){

		float nextTrackType = Random.value;
		if (turnsPerElevation [((int)this.transform.position.y/10) + 50] < 2) {
			if (nextTrackType < 0.7) {
				PlaceStraightTile ();
			} else {
				PlaceTurnTile ();
				turnsPerElevation [((int)this.transform.position.y/10) + 50]++;
			}
		} else {
			if (this.transform.position.y/10 >= 49) {
				goingUp = false;
			} else if (this.transform.position.y/10 <= -49) {
				goingUp = true;
			}
			PlaceRampTile (goingUp);
		}
	
			
	}

	void PlaceStraightTile ()
	{
		Vector3 trackLocation = this.transform.position;
		Vector3 nextTrackGeneratorLocation = new Vector3 (0, 0, 0);

		//depending on direction set placement for both the new tile and the new location of the track placer;
		switch (trackDirection) {
		case Directions.North:
			trackLocation.z += tileSize / 2;
			nextTrackGeneratorLocation.z += tileSize;
			break;
		case Directions.East:
			trackLocation.x += tileSize / 2;
			nextTrackGeneratorLocation.x += tileSize;
			break;
		case Directions.South:
			trackLocation.z -= tileSize / 2;
			nextTrackGeneratorLocation.z -= tileSize;
			break;
		case Directions.West:
			trackLocation.x -= tileSize / 2;
			nextTrackGeneratorLocation.x -= tileSize;
			break;
		}
		//create the new tile at the specified location
		GameObject newTrackPiece = Instantiate (StraightTilePool [Random.Range (0, StraightTilePool.Length)], trackLocation, Quaternion.identity) as GameObject;

		trackQueue.Enqueue (newTrackPiece);

		//rotate the tile if it needs to be adjusted
		switch (trackDirection) {
		case Directions.East:
		case Directions.West:
			newTrackPiece.transform.Rotate (new Vector3 (0, 90, 0));
			break;
		}
		//move the Track Generator to the new intersection
		this.transform.position += nextTrackGeneratorLocation;
	}

	void PlaceTurnTile (){
		Vector3 trackLocation = this.transform.position;
		Vector3 nextTrackGeneratorLocation = new Vector3 (0, 0, 0);
		bool clockWise = Random.value > 0.5f;

		//depending on direction set placement for both the new tile and the new location of the track placer;
		switch (trackDirection) {
		case Directions.North:
			trackLocation.z += tileSize / 2;
			if (clockWise) {
				nextTrackGeneratorLocation.x += tileSize/2;
				trackDirection = Directions.East;
			} else {
				nextTrackGeneratorLocation.x -= tileSize/2;
				trackDirection = Directions.West;
			}
			nextTrackGeneratorLocation.z += tileSize/2;
			break;
		case Directions.East:
			trackLocation.x += tileSize / 2;
			if (clockWise) {
				nextTrackGeneratorLocation.z -= tileSize/2;
				trackDirection = Directions.South;
			} else {
				nextTrackGeneratorLocation.z += tileSize/2;
				trackDirection = Directions.North;
			}
			nextTrackGeneratorLocation.x += tileSize/2;
			break;
		case Directions.South:
			trackLocation.z -= tileSize / 2;
			if (clockWise) {
				nextTrackGeneratorLocation.x -= tileSize/2;
				trackDirection = Directions.West;
			} else {
				nextTrackGeneratorLocation.x += tileSize/2;
				trackDirection = Directions.East;
			}
			nextTrackGeneratorLocation.z -= tileSize/2;
			break;
		case Directions.West:
			trackLocation.x -= tileSize / 2;
			if (clockWise) {
				nextTrackGeneratorLocation.z += tileSize/2;
				trackDirection = Directions.North;
			} else {
				nextTrackGeneratorLocation.z -= tileSize/2;
				trackDirection = Directions.South;
			}
			nextTrackGeneratorLocation.x -= tileSize/2;
			break;
		}
			
		//create the new tile at the specified location
		GameObject newTrackPiece = Instantiate (TurnTilePool [Random.Range (0, TurnTilePool.Length)], trackLocation, Quaternion.identity) as GameObject;

		trackQueue.Enqueue (newTrackPiece);

		newTrackPiece.GetComponent<TrackPieceScript> ().isTurn = true;

		//rotate the tile if it needs to be adjusted
		switch (trackDirection) {
		case Directions.North:
			if (clockWise) {
				newTrackPiece.transform.Rotate (new Vector3 (0, 0, 0));
			} else {
				newTrackPiece.transform.Rotate (new Vector3 (0, 270, 0));
			}
			break;
		case Directions.East:
			if (clockWise) {
				newTrackPiece.transform.Rotate (new Vector3 (0, 90, 0));
			} else {
				newTrackPiece.transform.Rotate (new Vector3 (0, 0, 0));
			}
			break;
		case Directions.South:
			if (clockWise) {
				newTrackPiece.transform.Rotate (new Vector3 (0, 180, 0));
			} else {
				newTrackPiece.transform.Rotate (new Vector3 (0, 90, 0));
			}
			break;
		case Directions.West:
			if (clockWise) {
				newTrackPiece.transform.Rotate (new Vector3 (0, 270, 0));
			} else {
				newTrackPiece.transform.Rotate (new Vector3 (0, 180, 0));
			}
			break;
		}
			
		//move the Track Generator to the new intersection
		this.transform.position += nextTrackGeneratorLocation;
	}

	void PlaceRampTile(bool up){
		Vector3 trackLocation = this.transform.position;
		Vector3 nextTrackGeneratorLocation = new Vector3 (0, 0, 0);

		if (up) {
			nextTrackGeneratorLocation.y += 10;
		} else {
			nextTrackGeneratorLocation.y -= 10;
			trackLocation.y -= 10;
		}

		//depending on direction set placement for both the new tile and the new location of the track placer;
		switch (trackDirection) {
		case Directions.North:
			trackLocation.z += tileSize / 2;
			nextTrackGeneratorLocation.z += tileSize;
			break;
		case Directions.East:
			trackLocation.x += tileSize / 2;
			nextTrackGeneratorLocation.x += tileSize;
			break;
		case Directions.South:
			trackLocation.z -= tileSize / 2;
			nextTrackGeneratorLocation.z -= tileSize;
			break;
		case Directions.West:
			trackLocation.x -= tileSize / 2;
			nextTrackGeneratorLocation.x -= tileSize;
			break;
		}

		//GameObject RampTile = RampTilePool [Random.Range (0, RampTilePool.Length)];
		//create the new tile at the specified location
		GameObject newTrackPiece = Instantiate (RampTilePool [Random.Range (0, RampTilePool.Length)], trackLocation, Quaternion.identity) as GameObject;

		trackQueue.Enqueue (newTrackPiece);

		//rotate the tile if it needs to be adjusted
		switch (trackDirection) {
		case Directions.North:
			newTrackPiece.transform.Rotate (new Vector3 (0, 180, 0));
			break;
		case Directions.East:
			newTrackPiece.transform.Rotate (new Vector3 (0, 270, 0));
			break;
		case Directions.South:
			newTrackPiece.transform.Rotate (new Vector3 (0, 0, 0));
			break;
		case Directions.West:
			newTrackPiece.transform.Rotate (new Vector3 (0, 90, 0));
			break;
		}

		if (!up) {
			newTrackPiece.transform.Rotate (new Vector3 (0, 180, 0));
		}

		//move the Track Generator to the new intersection
		this.transform.position += nextTrackGeneratorLocation;
	}

}
