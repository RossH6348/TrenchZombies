using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class PortalScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Check if the player is entering the portal, if so teleport them by setting their position to the destination's transform position.
        if (LayerMask.LayerToName(other.gameObject.layer).Equals("Player"))
            other.gameObject.transform.position = transform.GetChild(0).position;
    }
}
