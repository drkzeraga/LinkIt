using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour 
{
    public int mType = 0;                       //!< Type
    public float mScaleFactor = 1.5f;           //!< Up scale factor
    public Color mLinkColor = Color.white;      //!< Link color
    public Color mParticleColor = Color.white;  //!< Particle color
    public GameObject mRepel;                   //!< Repel
    public GameObject mExplosion;               //!< Explosion
    public GameObject mGainScore;               //!< Score gain

    public GameObject mGemBaseType;             //!< Gem base prefab
    private GameObject mGemBase;                //!< Gem base instance

    private bool mLinked = false;               //!< Is linked
    private bool mDestroyed = false;            //!< Is destroyed

    // Get is linked
    public bool GetIsLinked ()
    {
        return mLinked;
    }

    // Set is linked (Call by link script only!)
    public void SetIsLinked ( bool linked )
    {
        if ( mLinked != linked )
        {
            if ( linked )
                transform.localScale = transform.localScale * mScaleFactor;
            else
                transform.localScale = transform.localScale / mScaleFactor;
        }

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
        //Create gem base obj
        InitGemBase();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	
	}

    //Create the gem base obj
    // and attached it to gem
    void InitGemBase()
    {
        //Initialize the gem obj
        mGemBase = (GameObject)Instantiate(mGemBaseType, transform.position, transform.rotation);
        mGemBase.transform.parent = transform;
        mGemBase.transform.localScale = Vector3.one;
    }
}
