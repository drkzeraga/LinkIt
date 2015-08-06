using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCursor : MonoBehaviour 
{
	// Update is called once per frame
	void FixedUpdate () 
	{
        Ray r = Camera.main.ScreenPointToRay ( Input.mousePosition );
        transform.position = r.GetPoint ( Mathf.Abs( Camera.main.transform.position.z ) - Camera.main.nearClipPlane );
	}
}
