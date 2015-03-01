using UnityEngine;
using System.Collections;
using DragsRace.Utils;

public class LaserShooter : MonoBehaviour {

	public GameObject Target;
	public GameObject LaserMaterial;
	public bool ShootBackward;
	private GameObject _Laser;
	private GameObject _BackwardLaser;

	private const int _IGNORE_RAYCAST_LAYER = 2;

	// Use this for initialization
	void Start () {
		this.gameObject.layer = _IGNORE_RAYCAST_LAYER;
		_Laser = GameObject.Instantiate(LaserMaterial) as GameObject;
		if(ShootBackward)
			_BackwardLaser = GameObject.Instantiate(LaserMaterial) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		var origin = Utils.ToVector2(this.transform.position);
		var direction = Utils.ToVector2(Target.transform.position) - origin;
		ShootLaser(origin, direction, _Laser);
		if(ShootBackward)
			ShootLaser(origin, direction*(-1), _BackwardLaser);
	}

	void ShootLaser(Vector2 origin, Vector2 direction, GameObject laser)
	{
		RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction);
		LineRenderer renderer = laser.GetComponent<LineRenderer>();
		renderer.SetPosition(0, transform.position);
		if(hitInfo.collider)
			if(_PlayerHit(hitInfo))
				_DoHit(hitInfo);
			else
				_ShowLaserCollision(origin, direction, hitInfo, renderer);
		else
			_ShowInfiniteLaser(renderer, direction);

	}

	void _ShowInfiniteLaser (LineRenderer renderer, Vector2 direction)
	{
		renderer.SetPosition (1, Utils.ToVector3(direction*500));
	}

	bool _PlayerHit(RaycastHit2D hit)
	{
		return hit.collider.tag == "Player";
	}

	void _DoHit(RaycastHit2D hit)
	{
		(hit.collider.GetComponent<Player>() as Player).Hit();
	}

	void _ShowLaserCollision(Vector2 origin, Vector2 direction, RaycastHit2D hitInfo, LineRenderer renderer)
	{
		float distanceToCollider = Vector3.Distance(transform.position, hitInfo.collider.transform.position);	
		Vector2 colliderSurfaceVector = origin + (direction.normalized*distanceToCollider);
		renderer.SetPosition(1, Utils.ToVector3(colliderSurfaceVector));
	}
}
