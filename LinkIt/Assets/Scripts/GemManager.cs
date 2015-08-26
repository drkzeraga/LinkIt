using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemManager : MonoBehaviour 
{
    public GameObject[] mGemTypes;

    private LinkedList< GameObject > mGems = new LinkedList< GameObject > ();
    private LinkedList<GameObject> mGemsToBeRemoved = new LinkedList<GameObject>();
    private float mDropSpeed = 0.0f;

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
        // Update the gems
        UpdateGems();

        //Update and remove destroyed gems
        UpdateDestroyGems();
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

    void UpdateGems ()
	{
        ScoreKeeper scoreKeeper = GetComponent< ScoreKeeper > ();
        HealthKeeper healthKeeper = GetComponent< HealthKeeper > ();

		// Assume camera position is at ( 0 , y, z )
		float height = GetWorldHeight();
		float dropDistance = mDropSpeed *  Time.fixedDeltaTime;

        //Iteriate through the whole list of gems
		foreach ( var c in mGems )
		{
            // Not linked, we update position
            Gem g = c.GetComponent< Gem > ();
            if ( g == null || !g.GetIsLinked() )
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
                        ps.startColor = g.mParticleColor;
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
                    healthKeeper.AddHealth ( -5 );
                }

                DestroyGem( c );
			}
		}
	}

    //Destroy the gem
    void DestroyGem(GameObject obj)
    {
        mGemsToBeRemoved.AddLast(obj);
    }

    //Remove the gems that were destroyed
    void UpdateDestroyGems()
    {
        //Iteriate through and remove the gem
        foreach(var gem in mGemsToBeRemoved)
        {
            mGems.Remove(gem);
            Destroy(gem);
        }

        //Clear the list
        mGemsToBeRemoved.Clear();
        
    }

    public void AddGem ( int type, Vector3 position )
    {
        mGems.AddLast( ( GameObject )( Instantiate ( mGemTypes [ type ], position, Quaternion.identity ) ) );
    }

    public void SetDropSpeed ( float dropSpeed )
    {
        mDropSpeed = dropSpeed;
    }

    public int GetGemTypeCount ()
    {
        return mGemTypes.Length;
    }
}
