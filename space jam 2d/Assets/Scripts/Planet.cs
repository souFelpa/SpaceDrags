using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	public bool showDebugRange = false;
	public float GravityIntensity;
	public float GravityRange;
	private GameObject _debugRange;
	public GameObject prefab;
	// Use this for initialization
	void Start () {
		if (showDebugRange) {
						_debugRange = (GameObject)Instantiate (prefab);
				}
	}
	
	// Update is called once per frame
	void Update () {
		if (showDebugRange) {
						_debugRange.transform.position = this.transform.position;
						_debugRange.transform.Translate (0, 0, 10);
						_debugRange.transform.localScale = new Vector3 (GravityRange * 2, GravityRange * 2, 0);
				}
	}
}
