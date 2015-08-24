using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour 
{
    private uint mScore;        //!< Current score
    private uint mCombo;        //!< Current Combo

    private uint mHighScore;    //!< Highest score
    private uint mHighCombo;    //!< Highest combo

	// Use this for initialization
	void Start () 
    {
	    // @todo: Load high score from file

        ShowCombo ();
        ShowScore ();
	}

    // On Destroy
    void OnDestroy ()
    {
        // @todo: Save high score from file
    }

    // Set combo in UI
    void ShowCombo ()
    {
        GameObject c = GameObject.Find ( "ComboText" );
        UnityEngine.UI.Text ctext = c.GetComponent< UnityEngine.UI.Text > ();
        ctext.text = "Combo: " + mCombo.ToString ();
    }

    // Set score in UI
    void ShowScore ()
    {
        GameObject s = GameObject.Find ( "ScoreText" );
        UnityEngine.UI.Text stext = s.GetComponent< UnityEngine.UI.Text > ();
        stext.text = "Score: " + mScore.ToString ();
    }

    // Add score
    public void AddScore ( uint score )
    {
        mScore += score;
        ShowScore ();
    }

    // Zero score
    public void ZeroScore ()
    {
        mScore = 0;
        ShowScore ();
    }

    // Add combo
    public void AddCombo ( uint combo )
    {
        mCombo += combo;
        ShowCombo ();
    }

    // Zero combo
    public void ZeroCombo ()
    {
        mCombo = 0;
        ShowCombo ();
    }

    // Get gain score
    static public uint GetGainScore ( int gemNum )
    {
        uint n = ( uint )gemNum;
        return n * 10 + ( n - 3 ) * n;
    }
}
