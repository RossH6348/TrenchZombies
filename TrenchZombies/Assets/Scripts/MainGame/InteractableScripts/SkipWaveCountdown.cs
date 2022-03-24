using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class SkipWaveCountdown : Interactable
{
    public override void Interact(PlayerMovement player)
    {
        player.mainGame.skipCountdown();
    }
}
