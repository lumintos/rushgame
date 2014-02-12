using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupItem : MonoBehaviour
{

    public Vector3 idleRotation = new Vector3(0, 80, 0);		//idle rotation of coin
    public Vector3 rotationGain = new Vector3(10, 20, 10);	//added rotation when player gets near coin 
    public Vector3 rotation;
    public int maxRotationAdd; //number of times we increase rotation speed
    public float startSpeed = 1f;
    public float speedGain = 0.2f;

    private bool isNearPlayer;
    private TriggerParent triggerParent;
    private GameObject collectPlayer; // Who gets the object
    private Vector3 collectPlayerPosition;

    void Awake()
    {
        collider.isTrigger = true;
        triggerParent = GetComponentInChildren<TriggerParent>();        

        if (!triggerParent)
        {
            GameObject bounds = new GameObject();
            bounds.name = "Bounds";
            bounds.AddComponent("SphereCollider");
            bounds.GetComponent<SphereCollider>().radius = 0.5f;
            bounds.GetComponent<SphereCollider>().isTrigger = true;
            bounds.transform.parent = transform;
            bounds.transform.position = transform.position;
            bounds.AddComponent("TriggerParent");
            triggerParent = GetComponentInChildren<TriggerParent>();
            triggerParent.tagsToCheck = new string[1];
            triggerParent.tagsToCheck[0] = "Player";
            //Debug.LogWarning("No pickup radius 'bounds' trigger attached to coin: " + transform.name + ", one has been added automatically", bounds);
        }
    }

    // Use this for initialization
    void Start()
    {
        rotation = idleRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime, Space.World);

        isNearPlayer = (triggerParent.collided || triggerParent.colliding);
        

        if (isNearPlayer)
        {
            if (Network.isServer)
            {
                collectPlayer = triggerParent.hitObject;
                collectPlayerPosition = collectPlayer.transform.position;
                int nearFlag = 0;
                if (isNearPlayer)
                    nearFlag = 1;

                networkView.RPC("SyncItemStatus", RPCMode.Others, nearFlag, collectPlayerPosition);
            }

            if (rotation.x / rotationGain.x < maxRotationAdd)
            {
                rotation += rotationGain;
            }
            //startSpeed += speedGain;
            //transform.position = Vector3.Lerp(transform.position, collectPlayerPosition, startSpeed * Time.deltaTime);
        }
        else
        {
            if (rotation != idleRotation)
            {
                rotation -= rotationGain;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (Network.isServer) //Only server can decide world's state
        {
            if (other.tag == "Player")
                PickItem(other.networkView.owner);
        }
    }

    [RPC]
    void SyncItemStatus(int collectedFlag, Vector3 collectPlayerPos)
    {
        isNearPlayer = (collectedFlag == 0 ? false : true);
        collectPlayerPosition = collectPlayerPos;
    }

    [RPC]
    virtual public void PickItem(NetworkPlayer collectNetworkPlayer)
    {
        //NetworkPlayer tempNetWorkPlayer;
        //collectPlayer = other.transform.root.gameObject; //this may be different from collectPlayer in Update() since 1 player can jump in other's way
        //tempNetWorkPlayer = collectNetworkPlayer;
        //TODO: RPC to update status of player, do something with this networkPlayer,
        //and destroy object
        Destroy(gameObject);

        networkView.RPC("PickItem", RPCMode.Others, collectNetworkPlayer);
    }
}
