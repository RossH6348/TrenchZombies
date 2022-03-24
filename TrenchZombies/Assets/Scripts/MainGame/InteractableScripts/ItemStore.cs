using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class ItemStore : Interactable
{

    public GameObject item = null;
    public int cost = 0;

    public override void Interact(PlayerMovement player)
    {
        if(player.mainGame.score >= cost)
        {
            if (item != null)
            {
                player.entity.addItem(item);
                player.mainGame.score -= cost;
            }
        }
        else
        {
            player.mainGame.centerPrint(Color.red, "You do not have enough points!", 2.0f);
        }
    }

}
