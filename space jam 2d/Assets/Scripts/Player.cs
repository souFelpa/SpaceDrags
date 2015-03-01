using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public enum State {
		JUMPING, LANDED, DRIFTING, WALKING, DEAD
	}
	public State state;
	public GameObject landed = null;
	public float moveSpeed = 180;
	public float jumpForce = 10;
	public GameObject SpawnPoint;

	private float jumpTime = 0;
	private const float jumpTimeMax = 0.5f;

	private float deadTime;
	private const float deadTimeMax = 2f;


	// Use this for initialization
	void Start () {
		this.state = State.DRIFTING;
	}
	
	// Update is called once per frame
	void Update () {
		switch(state)
		{
			case State.DRIFTING:
				if(Input.GetKeyDown(KeyCode.R)){
					Debug.Log ("suicide");
					this.die ();
				}
				break;
			case State.LANDED:
			case State.WALKING:
				float axisV =  Input.GetAxis("Vertical");
				if(axisV > 0)
				{
					this.state = State.JUMPING;
					jumpTime = jumpTimeMax;
				}
				break;
			case State.JUMPING:
				this.jumpTime -= Time.deltaTime;
				if(jumpTime <= 0)
					this.state = State.DRIFTING;
				
				break;
			case State.DEAD:
				this.deadTime -= Time.deltaTime;
				if(deadTime <= 0)
					this.spawn();
				break;
		}
	}

	void FixedUpdate()
	{
		switch(state)
		{
		case State.LANDED:
		case State.WALKING:
			transform.localRotation = Quaternion.LookRotation(Vector3.forward, transform.localPosition);
			float axisH = Input.GetAxis("Horizontal");
			if(axisH != 0){
				transform.RotateAround(landed.transform.position,Vector3.back, axisH * moveSpeed *  Time.deltaTime);
			}

			break;
		case State.DRIFTING:
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
		case State.JUMPING:
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
				Debug.Log(landed.rigidbody2D.velocity.magnitude);
				Debug.Log(landed.rigidbody2D.angularVelocity);
			}
			break;
		}

	}
	void OnCollisionEnter2D(Collision2D col){
		if(state == State.DRIFTING && col.gameObject.tag == "Planet"){
			this.landed =  col.gameObject;
			this.transform.parent = col.gameObject.transform;
			this.rigidbody2D.velocity  = Vector2.zero;
			this.rigidbody2D.angularVelocity = 0;
			this.state = State.LANDED;
		} else {
			Debug.LogWarning("not landed");
		}
	}
	void die(){
		this.rigidbody2D.velocity  = Vector2.zero;
		this.rigidbody2D.angularVelocity = 0;
		this.renderer.enabled = false;
		this.transform.parent =null;
		this.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.None;
		this.state =  State.DEAD;
		this.deadTime = deadTimeMax;
	}
	void spawn(){
		this.state = State.DRIFTING;
		this.transform.rotation = Quaternion.identity;
		this.transform.position =  SpawnPoint.transform.position;
		this.renderer.enabled = true;
		this.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
	}
}
