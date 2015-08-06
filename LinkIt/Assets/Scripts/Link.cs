using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Link : MonoBehaviour 
{
    public GameObject mTrailPrefab;

    private GameObject mTrail;

    // Is linking?
    bool IsLinking ()
    {
        return Input.GetMouseButton ( 0 );
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

	// Update is called once per frame
	void FixedUpdate () 
	{
        if( IsLinking () )
        { 
            Ray r = Camera.main.ScreenPointToRay ( Input.mousePosition );
            transform.position = r.GetPoint ( Mathf.Abs( Camera.main.transform.position.z ) - Camera.main.nearClipPlane );

            CreateLink ();
        }
        else
        {
            DestoryLink ();
        }
	}
}
