using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Material CapturedMaterial;
	public enum PlayerState {
		JUMPING, LANDED, DRIFTING, WALKING, DEAD, CAPTURING
	}
	private PlayerState _state;
	public PlayerState State {
		get
		{
			return _state;
		}
		set
		{
			if(_state == PlayerState.CAPTURING)
			{
				Capture capture = landed.GetComponent<Capture>();
				if(capture != null){
					capture.StopCapture(this);
					captureDelay = captureDelayMax;
				}
			}
			_state = value;
		}
	}
	public GameObject landed = null;
	public float moveSpeed = 180;
	public float jumpForce = 10;
	public GameObject SpawnPoint;

	private float jumpTime = 0;
	private const float jumpTimeMax = 0.5f;

	private float deadTime;
	private const float deadTimeMax = 2f;

	
	private float captureDelay = 0;
	private const float captureDelayMax = 0.5f;

	// Use this for initialization
	void Start () {
		this.State = PlayerState.DRIFTING;
		spawn ();
	}
	
	// Update is called once per frame
	void Update () {
		float axisV =  Input.GetAxis("Vertical");
		if (captureDelay > 0) {
			captureDelay-= Time.deltaTime;
				}
		switch(State)
	{
		case PlayerState.DRIFTING:
			if(Input.GetKeyDown(KeyCode.R)){
				Debug.Log ("suicide");
				this.die ();
			}
			break;
		case PlayerState.LANDED:
		case PlayerState.WALKING:
			if(axisV > 0)
			{
				this.State = PlayerState.JUMPING;
				jumpTime = jumpTimeMax;
			} else if(axisV < 0 && captureDelay <= 0)
			{
				Capture capture = landed.GetComponent<Capture>();
				if(capture != null){
					if(capture.StartCapture(this))
						this.State = PlayerState.CAPTURING;
				}
				else {
					Debug.Log("no capture");
				}
			}
			break;
		case PlayerState.CAPTURING:
			Capture capture = landed.GetComponent<Capture>();
			if( axisV >= 0 || capture.Owner == this)
			{
				this.State = PlayerState.LANDED;
			}
			break;
		case PlayerState.JUMPING:
			this.jumpTime -= Time.deltaTime;
			if(jumpTime <= 0)
				this.State = PlayerState.DRIFTING;
			
			break;
		case PlayerState.DEAD:
			this.deadTime -= Time.deltaTime;
			if(deadTime <= 0)
				this.spawn();
			break;
		}
	}

	void FixedUpdate()
	{
		switch(State)
		{
		case PlayerState.LANDED:
		case PlayerState.WALKING:
			transform.localRotation = Quaternion.LookRotation(Vector3.forward, transform.localPosition);
			float axisH = Input.GetAxis("Horizontal");
			if(axisH != 0){
				transform.RotateAround(landed.transform.position,Vector3.back, axisH * moveSpeed *  Time.deltaTime);
			}

			break;
		case PlayerState.DRIFTING:
			var planets = GameObject.FindGameObjectsWithTag("Planet");
			foreach(var planet in planets)
			{
				var scriptPlanet = planet.GetComponent<Planet>() as Planet;
				var planetPosition = planet.transform.position;
				var playerPosition = this.transform.position;
				
				var gravityVector = planetPosition - playerPosition;
				if(gravityVector.magnitude <= scriptPlanet.GravityRange)
				{
					//var gravityForce = scriptPlanet.GravityIntensity / (gravityVector.magnitude * gravityVector.magnitude);
					//Debug.Log (gravityForce);
					var gravityForce = scriptPlanet.GravityIntensity / (gravityVector.magnitude);		
					gravityVector.Normalize();
					this.rigidbody2D.AddForce(new Vector2(gravityVector.x*gravityForce, gravityVector.y*gravityForce));
				}
			}
			break;
		case PlayerState.JUMPING:
			Debug.Log(rigidbody2D.velocity.magnitude);
			if(rigidbody2D.velocity.magnitude == 0)
			{
				this.jumpTime =jumpTimeMax;
			}
			if(jumpTime == jumpTimeMax){
				Vector2 jumpVector = (transform.position - landed.transform.position).normalized;
				rigidbody2D.velocity = Vector3.zero;
				rigidbody2D.angularVelocity = 0;
				rigidbody2D.AddForce(jumpVector * jumpForce);
				transform.parent = null;
				//rigidbody2D.velocity -= landed.rigidbody2D.velocity;
			}
			break;
		}

	}

	void OnCollisionEnter2D(Collision2D col){
		//Debug.Log (col.gameObject.name);
		if(State == PlayerState.DRIFTING && col.gameObject.tag == "Planet"){
			this.landed =  col.gameObject;
			this.transform.parent = col.gameObject.transform;
			this.rigidbody2D.velocity  = Vector2.zero;
			this.rigidbody2D.angularVelocity = 0;
			this.State = PlayerState.LANDED;
		} 
		if (col.gameObject.tag == "Hazard") {
			die ();		
		}
	}
	void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log (other.gameObject.name);
		if (other.gameObject.tag == "Hazard") {
			die ();		
		}
	}
	void die(){
		this.rigidbody2D.velocity  = Vector2.zero;
		this.rigidbody2D.angularVelocity = 0;
		this.renderer.enabled = false;
		this.transform.parent =null;
		this.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.None;
		this.State =  PlayerState.DEAD;
		this.deadTime = deadTimeMax;
	}

	public void Hit()
	{
		die();
	}

	void spawn(){
		this.State = PlayerState.DRIFTING;
		this.transform.rotation = Quaternion.identity;
		this.transform.position =  SpawnPoint.transform.position;
		this.renderer.enabled = true;
		this.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
	}
}
