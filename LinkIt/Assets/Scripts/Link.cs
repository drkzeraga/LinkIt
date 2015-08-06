using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Link : MonoBehaviour 
{
    public GameObject mTrailPrefab;

    private GameObject mTrail = null;                                       //!< Trail
    private List< GameObject > mLinkedGems = new List< GameObject > ();     //!< Currently linked objects
    private Vector3 mPrevPosition;

    // Is linking?
    bool IsLinking ()
    {
        return Input.GetMouseButton ( 0 );
    }

    // Just started linking?
    bool JustStartedLinking()
    {
        return Input.GetMouseButtonDown ( 0 );
    }

    // Create link
    void CreateLink ()
    {
        if ( mTrail != null )
            return;

        mTrail = ( GameObject )Instantiate( mTrailPrefab, transform.position, Quaternion.identity );
        mTrail.transform.parent = transform;
        mTrail.transform.localScale = Vector3.one;
    }

    // Destroy link
    void DestoryLink ()
    {
        if ( mTrail == null )
            return;

        Destroy( mTrail );
        mTrail = null;
    }

    // Link gems
    void LinkGems ()
    {
        // If link just started, there is no ray to test intersection with
        if ( JustStartedLinking() )
            return;

        GameObject spawnerObj = GameObject.Find ( "GemSpawner" );
        if ( spawnerObj == null )
            return;

        GemSpawner spawner = spawnerObj.GetComponent< GemSpawner > ();
        if ( spawner == null )
            return;

        Vector3 direction = transform.position - mPrevPosition;
        float distance = direction.magnitude;
        direction.Normalize ();
        Ray r = new Ray ( mPrevPosition, direction );

        foreach ( var gem in spawner.GetAllGems() )
        {
            Gem g = gem.GetComponent< Gem >();
            // If gem is linked, ignore
            if ( g == null || g.GetIsLinked () )
                continue;

            // Test for collision
            // @todo: Test for correct or wrong collision
            BoxCollider2D wCollider = gem.GetComponent< BoxCollider2D > ();
            if ( wCollider == null )
                continue;

            if( wCollider.bounds.IntersectRay( r ) )
            {
                g.SetIsLinked ( true );
                mLinkedGems.Add( gem );
                Debug.Log ( "Ray = " + r + ", Distance = " + distance );
            }
        }
    }

    // Destory all currently linked gems
    void DestoryLinkedGems ()
    {
        foreach ( var gem in mLinkedGems )
        {
            Gem g = gem.GetComponent< Gem >();
            //  @todo: Destroy linked gems
            if ( g != null )
                g.SetIsLinked ( false );
        }

        mLinkedGems.Clear();
    }

	// Update is called once per frame
	void FixedUpdate () 
	{
        // Linking
        if( IsLinking () )
        { 
            Ray r = Camera.main.ScreenPointToRay ( Input.mousePosition );
            transform.position = r.GetPoint ( Mathf.Abs( Camera.main.transform.position.z ) - Camera.main.nearClipPlane );

            CreateLink ();

            // Link Logic
            LinkGems ();
            mPrevPosition = transform.position;
        }
        // Not Linking
        else
        {
            DestoryLink ();

            // Unlink logic
            DestoryLinkedGems ();
        }
	}
}
