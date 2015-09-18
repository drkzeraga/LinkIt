using UnityEngine;
using System.Collections;

public class IngameUI : MonoBehaviour {

    public GameObject  GameSingletons;
    public Canvas      IngameMenuCanvas;
    private GemSpawner mGemSpawner;

	// Use this for initialization
	void Start () {
        mGemSpawner = GameSingletons.GetComponent<GemSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ToggleSpawnFreq()
    {
        mGemSpawner.IncrementSpawnFreq();
    }

    public void ToggleDropSpeed()
    {
        mGemSpawner.IncrementDropSpeed();
    }

    public void MenuPressed()
    {
        IngameMenuCanvas.GetComponent<IngameMenu>().ShowIngameMenu();
    }

}
