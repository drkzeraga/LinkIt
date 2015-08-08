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
	private LinkedList< GameObject > mGems = new LinkedList< GameObject > ();

    // Get All Gems
    public LinkedList< GameObject > GetAllGems ()
    {
        return mGems;
    }

	// Use this for initialization
	void Start () 
	{
        // Automated correction
	    for ( int i = 0; i < mGemTypes.Length; ++i )
        {
            Gem g = mGemTypes[i].GetComponent< Gem >();
            if ( g != null )
                g.mType = i;
        }
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

		mGems.AddLast( ( GameObject )( Instantiate ( mGemTypes [ c ], spawnPos, Quaternion.identity ) ) );
	}

	void UpdateGems ()
	{
		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float dropDistance = mDropSpeed *  Time.fixedDeltaTime;

		var current = mGems.First;
		while ( current != mGems.Last )
		{
			GameObject c = current.Value;

			current = current.Next;

            // Not linked, we update position
            Gem g = c.GetComponent< Gem >();
            if ( g == null || !g.GetIsLinked() )
			    c.transform.position -= Vector3.up * dropDistance;

			// Out of range
			if ( c.transform.position.y <= -height ||
                 ( g != null && g.GetIsDestroyed() ) )
			{
                if ( g != null && g.GetIsDestroyed() && g.mExplosion != null )
                { 
                    GameObject e = ( GameObject )Instantiate ( g.mExplosion, c.transform.position, Quaternion.identity );
                    ParticleSystem ps = e.GetComponent< ParticleSystem > ();

                    if ( ps != null )
                    {
                        ps.startColor = g.mLinkColor;
                        Destroy ( e, ps.duration + Time.fixedDeltaTime );
                    }
                    else
                    {
                        Destroy ( e );
                    }
                }

				mGems.Remove( c );
				Destroy( c );
			}
		}
	}
}
