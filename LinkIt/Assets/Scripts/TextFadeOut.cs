using UnityEngine;
using System.Collections;

public class TextFadeOut : MonoBehaviour 
{
    public float mLifeTime = 0.5f;
    public float mSpeed = 0.5f;

	// Use this for initialization
	void Start () 
    {
	    //Destroy ( this, mLifeTime + Time.fixedDeltaTime );
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        TextMesh t = GetComponent< TextMesh > ();
        t.color = new Color ( t.color.r, t.color.g, t.color.b, t.color.a - Time.fixedDeltaTime / mLifeTime );
        transform.position += Vector3.up * mSpeed * Time.fixedDeltaTime;
	}
}
