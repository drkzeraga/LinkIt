using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Networking : MonoBehaviour {

    const int   CONNECTION_MAX  = 2;
    const int   NETWORK_PORT    = 7777;
    const bool  USE_NAT         = false;

    public InputField ipDisplay; 

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void Btn_Host_Pressed()
    {
        Network.InitializeServer(CONNECTION_MAX, NETWORK_PORT, USE_NAT);
        ipDisplay.text = Network.player.ipAddress;
    }

    public void Btn_Join_Pressed()
    {
        Network.Connect(ipDisplay.text);
    }
}
