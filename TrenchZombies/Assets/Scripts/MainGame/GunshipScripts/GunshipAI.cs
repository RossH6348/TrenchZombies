using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class GunshipAI : MonoBehaviour
{
    [SerializeField] private GameObject rockets;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject turret;
    private Animator turretAnimate;


    private float rocketCooldown = 0.0f;
    public float maxRocketCooldown = 2.0f;


    private float fireCooldown = 0.0f;
    public float maxFireCooldown = 0.1f;
    public float muzzleVelocity = 60.0f;
    public float fireSpread = 7.5f; //In degrees.

    // Start is called before the first frame update
    void Start()
    {
        //Get our animator component for the turret, so we can animate when the gunship is firing.
        turretAnimate = turret.GetComponent<Animator>();
        turretAnimate.SetBool("isFiring", false);
    }

    private GameObject victim = null;
    // Update is called once per frame
    void Update()
    {

        Vector3 muzzlePoint = turret.transform.position + (turret.transform.forward * 1.5f);

        //Automatically handle turret firing animation if the gunship have a target or not.
        turretAnimate.SetBool("isFiring", victim != null);

        //Do the gunship's logic of finding a target, then firing at it.
        if(victim != null)
        {
            //Start shooting at the target, after calculating the direction vector.
            Vector3 toZombie = ((victim.transform.position + Vector3.up) - muzzlePoint);

            //Make a flat one, to speed up two computations.
            Vector3 toZombieFlat = (new Vector3(toZombie.x,0.0f,toZombie.z)).normalized;

            float distance = (muzzlePoint - victim.transform.position).magnitude;

            //Check if there is nothing in the way, and it is still within the 120 degree of view. (0.154251f is cosine of 30.)
            //(Which 90 - 30 = 60, giving us 60 degree of view in both left and right.)
            if (!Physics.Raycast(muzzlePoint, toZombie.normalized, distance, ~LayerMask.GetMask("Entity")) && Vector3.Dot(transform.forward,toZombieFlat) >= 0.154251f)
            {
                //TIME TO BRING DOOM UPON THE ZOMBIES!!!
                
                //Make the gunship start turning to its target, aiming the missiles.
                Quaternion rot = Quaternion.LookRotation((new Vector3(toZombie.x,0.0f,toZombie.z)).normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3.0f * Time.deltaTime);

                //Aim its turret gun at a faster turning rate toward the victim.
                rot = Quaternion.LookRotation(toZombie);
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, rot, 7.5f * Time.deltaTime);

                //Begin firing countdown.
                if(fireCooldown <= 0.0f)
                {
                    GameObject newBullet = Instantiate(bullet);
                    newBullet.transform.parent = null;

                    newBullet.transform.position = muzzlePoint;
                    newBullet.transform.localScale = new Vector3(100.0f, 100.0f, 100.0f);

                    //Before we give it velocity, I will need to do some complicated
                    //Trigonometry to get a rotated circle infront of the barrel that the radius of it.
                    //Equals to the degree of 5* in both ways, that way we can sample a random point from the circle.
                    //And effectively subtract it from the muzzle to produce a direction vector of a random spread.

                    //That is... if the up and right vector wasn't already rotated for me, which luckily Unity does it anyway.
                    //So it just a simple trigonometry of getting the right scalar value.

                    //Since we got the forward vector as our adjacent line.
                    //We simply need the length of the opposite side to produce an length that
                    //creates a 5* degrees angle. Therefore it is TOA. Where O is T * A

                    //Since the forward vector is length of 1, we only need to do tan.
                    float scalar = Mathf.Tan(fireSpread * Mathf.Deg2Rad);

                    //Sample our random point on the circle, with a random angle then scale it randomly.
                    float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
                    Vector3 offset = (turret.transform.up * Mathf.Sin(angle) + turret.transform.right * Mathf.Cos(angle)).normalized * Random.Range(-scalar, scalar);

                    //Add the offset onto the muzzle's position.
                    offset += muzzlePoint;

                    //Now subtract it from the base position of the turret, and normalize it.
                    Vector3 velocity = (offset - turret.transform.position).normalized;

                    //And now we have our velocity vector with random spread!!!.
                    newBullet.GetComponent<Rigidbody>().velocity = velocity * muzzleVelocity;

                    newBullet.transform.rotation = Quaternion.LookRotation(velocity);

                    newBullet.SetActive(true);

                    fireCooldown = maxFireCooldown;
                }
                else
                {
                    fireCooldown -= Time.deltaTime;
                }

                if(rocketCooldown <= 0.0f)
                {
                    int rocketsCount = rockets.transform.childCount;
                    if (rocketsCount > 0)
                    {
                        GameObject rocket = rockets.transform.GetChild(rocketsCount - 1).gameObject;
                        MissileScript missileScript = rocket.GetComponent<MissileScript>();
                        missileScript.FireMissile(victim.transform.position);
                        rocketCooldown = maxRocketCooldown;
                    }
                }
                else
                {
                    rocketCooldown -= Time.deltaTime;
                }

            }
            else
            {
                //Cannot see the victim anymore as the clear line of fire is distrupted, make their target nothing.
                victim = null;
            }
        }
        else
        {
            //Start scanning for a target.
            float closest = 128.0f;
            GameObject[] victims = GameObject.FindGameObjectsWithTag("Zombie");
            foreach(GameObject potential in victims)
            {
                Vector3 toZombie = ((potential.transform.position + Vector3.up) - muzzlePoint);
                Vector3 toZombieFlat = (new Vector3(toZombie.x, 0.0f, toZombie.z)).normalized;

                float distance = toZombie.magnitude;

                //Check if there is nothing in the way, and it is still within the 120 degree of view. (0.154251f is cosine of 30.)
                //(Which 90 - 30 = 60, giving us 60 degree of view in both left and right.)
                if (!Physics.Raycast(muzzlePoint, toZombie.normalized, distance, ~LayerMask.GetMask("Entity")) && Vector3.Dot(transform.forward, toZombieFlat) >= 0.154251f)
                {
                    if(distance <= closest)
                    {
                        closest = distance;
                        victim = potential;
                    }
                }
            }
        }
    }
}
