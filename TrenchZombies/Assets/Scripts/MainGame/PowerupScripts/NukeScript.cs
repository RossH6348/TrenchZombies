using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class NukeScript : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 200.0f;
    [SerializeField] private float explosionDmg = 1000.0f;
    [SerializeField] private Rigidbody rigid;

    private void Start()
    {

    }

    private void Update()
    {
        //This will always rotate the nuke model to face toward its direction.
        transform.LookAt(transform.position + rigid.velocity.normalized);
    }

    public void OnCollisionEnter(Collision other)
    {
        //KA-BOOOOOOOOOOOOM!!!
        //Do explosion and damage zombies within its blast radius.
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombie in zombies)
        {
            float ratio = 1.0f - ((zombie.transform.position - other.GetContact(0).point).magnitude / explosionRadius);
            if (ratio > 0.0f)
            {
                BaseZombie zombieAI = zombie.GetComponent<BaseZombie>();
                if (zombieAI != null)
                    zombieAI.Damage(ratio * explosionDmg);
            }
        }

        //Deactivate the nuke instead upon collision.
        transform.position = new Vector3(0.0f, 30.0f, 0.0f);
        gameObject.SetActive(false); 
    }
}
