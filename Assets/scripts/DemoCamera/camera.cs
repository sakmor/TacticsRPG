using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {
	

	// Use this for initialization
	void Start () 
	{
		this.transform.rotation =Quaternion.Euler(　 new Vector3 (0,60,0));		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
