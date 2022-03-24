using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Name: Ross Hutchins
//ID: HUT18001284

public class BaseZombie : MonoBehaviour
{

    [SerializeField] private MainGameScript mainGame;

    private Animator animate;
    private NavMeshAgent agent;
    private Rigidbody rigid;

    [SerializeField] private Transform eyePoint;

    public Transform player;
    public float Health = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        animate = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        animate.SetBool(
            "isWalking",
            !agent.isStopped
        );

        animate.SetBool(
            "isAttacking",
            agent.isStopped
        );

        //This is to determine what sort of AI the zombie should do.
        bool canSeePlayer = false;
        Vector3 toPlayer = ((player.position + new Vector3(0.0f, 1.0f, 0.0f)) - eyePoint.position);

        RaycastHit result;
        //Fire a raycast that doesn't intersect with the layer the zombie is on, which is Entity.
        //So only fire a raycast on the default and player layer.
        if (Physics.Raycast(eyePoint.position, toPlayer.normalized, out result, toPlayer.magnitude, LayerMask.GetMask("Default", "Player")))
        {
            //See if it is the player we have collided with.
            if (result.collider.name.Contains("Player"))
                canSeePlayer = true; //Yes it is, chase the player instead.
        }

        //Extremely basic, if close to target, then attack target elsewise walk to target.
        Vector3 targetPosition = new Vector3(0.0f, 0.1f, 0.0f); //The fortress is at the origin.
        if (canSeePlayer)
            targetPosition = player.position; //Update to chase the player instead.


        if ((transform.position - targetPosition).magnitude > 1.5f)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition);
        }
        else
        {
            agent.isStopped = true;
            if (!canSeePlayer)
            {
                //Begin draining the fortress' health, if they are not targetting the player.
                mainGame.DamageFortress();
            }
        }
    }

    public void Attack()
    {
        //Attempt to attack the player, by raycasting to see if they are infront of them.
        //I am having to multiply the forward vector by negative 1.5f, cuz the forward vector is facing the opposite direction within the model.
        RaycastHit result;
        if (Physics.Raycast(eyePoint.position, eyePoint.forward * -1.0f, out result, 1.5f) && result.collider.name.Contains("Player"))
        {
            //They have attacked the player! Do damage to the player.
            result.collider.GetComponent<PlayerMovement>().entity.Damage(20);
        }
    }

    public void Damage(float dmg)
    {
        //No need to damage zombie, if their health is 0 or below...
        if (Health > 0.0f)
        {
            Health -= dmg;
            if (Health <= 0.0f)
            {
                //Tell the main game to reward the player points for killing.
                player.GetComponent<PlayerMovement>().mainGame.onKill(gameObject);

                gameObject.SetActive(false);
            }
        }
    }
}
