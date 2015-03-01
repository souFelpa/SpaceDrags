using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

	public GameObject Planet;
	public float orbitSpeed = 10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Planet != null)
			transform.RotateAround (Planet.transform.position, new Vector3(0, 0, 1), orbitSpeed * Time.deltaTime);
	}
}
