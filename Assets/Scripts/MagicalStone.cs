using UnityEngine;
using System.Collections;


public class MagicalStone : PickupItem {
    public NetworkPlayer keeper; //player who is keeping the stone

    [RPC]
    override public void PickItem(NetworkPlayer collectNetworkPlayer)
    {
        GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameController.stoneKeeper = collectNetworkPlayer;
        gameController.isStoneTaken = true;

        keeper = collectNetworkPlayer;

        networkView.RPC("PickItem", RPCMode.Others, collectNetworkPlayer);
        Destroy(gameObject);
    }
}
