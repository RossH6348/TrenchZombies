using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class NukePowerup : Interactable
{

    [SerializeField] private int cost = 5000;
    [SerializeField] private GameObject Nuke;

    public override void Interact(PlayerMovement player)
    {

        //See if the nuke powerup is not active yet.
        if (!Nuke.activeSelf)
        {
            //Check if they got enough score.
            if (player.mainGame.score >= cost)
            {
                //Take away the score, and announce that they bought the powerup.
                player.mainGame.score -= cost;
                player.mainGame.announce(Color.green, "Nuke have been launched! Killing all zombies!");

                //Activate powerups.
                Nuke.SetActive(true);
                Nuke.GetComponent<Rigidbody>().velocity = Vector3.up * 50.0f;
            }
            else
            {
                player.mainGame.centerPrint(Color.red, "You do not have enough points!", 2.0f);
            }
        }
        else
        {
            player.mainGame.centerPrint(Color.red, "There is already a nuke launched!", 2.0f);
        }

    }

}
