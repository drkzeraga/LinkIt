using UnityEngine;
using System.Collections;

public class IngameMenu : MonoBehaviour
{
    public Canvas IngameMenuCanvas;

    // Use this for initialization
    void Start()
    {
        IngameMenuCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowIngameMenu()
    {
        IngameMenuCanvas.enabled = true;
    }

    public void HideIngameMenu()
    {
        IngameMenuCanvas.enabled = false;
    }

    public void ResumePressed()
    {
        HideIngameMenu();
    }

    public void OptionsPressed()
    {

    }

    public void RestartPressed()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitPressed()
    {
        Application.LoadLevel("MainMenu");
    }

    public void ShowMenu()
    {

    }
}
