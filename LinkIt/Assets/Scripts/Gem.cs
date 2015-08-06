using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour 
{
    public int mType = 0;               //!< Type

    private bool mLinked = false;   //!< Is linked

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

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	
	}
}
