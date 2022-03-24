using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class AttackHelicopterPowerup : Interactable
{
    [SerializeField] private int cost = 1500;
    [SerializeField] private GameObject Gunships;

    public override void Interact(PlayerMovement player)
    {

        if (!Gunships.activeSelf)
        {
            if (player.mainGame.score >= cost)
            {
                player.mainGame.score -= cost;
                player.mainGame.announce(Color.green, "Attack helicopter force have arrived! Supporting for 60 seonds!");

                Gunships.SetActive(true);
                Invoke("EndSupport", 60.0f);
            }
            else
            {
                player.mainGame.centerPrint(Color.red, "You do not have enough points!", 2.0f);
            }
        }
        else
        {
            player.mainGame.centerPrint(Color.red, "You already have an attack helicopter force in session!", 3.0f);
        }

    }

    private void EndSupport()
    {
        Gunships.SetActive(false);
    }

}
