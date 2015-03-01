using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capture : MonoBehaviour {

	private enum State{
		IDLE, CAPTURE, CAPTURED
	}
	private State state;
	float CaptureTime = 2f;
	Player currentPlayer = null;
	Player owner = null;
	public Player Owner{
		get{ return owner;}
	}
	private class CaptureProgress {
		public Player player;
		public float progress;
	}
	List<CaptureProgress> progs;
	// Use this for initialization
	void Start () {
		progs = new List<CaptureProgress> ();
		state = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.CAPTURE:
			CaptureProgress prog = new CaptureProgress();
			prog.progress = -1;
			foreach (CaptureProgress p in progs)
			{
				if(p.player == currentPlayer)
				{
					prog = p;
				}
			}
			if(prog.progress == -1)
			{
				Debug.Log("created obj");
				prog = new CaptureProgress();
				prog.player = currentPlayer;
				prog.progress = 0;
				progs.Add(prog);
			}
			prog.progress += Time.deltaTime;
			if(prog.progress > CaptureTime){
				capture (currentPlayer);
			}
		break;
		}
	}
	public void StopCapture(Player player){
		if (player == currentPlayer) {
			this.state = State.IDLE;
			this.currentPlayer = null;
		}
	}
	public bool StartCapture(Player player){
		Debug.Log ("start capture"); 
		if (currentPlayer == null &&( state == State.IDLE || state == State.CAPTURED) && owner != player) {
			currentPlayer = player;
			state = State.CAPTURE;
			return true;
				}
		return false;
	}
	void capture(Player player){
		Debug.Log ("Captured!");
		state = State.CAPTURED;
		owner = player;
		renderer.material = owner.CapturedMaterial;
	}
}
