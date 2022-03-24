using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiesData : MonoBehaviour
{
    //Since there is going to be alot of zombies, I am going to use object pooling in this scenario.
    [SerializeField] private Transform zombiePool;
    [SerializeField] private Transform zombieSpawns;

    //This will contain list of all type of zombies that exist by name.
    public List<string> ZombiesName = new List<string>();

    //Using the name as a key, we can get all the data related to that zombie.
    private Dictionary<string, float> ZombiesHealth = new Dictionary<string, float>(); //How much health will they spawn with?
    private Dictionary<string, float> ZombiesSpeed = new Dictionary<string, float>(); //How fast can they move?
    private Dictionary<string, Vector3> ZombiesScale = new Dictionary<string, Vector3>(); //How large or small will they physically be?
    private Dictionary<string, float> ZombiesProbability = new Dictionary<string, float>(); //What the odds of this zombie spawning when we need to spawn a zombie.

    public void addZombie(string name, float maxHealth, float speed, Vector3 scale, float probability)
    {
        //Check if this zombie type does not exist, if so we can create a new database.
        if (!ZombiesName.Contains(name))
        {
            //Add this new type of zombie to the database.
            ZombiesName.Add(name);
            ZombiesHealth.Add(name, maxHealth);
            ZombiesSpeed.Add(name, speed);
            ZombiesScale.Add(name, scale);
            ZombiesProbability.Add(name, probability);
        }
    }

    //This funciton will spawn a random zombie from the database, given a set of spawn points to randomly spawn them at.
    public GameObject spawnZombie(string isSpecial = "")
    {

        //Don't bother spawning a zombie if there is no type to spawn, or a spawn point to send it to.
        if (zombieSpawns.childCount < 1 || ZombiesName.Count < 1)
            return null;

        //isSpecial - Determines where the wave is a special wave where only one type of zombie will spawn.
        //But before we start applying the attributes from the database to a zombie, we need to find if there is a dead one
        //available for us to re-use from the object pool.
        GameObject zombie = null;
        for (int i = zombiePool.childCount - 1; i > -1; i--)
        {
            if (!zombiePool.GetChild(i).gameObject.activeSelf)
            {
                zombie = zombiePool.GetChild(i).gameObject;
                break;
            }
        }

        if (zombie != null)
        {

            BaseZombie baseAI = zombie.GetComponent<BaseZombie>();
            NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
            //We got a zombie we can apply attributes to, let pick a random zombie type, or elsewise use the isSpecial.
            if (isSpecial == "")
            {
                //Pick a random zombie based on probability, which is a little complicated to handle...
                //As it involves adding up all the probabilities, picking a random number out of that range and then comparing it down to zero.
                int index = 0;
                float rng = 0.0f;
                for(int i = 0; i < ZombiesName.Count; i++)
                    rng += ZombiesProbability[ZombiesName[i]];

                rng = Random.Range(0.0f, rng);
                while(index < ZombiesName.Count)
                {
                    string name = ZombiesName[index];
                    if(ZombiesProbability[name] < rng)
                    {
                        rng -= ZombiesProbability[name];
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                //We got a random zombie to pick from, let use its database.
                isSpecial = ZombiesName[index];
            }

            zombie.name = isSpecial;
            baseAI.Health = ZombiesHealth[isSpecial];
            zombie.transform.localScale = ZombiesScale[isSpecial];
            agent.speed = ZombiesSpeed[isSpecial];

            zombie.transform.position = zombieSpawns.GetChild(Random.Range(0, zombieSpawns.childCount - 1)).position;

            zombie.SetActive(true);

        }

        return zombie;
    }
}
