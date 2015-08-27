using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public Canvas   MainMenuCanvas;
    public int      PLAY_LEVEL_ID = 1;
    public float    START_LEVEL_DELAY = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayPressed()
    {
        //TODO: Fade the canvas
        
        //Start the game after a short delay
        StartCoroutine("StartGame", START_LEVEL_DELAY);
    }

    public void ShopPressed()
    {

    }

    public void HelpPressed()
    {

    }

    public void OptionsPressed()
    {

    }

    public void CreditsPressed()
    {

    }

    public void QuitPressed()
    {
        //If using editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

        //Else if release
        #else
            Application.Quit();
        #endif
    }

    void StartGame()
    {
        Application.LoadLevel(PLAY_LEVEL_ID);
    }
}
