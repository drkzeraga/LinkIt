//#define DESTORY_TRAIL_BY_TIME
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Link : MonoBehaviour 
{
    public GameObject mTrailPrefab;

    private GameObject mTrail = null;                                       //!< Trail
    private List< GameObject > mLinkedGems = new List< GameObject > ();     //!< Currently linked objects
    private Vector3 mPrevPosition;
    private int mLinkType = -1;                                             //!< Current link type
    private bool mFailLink = false;
    private float mLinkTime = 0.0f;

    // Is linking?
    bool IsLinking ()
    {
        return Input.GetMouseButton ( 0 );
    }

    // Just started linking?
    bool JustStartedLinking ()
    {
        return Input.GetMouseButtonDown ( 0 );
    }

    // Just finish linking?
    bool JustFinishLinking ()
    {
        return Input.GetMouseButtonUp ( 0 );
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

    // Destroy link
    void DestorySuccessLink ()
    {
        if ( mTrail == null )
            return;

        TrailRenderer t = mTrail.GetComponent< TrailRenderer > ();
        t.time = mLinkTime;
        Destroy ( mTrail, mLinkTime + Time.fixedDeltaTime );
        mLinkTime = 0.0f;

        //Destroy( mTrail );
        mTrail = null;
    }

    // Set link colour
    void SetLinkColor ( Gem g )
    {
        if ( mTrail == null || mLinkType != -1 )
            return;

        Color c = g.mLinkColor;
       
        Material trail = mTrail.GetComponent< TrailRenderer > ().material;

        // Set the color of the material to tint the trail.
        if ( trail != null )
        {
            trail.SetColor( "_TintColor", c );
        }

        ParticleSystem ps = mTrail.GetComponent< ParticleSystem > ();
        if ( ps != null )
        {
            ps.startColor = c;
        }
    }

    // Create Repel
    void CreateRepel ( GameObject g, Gem gem )
    {
        if ( gem.mRepel == null )
            return;

        GameObject e = ( GameObject )Instantiate ( gem.mRepel, g.transform.position, Quaternion.identity );
        e.transform.parent = g.transform;
        e.transform.localScale = Vector3.one;

        ParticleSystem ps = e.GetComponent< ParticleSystem > ();

        if ( ps != null )
        {
            ps.startColor = gem.mParticleColor;
            Destroy ( e, ps.duration + Time.fixedDeltaTime );
        }
        else
        {
            Destroy ( e );
        }
    }

    // Create gain score
    void CreateGainScore ( GameObject g, Gem gem, uint score )
    {
        if ( gem.mGainScore == null )
            return;

        GameObject e = ( GameObject )Instantiate ( gem.mGainScore, g.transform.position, Quaternion.identity );

        TextMesh t = e.GetComponent< TextMesh > ();
        TextFadeOut f = e.GetComponent< TextFadeOut > ();

        if ( t != null && f != null )
        {
            //t.color = gem.mParticleColor;
            t.text = score.ToString ();
            Destroy ( e, f.mLifeTime + Time.fixedDeltaTime );
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

        GameObject singletons = GameObject.Find ( "GameSingletons" );
        if ( singletons == null )
            return;

        GemManager gemManager = singletons.GetComponent< GemManager > ();
        if ( gemManager == null )
            return;

        Vector2 direction = ( Vector2 )transform.position - ( Vector2 )mPrevPosition;
        float distance = direction.magnitude;
        direction.Normalize ();
        Ray r = new Ray ( ( Vector2 )mPrevPosition, direction );

        foreach ( var gem in gemManager.GetAllGems() )
        {
            Gem g = gem.GetComponent< Gem >();
            // If gem is linked, ignore
            if ( g == null || g.GetIsLinked () )
                continue;

            // Test for collision
            if ( mLinkType == -1 || mLinkType == g.mType )
            {
                CircleCollider2D cCollider = gem.GetComponentInChildren< CircleCollider2D > ();
                if ( cCollider == null )
                    continue;
            
                if ( distance == 0.0f )
                {
                    if ( cCollider.bounds.Contains( ( Vector2 )mPrevPosition ) )
                    {
                        g.SetIsLinked ( true );
                        mLinkedGems.Add( gem );

                        SetLinkColor ( g );
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

                            SetLinkColor ( g );
                            mLinkType = g.mType;
                        }
                    }
                }
            }
            else
            {
                BoxCollider2D wCollider = gem.GetComponentInChildren<BoxCollider2D>();
                if ( wCollider == null )
                    continue;

                if ( distance == 0.0f )
                {
                    if ( wCollider.bounds.Contains( ( Vector2 )mPrevPosition ) )
                    {
                        mFailLink = true;
                        DestoryLinkedGems();
                        CreateRepel ( gem, g );
                    }
                }
                else
                {
                    float intersectDistance;
                    if ( wCollider.bounds.IntersectRay( r, out intersectDistance ) )
                    {
                        if ( intersectDistance >= 0.0f && distance >= intersectDistance )
                        { 
                            mFailLink = true;
                            DestoryLinkedGems();
                            CreateRepel ( gem, g );
                        }
                    }
                }
            }
        }
    }

    // Destory all currently linked gems
    bool DestoryLinkedGems ()
    {
        GameObject singletons = GameObject.Find ( "GameSingletons" );
        ScoreKeeper scoreKeeper = ( singletons != null ) ? singletons.GetComponent< ScoreKeeper > () : null;

        bool destory = mLinkedGems.Count >= 3;
        uint score = 0;
        if ( destory )
        {
            scoreKeeper.AddCombo ( ( uint )mLinkedGems.Count );
            score = ScoreKeeper.GetGainScore ( mLinkedGems.Count );
            scoreKeeper.AddScore ( score );
            score /= ( uint )mLinkedGems.Count;
        }

        foreach ( var gem in mLinkedGems )
        {
            Gem g = gem.GetComponent< Gem > ();
            if ( g != null )
            {
                if ( destory )
                {
                    g.SetIsDestroyed ( true );
                    CreateGainScore( gem, g, score );
                }
                else
                {
                    g.SetIsLinked ( false );
                }

                // @todo: debug
                if ( mFailLink )
                    CreateRepel ( gem, g );
            }
        }

        mLinkedGems.Clear();
        mLinkType = -1;

        if ( mFailLink || ( JustFinishLinking () && !destory ) )
            scoreKeeper.ZeroCombo ();

        return destory;
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
                mLinkTime += Time.fixedDeltaTime;
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
            // Unlink logic
#if DESTORY_TRAIL_BY_TIME
            if( DestoryLinkedGems () )
                DestorySuccessLink();
            else
                DestoryLink();
#else
            DestoryLinkedGems ();
            DestoryLink();
#endif

            mFailLink = false;
            mLinkTime = 0.0f;
        }
	}
}
