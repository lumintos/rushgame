using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
    public float speed;
    public float minHeight, maxHeight;
    private bool movingUp;
    public Vector3 destination;
    public int variant;
	// Use this for initialization
	void Start () {
        minHeight = transform.position.y;
        movingUp = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (movingUp && transform.position.y >= maxHeight)
        {
            movingUp = false;
            destination = transform.position;
            destination.y = minHeight - variant;
        }
        else if (!movingUp && transform.position.y <= minHeight)
        {
            movingUp = true;
            destination = transform.position;
            destination.y = maxHeight + variant;
        }

        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
	}
}
