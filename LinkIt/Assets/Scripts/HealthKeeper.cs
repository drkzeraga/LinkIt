using UnityEngine;
using System.Collections;

public class HealthKeeper : MonoBehaviour 
{
    public int mHealth = 100;

	// Use this for initialization
	void Start () 
    {
        ShowHealth();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        
	}

    // Add health (-ve to minus)
    public void AddHealth ( int gain )
    {
        mHealth += gain;
        ShowHealth ();

        // @todo: Transit scene logic
	    if ( mHealth == 0 )
            ;
    }

    void ShowHealth ()
    {
        GameObject s = GameObject.Find ( "HealthText" );
        UnityEngine.UI.Text stext = s.GetComponent< UnityEngine.UI.Text > ();
        stext.text = "Health: " + mHealth.ToString();
    }
}
