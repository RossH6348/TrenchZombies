using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float dmg = 5.0f;
    [SerializeField] private float lifeTime = 10.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
            Destroy(gameObject);

    }

    public void OnCollisionEnter(Collision other)
    {
        //See if it have collided with an entity, and if so damage it.
        if (other.collider.tag.Equals("Zombie"))
        {
            BaseZombie zombieAI = other.gameObject.GetComponent<BaseZombie>();
            if (zombieAI != null)
                zombieAI.Damage(dmg);
        }

        //Destroy itself.
        Destroy(gameObject);
    }

}
