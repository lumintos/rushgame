using UnityEngine;
using System.Collections;


public class MagicalStone : PickupItem {
    public NetworkPlayer keeper; //player who is keeping the stone

    [RPC]
    override public void PickItem(NetworkPlayer collectNetworkPlayer)
    {
        Debug.Log("Set owner: ");
        GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameController.stoneKeeper = collectNetworkPlayer;

        keeper = collectNetworkPlayer;
        Destroy(gameObject);

        networkView.RPC("PickItem", RPCMode.Others, collectNetworkPlayer);
    }
}
