using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GemSpawner : MonoBehaviour 
{
	//public GameObject[] mGemTypes;
	public int   mLaneCount;
	public float mSpawnFrequency;
	public float mDropSpeed;
    public float mLaneCooldown;

	//private LinkedList< GameObject > mGems = new LinkedList< GameObject > ();
	private float   mSpawnCounter = 0.0f;
    private float[] mLaneTimerArr;
    private bool    mIsLaneFull;

    const int       INVALID_LANE = -1;

    private List<int>   SpawnList = new List<int>();
    private int         SpawnListLimit;
    private int         SpawnListIndex = 0;
    private int         SpawnGroupMaxSize = 3;    //Max number of gem to spawn


    //DEBUG STUFF
    const float     SPAWN_FREQ_DELTA = 0.25f;
    const float     DROP_SPEED_DELTA = 0.02f;

    public Text     spawnFreqText;
    public Text     dropSpeedText;

    private GemManager mGemManager = null;

	// Use this for initialization
	void Start () 
	{
        //Init lane cooldow
        InitLane();

        //HACKKK: Seeding with a default value
        Random.seed = 0;

        mGemManager = GetComponent< GemManager > ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        //Update the spawn variables
        UpdateSpawnVariables();

        //Handle input
        HandleInput();

        //Update the lane
        UpdateLane();

        //Update the spawn
        UpdateSpawn();

        //Update debug text
        UpdateDebugText();

        //Update gem drop speed
        mGemManager.SetDropSpeed( mDropSpeed );
	}

	// Get the next colour gem
	int GetNextGem()
	{
        //If the spawn list is used
        if (SpawnListIndex >= SpawnList.Count)
        {
            //Regenerate the spawn list
            GenerateSpawnList();
        }

        //Return the gem at index
        // and increment index
		return SpawnList[SpawnListIndex++];
	}

	// Get the next lane number
	int GetNextLane()
	{
        //Get an intial random lane
        int randLane = Random.Range ( 0, Mathf.Max( 1, mLaneCount ) );
        
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
        //Debug.Log(lane);
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

		float offset = minSpawnX + ( float )lane * spawnWidth / ( float )( mLaneCount - 1 );
        //if ( lane == mLaneCount - 1 )
        //    Debug.Log( "Position = " + offset );

		Vector3 spawnPos = Vector3.up * height+
						   Vector3.right * offset;

		//mGems.AddLast ( ( GameObject )( Instantiate ( mGemTypes [ color ], spawnPos, Quaternion.identity ) ) );
        mGemManager.AddGem( color, spawnPos );

        //Decrement spawn counter
        mSpawnCounter -= 1.0f;
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

    void UpdateSpawnVariables()
    {
        //update the max list size
        //  calculated as the number of gems being spawn per second multipied
        //  by the time needed for the gem to travel half of the screen rounded up
        SpawnListLimit = Mathf.CeilToInt(mSpawnFrequency * mDropSpeed * 0.5f);
    }

    void GenerateSpawnList()
    {
        //keep track of the number of gem per type spawn
        var gemCountArray = new int[mGemManager.GetGemTypeCount()];

        //type of gem to be spawn
        int gemType, spawnGemGroupSize;

        //Empty the old spawnList
        SpawnList.Clear();

        //while list is not full
        while (SpawnList.Count < SpawnListLimit)
        {
            //roll a random gem type
            gemType = Random.Range(0, mGemManager.GetGemTypeCount());

            //Roll a random number of gems to spawn
            // from 1 to SpawnGemSize
            spawnGemGroupSize = 1 + Random.Range(0, SpawnGroupMaxSize);

            //for every gem to be spawned
            for (int i=0; i<spawnGemGroupSize; ++i)
            {
                //Push back the select gem type into the array
                SpawnList.Add(gemType);

                //increment gem count array
                ++gemCountArray[gemType];
            }
        }

        //at the end, do a check on the number of gems
        for (int i = 0; i < mGemManager.GetGemTypeCount(); ++i)
        {
            //if we are adding gem of a type
            if (gemCountArray[i] != 0)
            {
                //make sure there is a minimium of 3
                while (gemCountArray[i] < 3)
                {
                    SpawnList.Add(i);
                    ++gemCountArray[i];
                }
            }
        }

        //randomly swap around the gem type
        ShuffleSpawnList();

        //for in case player have only 2 gems of a type on screen
        for (int i = 0; i < mGemManager.GetGemTypeCount(); ++i)
        {
            //Get the number of gems of type on screen
            int numberOfGemsOfTypeOnScreen = mGemManager.GetGemCountOfType(i);

            //if there are gems of that type 
            if (numberOfGemsOfTypeOnScreen > 0)
            {
                //and the count is less than 3
                for (; numberOfGemsOfTypeOnScreen<3; ++numberOfGemsOfTypeOnScreen)
                {
                    //insert the gem at the front so that it will spawn first
                    SpawnList.Insert(0,i);
                }
            }
        }

        //reset the spawn index
        SpawnListIndex = 0;
    }

    void ShuffleSpawnList()
    {
        for (int i=0; i<SpawnList.Count; ++i)
        {
            int tmp = SpawnList[i];
            int r = Random.Range(0, SpawnList.Count);

            SpawnList[i] = SpawnList[r];
            SpawnList[r] = tmp;
        }
    }
}
