using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour 
{
    public int mType = 0;                   //!< Type
    public Color mLinkColor = Color.white;  //!< Link color
    public GameObject mRepel;               //!< Repel
    public GameObject mExplosion;           //!< Explosion

    private bool mLinked = false;           //!< Is linked
    private bool mDestroyed = false;        //!< Is destroyed

    // Get is linked
    public bool GetIsLinked ()
    {
        return mLinked;
    }

    // Set is linked (Call by link script only!)
    public void SetIsLinked ( bool linked )
    {
        mLinked = linked;
    }

    // Get is destroyed
    public bool GetIsDestroyed ()
    {
        return mDestroyed;
    }

    // Set is destroyed (Call by link script only!)
    public void SetIsDestroyed ( bool destroyed )
    {
        mDestroyed = destroyed;
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	
	}
}
