using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class Message : Interactable
{
    public Color color = Color.white;
    public string message = "";
    public float time = 3.0f;

    public override void Interact(PlayerMovement player)
    {
        player.mainGame.centerPrint(color, message, time);
    }

}
