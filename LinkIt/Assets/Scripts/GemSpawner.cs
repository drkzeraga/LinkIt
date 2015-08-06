using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemSpawner : MonoBehaviour 
{
	public GameObject[] mGemTypes;
	public int mLaneCount = 1;
	public float mSpawnInterval = 1.0f;
	public float mDropSpeed = 1.0f;

	private float mTimeElapsed = 0.0f;
	private LinkedList<Object> mGems = new LinkedList<Object>();

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		UpdateGems ();

		mTimeElapsed +=  Time.fixedDeltaTime;
		if( mTimeElapsed >= mSpawnInterval )
		{
			Spawn ();
			mTimeElapsed -= mSpawnInterval;
		}
	}

	// Get the next colour gem
	int GetNextGem()
	{
		return Random.Range ( 0, mGemTypes.Length );
	}

	// Get the next lane number
	int GetNextLane()
	{
		return Random.Range ( 1, Mathf.Max( 1, mLaneCount + 1 ) );
	}

	// Get the camera view height in world space
	float GetWorldHeight()
	{
		if( Camera.main.orthographic )
		{
			return Camera.main.orthographicSize;
		}
		else
		{
			return Camera.main.transform.position.y + Mathf.Abs ( Camera.main.transform.position.z ) * Mathf.Tan ( 0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView );
		}
	}

	// Spawn logic
	void Spawn ()
	{
		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float width = Camera.main.aspect * height;

		int c = GetNextGem ();
		int l = GetNextLane ();

		// 0.1f -> pad
		float minSpawnX = -( width - 1.0f );
		float spawnWidth = -2.0f * minSpawnX;

		float offset = minSpawnX + ( float )l * spawnWidth / ( float )( mLaneCount + 1 );
		Vector3 spawnPos = Vector3.up * height+
						   Vector3.right * offset;

		mGems.AddLast( Instantiate ( mGemTypes [ c ], spawnPos, Quaternion.identity ) );
	}

	void UpdateGems ()
	{
		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float dropDistance = mDropSpeed *  Time.fixedDeltaTime;

		var current = mGems.First;
		while ( current != mGems.Last )
		{
			GameObject c = ( GameObject )current.Value;

			current = current.Next;

			c.transform.position -= Vector3.up * dropDistance;
			// Out of range
			if( c.transform.position.y <= -height )
			{
				mGems.Remove( c );
				Destroy( c );
			}
		}
	}
}
