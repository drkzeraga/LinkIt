using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Link : MonoBehaviour 
{
    public GameObject mTrailPrefab;
    public GameObject mRepelPrefab;

    private GameObject mTrail = null;                                       //!< Trail
    private List< GameObject > mLinkedGems = new List< GameObject > ();     //!< Currently linked objects
    private Vector3 mPrevPosition;
    private int mLinkType = -1;                                             //!< Current link type
    private bool mFailLink = false;

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

    // Get color
    public static Color GetColor ( int type )
    {
        switch ( type )
        {
            case 0:
                return Color.blue;
            case 1:
                return Color.green;
            case 2:
                return Color.red;
            case 3:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    // Set link colour
    void SetLinkColor ( int type )
    {
        if ( mTrail == null || mLinkType != -1 )
            return;

        Color c = GetColor ( type );
       
        Material trail = mTrail.GetComponent< TrailRenderer > ().material;

        // Set the color of the material to tint the trail.
        if ( trail != null )
            trail.SetColor( "_Color", c );

        ParticleSystem ps = mTrail.GetComponent< ParticleSystem > ();
        if ( ps != null )
        {
            ps.startColor = c;
        }
    }

    // Create Repel
    void CreateRepel ( GameObject g, int type )
    {
        if ( mRepelPrefab == null )
            return;

        GameObject e = ( GameObject )Instantiate ( mRepelPrefab, g.transform.position, Quaternion.identity );
        e.transform.parent = g.transform;
        e.transform.localScale = Vector3.one;

        ParticleSystem ps = e.GetComponent< ParticleSystem > ();

        if ( ps != null )
        {
            ps.startColor = Link.GetColor( type );
            Destroy ( e, ps.duration + Time.fixedDeltaTime );
        }
        else
        {
            Destroy ( e );
        }
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

        Vector2 direction = ( Vector2 )transform.position - ( Vector2 )mPrevPosition;
        float distance = direction.magnitude;
        direction.Normalize ();
        Ray r = new Ray ( ( Vector2 )mPrevPosition, direction );

        foreach ( var gem in spawner.GetAllGems() )
        {
            Gem g = gem.GetComponent< Gem >();
            // If gem is linked, ignore
            if ( g == null || g.GetIsLinked () )
                continue;

            // Test for collision
            if ( mLinkType == -1 || mLinkType == g.mType )
            {
                CircleCollider2D cCollider = gem.GetComponent< CircleCollider2D > ();
                if ( cCollider == null )
                    continue;
            
                if ( distance == 0.0f )
                {
                    if ( cCollider.bounds.Contains( ( Vector2 )mPrevPosition ) )
                    {
                        g.SetIsLinked ( true );
                        mLinkedGems.Add( gem );

                        SetLinkColor ( g.mType );
                        mLinkType = g.mType;
                    }
                }
                else
                {
                    float intersectDistance;
                    if ( cCollider.bounds.IntersectRay( r, out intersectDistance ) )
                    {
                        if ( intersectDistance >= 0.0f && distance >= intersectDistance )
                        { 
                            g.SetIsLinked ( true );
                            mLinkedGems.Add( gem );

                            SetLinkColor ( g.mType );
                            mLinkType = g.mType;
                        }
                    }
                }
            }
            else
            {
                BoxCollider2D wCollider = gem.GetComponent< BoxCollider2D > ();
                if ( wCollider == null )
                    continue;

                if ( distance == 0.0f )
                {
                    if ( wCollider.bounds.Contains( ( Vector2 )mPrevPosition ) )
                    {
                        DestoryLinkedGems ();
                        mFailLink = true;
                        CreateRepel ( gem, g.mType );
                    }
                }
                else
                {
                    float intersectDistance;
                    if ( wCollider.bounds.IntersectRay( r, out intersectDistance ) )
                    {
                        if ( intersectDistance >= 0.0f && distance >= intersectDistance )
                        { 
                            DestoryLinkedGems ();
                            mFailLink = true;
                            CreateRepel ( gem, g.mType );
                        }
                    }
                }
            }
        }
    }

    // Destory all currently linked gems
    void DestoryLinkedGems ()
    {
        bool destory = mLinkedGems.Count >= 3;
        foreach ( var gem in mLinkedGems )
        {
            Gem g = gem.GetComponent< Gem >();
            //  @todo: Destroy linked gems if more than 3
            if ( g != null )
            {
                if ( destory )
                    g.SetIsDestroyed ( true );
                else
                    g.SetIsLinked ( false );
            }
        }

        mLinkedGems.Clear();
        mLinkType = -1;
    }

	// Update is called once per frame
	void FixedUpdate () 
	{
        // Linking
        if ( IsLinking () )
        {
            if ( !mFailLink )
            { 
                Ray r = Camera.main.ScreenPointToRay ( Input.mousePosition );
                transform.position = r.GetPoint ( Mathf.Abs( Camera.main.transform.position.z ) - Camera.main.nearClipPlane );

                CreateLink ();

                // Link Logic
                LinkGems ();
                mPrevPosition = transform.position;
            }
            else
            {
                DestoryLink ();
            }
        }
        // Not Linking
        else
        {
            DestoryLink ();

            // Unlink logic
            DestoryLinkedGems ();

            mFailLink = false;
        }
	}
}
