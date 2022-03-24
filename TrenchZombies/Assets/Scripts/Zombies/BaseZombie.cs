using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Name: Ross Hutchins
//ID: HUT18001284

public class BaseZombie : MonoBehaviour
{

    private Animator animate;
    private NavMeshAgent agent;
    private Rigidbody rigid;

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

        //Extremely basic, if close to player stop and attack player.
        if ((transform.position - player.position).magnitude > 1.0f)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }

    }

    public void Damage(float dmg)
    {
        Health -= dmg;
        if (Health <= 0.0f)
            gameObject.SetActive(false);
    }
}
