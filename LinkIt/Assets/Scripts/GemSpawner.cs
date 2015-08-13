using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GemSpawner : MonoBehaviour 
{
	public GameObject[] mGemTypes;
	public int   mLaneCount;
	public float mSpawnFrequency;
	public float mDropSpeed;
    public float mLaneCooldown;

	private LinkedList< GameObject > mGems = new LinkedList< GameObject > ();
	private float   mSpawnCounter = 0.0f;
    private float[] mLaneTimerArr;
    private bool    mIsLaneFull;

    const int       INVALID_LANE = -1;


    //DEBUG STUFF
    const float     SPAWN_FREQ_DELTA = 0.25f;
    const float     DROP_SPEED_DELTA = 0.1f;

    public Text     spawnFreqText;
    public Text     dropSpeedText;

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

        //Init lane cooldow
        InitLane();

        //HACKKK: Seeding with a default value
        Random.seed = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        //Handle input
        HandleInput();

        //Update the lane
        UpdateLane();

        //Update the spawn
        UpdateSpawn();

        //Update the gems
		UpdateGems();

        //Update debug text
        UpdateDebugText();
	}

	// Get the next colour gem
	int GetNextGem()
	{
		return Random.Range ( 0, mGemTypes.Length );
	}

	// Get the next lane number
	int GetNextLane()
	{
        //Get an intial random lane
        int randLane = Random.Range ( 0, Mathf.Max( 0, mLaneCount) );
        
        //This is the lane to be returned
        int choosenLane = randLane;

        //If the choosen lane is on cooldown
        while (IsLaneOnCooldown(choosenLane))
        {
            //Increment to next lane
            ++choosenLane;

            //If choosen lane exceed lane limit
            if (choosenLane == mLaneCount)
            {
                //Wrap around back to start
                choosenLane = 0;
            }

            //If choosen lane is back to initial lane
            if (choosenLane == randLane)
            {
                //Then no lanes is avaliable,
                // return invalid lane
                return INVALID_LANE;
            }
        }

        //Else if the lane is avaliable
        // return lane
        return choosenLane;
	}

    bool IsLaneOnCooldown(int lane)
    {
        Debug.Log(lane);
        return mLaneTimerArr[lane] > 0.0f;
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

    //Update the spawn logic
    void UpdateSpawn()
    {
        //Increment spawn counter
        mSpawnCounter += Time.fixedDeltaTime * mSpawnFrequency;
        
        //While there is still gems remaining to be spawn
        // and lanes are still not full
        while (mSpawnCounter >= 1.0f && !mIsLaneFull)
        {
            //Spawn a new gem
            Spawn();
        }
    }

	// Spawn logic
	void Spawn ()
	{
		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float width = Camera.main.aspect * height;

		int color = GetNextGem ();
		int lane = GetNextLane ();

        //If the lane return is invalid,
        // aka there is no lane free
        if (lane == INVALID_LANE)
        {
            //Set flag to true
            mIsLaneFull = true;
            return;
        }

		// 0.1f -> pad
		float minSpawnX = -( width - 1.0f );
		float spawnWidth = -2.0f * minSpawnX;

		float offset = minSpawnX + ( float )lane * spawnWidth / ( float )( mLaneCount + 1 );
		Vector3 spawnPos = Vector3.up * height+
						   Vector3.right * offset;

		mGems.AddLast( ( GameObject )( Instantiate ( mGemTypes [ color ], spawnPos, Quaternion.identity ) ) );

        //Decrement spawn counter
        mSpawnCounter -= 1.0f;
	}

	void UpdateGems ()
	{
        GameObject scoreKeeperObj = GameObject.Find ( "ScoreKeeper" );
        ScoreKeeper scoreKeeper = ( scoreKeeperObj != null ) ? scoreKeeperObj.GetComponent< ScoreKeeper > () : null;

		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float dropDistance = mDropSpeed *  Time.fixedDeltaTime;

		var current = mGems.First;
		while ( current != mGems.Last )
		{
			GameObject c = current.Value;

			current = current.Next;

            // Not linked, we update position
            Gem g = c.GetComponent< Gem > ();
            if ( g == null || !g.GetIsLinked () )
			    c.transform.position -= Vector3.up * dropDistance;

			// Out of range
			if ( c.transform.position.y <= -height ||
                 ( g != null && g.GetIsDestroyed () ) )
			{
                if ( g != null && g.GetIsDestroyed () && g.mExplosion != null )
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
                else
                {
                    scoreKeeper.ZeroCombo ();
                }

				mGems.Remove ( c );
				Destroy ( c );
			}
		}
	}

    void InitLane()
    {
        //Allocate memeory
        mLaneTimerArr = new float[mLaneCount];

        //Clean the array
        for (int i = 0; i < mLaneCount; ++i)
        {
            mLaneTimerArr[i] = 0.0f;
        }

        //Set lane flag to false
        mIsLaneFull = false;
    }

    void UpdateLane()
    {
        //Reset the lane flag
        mIsLaneFull = false;

        //Iteriate throught all lanes
        for (int i = 0; i < mLaneCount; ++i)
        {
            //Decrement lane timer
            mLaneTimerArr[i] -= Time.fixedDeltaTime;
        }
    }

    void HandleInput()
    {
        //Handle input for debug functions
        HandleDebugInput();
    }

    void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            mSpawnFrequency += SPAWN_FREQ_DELTA;
        }

        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            mSpawnFrequency -= SPAWN_FREQ_DELTA;
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            mDropSpeed += DROP_SPEED_DELTA;
        }

        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            mDropSpeed -= DROP_SPEED_DELTA;
        }
    }

    void UpdateDebugText()
    {
        spawnFreqText.text = "SpawnFreq: " + mSpawnFrequency;
        dropSpeedText.text = "DropSpeed: " + mDropSpeed;
    }
}
