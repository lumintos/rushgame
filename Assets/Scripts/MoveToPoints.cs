using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//moves object along a series of waypoints, useful for moving platforms or hazards
//this class adds a kinematic rigidbody so the moving object will push other rigidbodies whilst moving
[RequireComponent(typeof(Rigidbody))]
public class MoveToPoints : MonoBehaviour 
{
	public float speed;										//how fast to move
	public float delay;										//how long to wait at each waypoint
	public type movementType;								//stop at final waypoint, loop through waypoints or move back n forth along waypoints
    public bool moveEnabled = false;                        //allow the platform moving

	public enum type { PlayOnce, Loop, PingPong }
	private int currentWp;
	private float arrivalTime;
	private bool forward = true, arrived = false;
	public List<Transform> waypoints = new List<Transform>();
	
	//setup
	void Awake()
	{
		//add kinematic rigidbody
		if(!rigidbody)
			gameObject.AddComponent("Rigidbody");
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;		
		
		//get child waypoints, then detach them (so object can move without moving waypoints)
		foreach (Transform child in transform)
			if(child.tag == "Waypoint")
				waypoints.Add (child);
		if(waypoints.Count == 0)
			Debug.LogError("No waypoints found for 'MoveToPoints' script. To add waypoints: add child gameObjects with the tag 'Waypoint'", transform);
		transform.DetachChildren();
	}
	
	//if we've arrived at waypoint, get the next one
	void Update()
	{
		if(waypoints.Count > 0)
		{
			if(!arrived)
			{
				if (Vector3.Distance(transform.position, waypoints[currentWp].position) < 0.3f)
				{
					arrivalTime = Time.time;
					arrived = true;
				}
			}
			else
			{
				if(Time.time > arrivalTime + delay)
				{
					GetNextWP();
					arrived = false;
				}
			}
		}
	}
	
	//move object toward waypoint
	void FixedUpdate()
	{
        //do not move if it's not allowed
        if (!moveEnabled)
            return;

		if(!arrived && waypoints.Count > 0)
		{
			Vector3 direction = waypoints[currentWp].position - transform.position;
			rigidbody.MovePosition(transform.position + (direction.normalized * speed * Time.fixedDeltaTime));
		}
	}
	
	//get the next waypoint
	private void GetNextWP()
	{
		if(movementType == type.PlayOnce)
		{
			currentWp++;
			if(currentWp == waypoints.Count)
					enabled = false;
		}
		
		if (movementType == type.Loop)
			currentWp = (currentWp == waypoints.Count-1) ? 0 : currentWp += 1;
		
		if (movementType == type.PingPong)
		{
			if(currentWp == waypoints.Count-1)
				forward = false;
			else if(currentWp == 0)
				forward = true;
			currentWp = (forward) ? currentWp += 1 : currentWp -= 1;
		}
	}
	
	//draw gizmo spheres for waypoints
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		foreach (Transform child in transform)
		{
			if(child.tag == "Waypoint")
				Gizmos.DrawSphere(child.position, .7f);
		}
	}

    public void SetPositionByTime(float time)
    {
        Vector3 direction = waypoints[currentWp].position - transform.position;
        rigidbody.MovePosition(transform.position + (direction.normalized * speed * time));
    }
}

/* NOTE: remember to tag object as "Moving Platform" if you want the player to be able to stand and move on it
 * for waypoints, simple use an empty gameObject parented the the object. Tag it "Waypoint", and number them in order */