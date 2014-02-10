/*
 * Not in used
 * 
using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
    public float speed;
    public float minHeight, maxHeight;
    private bool movingUp;
    public Vector3 destination;
    public int variant;
    public Vector3 originalPosition;
    private Vector3 endPosition;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
        minHeight = originalPosition.y;
        movingUp = false;
	}
	
	// Update is called once per frame
	void Update () {
       // if (networkView != null && Network.isServer)
       // {
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

            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
       // }
        else if (networkView != null && Network.isClient)
        {
            SyncedMovement();
        }
	}
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncRigidVelocity = Vector3.zero;
        Vector3 syncPosition = Vector3.zero;

        if (stream.isWriting)
        {
            syncRigidVelocity = new Vector3(0, speed, 0);
            syncPosition = transform.position;

            stream.Serialize(ref syncRigidVelocity);
            stream.Serialize(ref syncPosition);
        }
        if (stream.isReading)
        {
            stream.Serialize(ref syncRigidVelocity);
            stream.Serialize(ref syncPosition);

            syncTime = 0;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncRigidVelocity * syncDelay;
            syncStartPosition = transform.position;
        }
    }

    void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }
}
 * 
 */
