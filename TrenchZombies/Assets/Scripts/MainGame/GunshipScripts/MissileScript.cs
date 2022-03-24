using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class MissileScript : MonoBehaviour
{

    private Rigidbody rigid;
    private MeshRenderer renderer;
    private MeshCollider collide;
    [SerializeField] private GameObject Gunship;

    [SerializeField] private float explosionDmg = 100.0f;
    [SerializeField] private float explosionRadius = 12.0f;

    private Vector3 mountPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        renderer = GetComponent<MeshRenderer>();
        collide = GetComponent<MeshCollider>();
        collide.enabled = false;

        mountPos = transform.localPosition;
    }

    //Reset the missile being fired.
    public void ReloadMissile()
    {
        transform.SetParent(Gunship.transform);
        renderer.enabled = true;
        collide.enabled = false;
        rigid.velocity = rigid.angularVelocity = Vector3.zero;

        //Reset the rotation and re-mount.
        transform.rotation = Quaternion.LookRotation(Gunship.transform.forward);
        transform.localPosition = mountPos;
    }

    //This will fire the missile at whatever position requested.
    public void FireMissile(Vector3 targetPos)
    {
        transform.parent = null;
        collide.enabled = true;

        //Calculate the velocity to give to the rocket.
        Vector3 direction = (targetPos - transform.position).normalized;
        rigid.velocity = direction * 40.0f;

        //Rotate the missile so it goes toward the position.
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void OnCollisionEnter(Collision other)
    {
        //KA-BOOOOOOOOOOOOM!!!

        //Do explosion and damage zombies within its blast radius.
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach(GameObject zombie in zombies)
        {
            float ratio = 1.0f - ((zombie.transform.position - other.GetContact(0).point).magnitude / explosionRadius);
            if(ratio > 0.0f)
            {
                BaseZombie zombieAI = zombie.GetComponent<BaseZombie>();
                if (zombieAI != null)
                    zombieAI.Damage(ratio * explosionDmg);
            }
        }

        //Disable afterward and re-enable it at a later date.
        renderer.enabled = false;
        collide.enabled = false;
        Invoke("ReloadMissile", 4.0f);
    }
}
